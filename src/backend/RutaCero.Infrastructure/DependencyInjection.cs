using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RutaCero.Application;
using RutaCero.Application.Accounts;
using RutaCero.Application.Auth;
using RutaCero.Application.Debts;
using RutaCero.Application.Transactions;
using RutaCero.Application.Obligations;
using RutaCero.Application.Planning;
using RutaCero.Application.Notifications;
using RutaCero.Application.Imports;
using RutaCero.Infrastructure.Imports;
using RutaCero.Application.ExchangeRates;
using RutaCero.Infrastructure.Auth;
using RutaCero.Infrastructure.Persistence;
using Npgsql;

namespace RutaCero.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is required.");
        var connectionOptions = new NpgsqlConnectionStringBuilder(connection)
        {
            Timeout = 10,
            CommandTimeout = 30,
            KeepAlive = 30,
            ConnectionIdleLifetime = 60,
            ConnectionPruningInterval = 10,
            MaxPoolSize = 20
        };
        var resilientConnection = connectionOptions.ConnectionString;
        services.AddDbContext<RutaCeroDbContext>(options => options.UseNpgsql(resilientConnection,npgsql =>
            npgsql.EnableRetryOnFailure(3,TimeSpan.FromSeconds(2),null)));
        services.AddScoped<IFinancialAccountRepository, FinancialAccountRepository>();
        services.AddScoped<IBalanceSnapshotRepository, BalanceSnapshotRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddSingleton<IPasswordService, PasswordService>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<IDebtRepository, DebtRepository>();
        services.AddScoped<IDebtPaymentRepository, DebtPaymentRepository>();
        services.AddScoped<IDebtBalanceSnapshotRepository, DebtBalanceSnapshotRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITransactionTypeSettingRepository, TransactionTypeSettingRepository>();
        services.AddScoped<ICategorizationRuleRepository, CategorizationRuleRepository>();
        services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
        services.AddScoped<IObligationRepository, ObligationRepository>();
        services.AddScoped<IPlanningRepository, PlanningRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IStatementImportRepository, StatementImportRepository>();
        services.AddSingleton<IPrivateFileStorage, LocalPrivateFileStorage>();
        services.AddSingleton<IStatementParser, CsvStatementParser>();
        services.AddSingleton<IStatementParser, XlsxStatementParser>();
        services.AddSingleton<IStatementParser, PdfStatementParser>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<RutaCeroDbContext>());
        services.AddHealthChecks().AddNpgSql(resilientConnection, name: "postgresql");
        return services;
    }
}
