using Microsoft.EntityFrameworkCore;
using RutaCero.Domain.Debts;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Infrastructure.Persistence;

internal static class DebtConfiguration
{
    public static void Configure(ModelBuilder builder)
    {
        var debt = builder.Entity<Debt>(); debt.ToTable("debts"); debt.HasKey(x => x.Id);
        debt.Property(x => x.Id).HasColumnName("id"); debt.Property(x => x.UserId).HasColumnName("user_id");
        debt.Property(x => x.InstitutionName).HasColumnName("institution_name"); debt.Property(x => x.Name).HasColumnName("name");
        debt.Property(x => x.Type).HasColumnName("debt_type").HasConversion<string>();
        debt.Property(x => x.Status).HasColumnName("status").HasConversion<string>();
        debt.Ignore(x => x.OriginalPrincipal); debt.Ignore(x => x.CurrentPrincipal); debt.Ignore(x => x.RegularPayment);
        debt.Property<decimal>("_originalPrincipalAmount").HasColumnName("original_principal").HasPrecision(18, 2);
        debt.Property<decimal>("_currentPrincipalAmount").HasColumnName("current_principal").HasPrecision(18, 2);
        debt.Property<decimal>("_regularPaymentAmount").HasColumnName("regular_payment").HasPrecision(18, 2);
        debt.Property<Currency>("_currency").HasColumnName("currency").HasConversion<string>();
        debt.Property(x => x.AnnualInterestRate).HasColumnName("annual_interest_rate").HasPrecision(9, 6);
        debt.Property(x => x.AllowsCapitalPrepayment).HasColumnName("allows_capital_prepayment");
        debt.Property(x => x.HasPrepaymentPenalty).HasColumnName("has_prepayment_penalty"); debt.HasIndex(x => x.UserId);
        debt.Property(x=>x.StatementClosingDay).HasColumnName("statement_closing_day");
        debt.Property(x=>x.PaymentDueDay).HasColumnName("payment_due_day");
        debt.Property(x=>x.AutoGeneratePaymentObligations).HasColumnName("auto_generate_payment_obligations");
        var payment = builder.Entity<DebtPayment>(); payment.ToTable("debt_payments"); payment.HasKey(x => x.Id);
        payment.Property(x => x.Id).HasColumnName("id"); payment.Property(x => x.DebtId).HasColumnName("debt_id");
        payment.Property(x => x.PaymentDate).HasColumnName("payment_date");
        payment.Property(x => x.Type).HasColumnName("payment_type").HasConversion<string>();
        payment.Property(x => x.IsAllocationConfirmed).HasColumnName("is_allocation_confirmed");
        payment.Ignore(x => x.TotalAmount); payment.Ignore(x => x.PrincipalAmount);
        payment.Property<decimal>("_totalAmount").HasColumnName("total_amount").HasPrecision(18, 2);
        payment.Property<decimal?>("_principalAmount").HasColumnName("principal_amount").HasPrecision(18, 2);
        payment.Property<Currency>("_currency").HasColumnName("currency").HasConversion<string>();
        var snapshot=builder.Entity<DebtBalanceSnapshot>();snapshot.ToTable("debt_balance_snapshots");snapshot.HasKey(x=>x.Id);
        snapshot.Property(x=>x.Id).HasColumnName("id");snapshot.Property(x=>x.UserId).HasColumnName("user_id");snapshot.Property(x=>x.DebtId).HasColumnName("debt_id");snapshot.Property(x=>x.StatementImportId).HasColumnName("statement_import_id");snapshot.Property(x=>x.StatementDate).HasColumnName("statement_date");snapshot.Ignore(x=>x.Balance);snapshot.Property<decimal>("_balanceAmount").HasColumnName("balance").HasPrecision(18,2);snapshot.Property<Currency>("_currency").HasColumnName("currency").HasConversion<string>();snapshot.Property(x=>x.CreatedAt).HasColumnName("created_at");snapshot.HasIndex(x=>new{x.UserId,x.DebtId,x.StatementDate});snapshot.HasIndex(x=>x.StatementImportId).IsUnique();
    }
}
