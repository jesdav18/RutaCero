using Microsoft.EntityFrameworkCore;
using RutaCero.Application;
using RutaCero.Domain.Accounts;
using RutaCero.Domain.Users;
using RutaCero.Domain.ValueObjects;
using RutaCero.Domain.Debts;
using RutaCero.Domain.Transactions;
using RutaCero.Domain.Obligations;
using RutaCero.Domain.Planning;
using RutaCero.Domain.Notifications;
using RutaCero.Domain.Imports;
using RutaCero.Domain.ExchangeRates;

namespace RutaCero.Infrastructure.Persistence;

public sealed class RutaCeroDbContext(DbContextOptions<RutaCeroDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<FinancialAccount> FinancialAccounts => Set<FinancialAccount>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<DebtPayment> DebtPayments => Set<DebtPayment>();
    public DbSet<DebtBalanceSnapshot> DebtBalanceSnapshots => Set<DebtBalanceSnapshot>();
    public DbSet<BalanceSnapshot> BalanceSnapshots => Set<BalanceSnapshot>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<TransactionCategory> TransactionCategories => Set<TransactionCategory>();
    public DbSet<TransactionTypeSetting> TransactionTypeSettings => Set<TransactionTypeSetting>();
    public DbSet<PaymentObligation> PaymentObligations => Set<PaymentObligation>();
    public DbSet<ExpectedIncome> ExpectedIncomes=>Set<ExpectedIncome>();
    public DbSet<RecurringCommitment> RecurringCommitments=>Set<RecurringCommitment>();
    public DbSet<MonthlyBudget> MonthlyBudgets=>Set<MonthlyBudget>();
    public DbSet<UserFinancialSettings> UserFinancialSettings=>Set<UserFinancialSettings>();
    public DbSet<Notification> Notifications=>Set<Notification>();
    public DbSet<StatementImport> StatementImports=>Set<StatementImport>();
    public DbSet<StatementImportRow> StatementImportRows=>Set<StatementImportRow>();
    public DbSet<CategorizationRule> CategorizationRules=>Set<CategorizationRule>();
    public DbSet<ExchangeRate> ExchangeRates=>Set<ExchangeRate>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var account = builder.Entity<FinancialAccount>();
        account.ToTable("financial_accounts");
        account.HasKey(x => x.Id);
        account.Property(x => x.Id).HasColumnName("id");
        account.Property(x => x.UserId).HasColumnName("user_id");
        account.Property(x => x.InstitutionName).HasColumnName("institution_name").HasMaxLength(120);
        account.Property(x => x.DisplayName).HasColumnName("display_name").HasMaxLength(120);
        account.Property(x => x.Reference).HasColumnName("account_reference").HasConversion(x => x.Value, x => new(x));
        account.Property(x => x.Type).HasColumnName("account_type").HasConversion<string>();
        account.Ignore(x => x.CurrentBalance);
        account.Property<decimal>("_currentBalanceAmount").HasColumnName("current_balance").HasPrecision(18, 2);
        account.Property<Currency>("_currency").HasColumnName("currency").HasConversion<string>();
        account.Property(x => x.CurrentBalanceDate).HasColumnName("current_balance_date");
        account.Property(x=>x.BalanceSource).HasColumnName("balance_source").HasConversion<string>();
        account.Property(x=>x.BalanceConfidence).HasColumnName("balance_confidence").HasConversion<string>();
        account.Ignore(x => x.MinimumBuffer);
        account.Property<decimal>("_minimumBufferAmount").HasColumnName("minimum_buffer").HasPrecision(18, 2);
        account.Property(x => x.IsIncludedInAvailableCash).HasColumnName("is_included_in_available_cash");
        account.Property(x => x.IsActive).HasColumnName("is_active");
        account.Property(x=>x.CreatedAt).HasColumnName("created_at");
        account.Property(x=>x.UpdatedAt).HasColumnName("updated_at");
        account.HasIndex(x => x.UserId);
        ConfigureAuth(builder);
        DebtConfiguration.Configure(builder);
        BalanceConfiguration.Configure(builder);
        TransactionConfiguration.Configure(builder);
        ObligationConfiguration.Configure(builder);
        PlanningConfiguration.Configure(builder);
        NotificationConfiguration.Configure(builder);
        StatementImportConfiguration.Configure(builder);
        CategorizationRuleConfiguration.Configure(builder);
        ExchangeRateConfiguration.Configure(builder);
    }

    private static void ConfigureAuth(ModelBuilder builder)
    {
        var user = builder.Entity<User>();
        user.ToTable("users"); user.HasKey(x => x.Id);
        user.Property(x => x.Id).HasColumnName("id");
        user.Property(x => x.Email).HasColumnName("email");
        user.Property(x => x.PasswordHash).HasColumnName("password_hash");
        user.Property(x => x.CreatedAt).HasColumnName("created_at");
        user.HasIndex(x => x.Email).IsUnique();
        var refresh = builder.Entity<RefreshToken>();
        refresh.ToTable("refresh_tokens"); refresh.HasKey(x => x.Id);
        refresh.Property(x => x.Id).HasColumnName("id");
        refresh.Property(x => x.UserId).HasColumnName("user_id");
        refresh.Property(x => x.TokenHash).HasColumnName("token_hash");
        refresh.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        refresh.Property(x => x.CreatedAt).HasColumnName("created_at");
        refresh.Property(x => x.RevokedAt).HasColumnName("revoked_at");
        refresh.Property(x => x.ReplacedById).HasColumnName("replaced_by_id");
        refresh.HasIndex(x => x.TokenHash).IsUnique();
        refresh.HasOne<User>().WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);
        refresh.HasOne<RefreshToken>().WithMany().HasForeignKey(x=>x.ReplacedById).OnDelete(DeleteBehavior.NoAction);
    }
}
