using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using RutaCero.Application.Accounts;
using RutaCero.Application.Auth;
using RutaCero.Application.Debts;
using RutaCero.Application.Transactions;
using RutaCero.Application.Obligations;
using RutaCero.Application.Planning;
using RutaCero.Application.Recommendations;
using RutaCero.Application.Notifications;
using RutaCero.Application.Imports;
using RutaCero.Application.Reconciliation;
using RutaCero.Application.Dashboard;
using RutaCero.Application.ExchangeRates;
using FluentValidation;
using FluentValidation.AspNetCore;
using RutaCero.Infrastructure;
using RutaCero.Api.Errors;
using RutaCero.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<BalanceService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<DebtService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<TransactionTypeSettingService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CategorizationRuleService>();
builder.Services.AddScoped<ObligationService>();
builder.Services.AddScoped<PlanningService>();
builder.Services.AddScoped<BudgetProgressService>();
builder.Services.AddScoped<RecommendationApplicationService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<StatementImportService>();
builder.Services.AddScoped<ReconciliationService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ExpenseAnalyticsService>();
builder.Services.AddScoped<ExchangeRateService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRateLimiter(options => options.AddFixedWindowLimiter("api", policy =>
{
    policy.PermitLimit = 100;
    policy.Window = TimeSpan.FromMinutes(1);
}));
var key = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true,
        ValidateIssuerSigningKey = true, ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});
builder.Services.AddAuthorization();
var origins=builder.Configuration.GetSection("Cors:Origins").Get<string[]>()??[];
builder.Services.AddCors(options=>options.AddPolicy("web",policy=>
{
    if(origins.Length>0)policy.WithOrigins(origins);
    else policy.SetIsOriginAllowed(_=>false);
    policy.AllowAnyHeader().AllowAnyMethod();
}));
var app = builder.Build();
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RutaCeroDbContext>();
    await db.Database.ExecuteSqlRawAsync("""
        create table if not exists transaction_type_settings(
            user_id uuid not null references users(id) on delete cascade,
            code varchar(30) not null,
            label varchar(80) not null,
            effect varchar(12) not null check(effect in('Positive','Negative','Neutral')),
            primary key(user_id,code)
        );
        insert into transaction_type_settings(user_id,code,label,effect)
        select u.id,v.code,v.label,v.effect
        from users u
        cross join (values
            ('Income','Ingreso','Positive'),('Expense','Gasto','Negative'),
            ('Transfer','Transferencia','Neutral'),('DebtPayment','Pago de deuda','Negative'),
            ('Interest','Interés','Negative'),('Fee','Comisión','Negative'),
            ('Refund','Reembolso','Positive'),('Adjustment','Ajuste','Neutral')
        ) as v(code,label,effect)
        on conflict(user_id,code) do nothing;
        """);
    await db.Database.ExecuteSqlRawAsync("""
        alter table transactions add column if not exists transfer_group_id uuid;
        alter table transactions add column if not exists debt_id uuid references debts(id);
        alter table transactions add column if not exists transfer_direction varchar(12)
            check(transfer_direction in('Outgoing','Incoming'));
        create index if not exists ix_transactions_transfer_group on transactions(transfer_group_id)
            where transfer_group_id is not null;
        create index if not exists ix_transactions_debt_id on transactions(debt_id) where debt_id is not null;
        """);
    await db.Database.ExecuteSqlRawAsync("""
        alter table debts add column if not exists statement_closing_day integer check(statement_closing_day between 1 and 31);
        alter table debts add column if not exists payment_due_day integer check(payment_due_day between 1 and 31);
        alter table debts add column if not exists auto_generate_payment_obligations boolean not null default false;
        create unique index if not exists uq_payment_obligation_debt_due_type
            on payment_obligations(debt_id,due_date,obligation_type)
            where debt_id is not null and payment_status<>'Cancelled';
        """);
    await db.Database.ExecuteSqlRawAsync("""
        insert into transaction_categories(id,user_id,name,is_income,is_system)
        select gen_random_uuid(),null,'Cambio de moneda',false,true
        where not exists (
            select 1 from transaction_categories where user_id is null and name='Cambio de moneda'
        );
        """);
    await db.Database.ExecuteSqlRawAsync("""
        create table if not exists debt_balance_snapshots(
            id uuid primary key,user_id uuid not null references users(id) on delete cascade,
            debt_id uuid not null references debts(id) on delete cascade,
            statement_import_id uuid null references statement_imports(id) on delete set null,
            statement_date date not null,balance numeric(18,2) not null check(balance>=0),
            currency varchar(3) not null,created_at timestamptz not null
        );
        create index if not exists ix_debt_balance_snapshots_user_debt_date on debt_balance_snapshots(user_id,debt_id,statement_date desc);
        create unique index if not exists ux_debt_balance_snapshots_statement_import on debt_balance_snapshots(statement_import_id) where statement_import_id is not null;
        """);
}
app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors("web");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("api");
app.MapHealthChecks("/health");
app.Run();

public partial class Program;
