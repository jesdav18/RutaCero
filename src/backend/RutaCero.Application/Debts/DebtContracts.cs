using RutaCero.Domain.Debts;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Application.Debts;

public sealed record DebtDto(Guid Id, string InstitutionName, string Name, DebtType Type,
    decimal OriginalPrincipal, decimal CurrentPrincipal, Currency Currency, decimal? AnnualInterestRate,
    decimal RegularPayment, bool AllowsCapitalPrepayment, bool HasPrepaymentPenalty, DebtStatus Status,
    decimal ProgressPercentage,int? StatementClosingDay,int? PaymentDueDay,bool AutoGeneratePaymentObligations);
public sealed record CreateDebtCommand(string InstitutionName, string Name, DebtType Type,
    decimal OriginalPrincipal, Currency Currency, decimal? AnnualInterestRate, decimal RegularPayment,
    bool AllowsCapitalPrepayment, bool HasPrepaymentPenalty,int? StatementClosingDay=null,int? PaymentDueDay=null,bool AutoGeneratePaymentObligations=false);
public sealed record UpdateDebtCommand(string InstitutionName, string Name, DebtType Type,
    decimal? AnnualInterestRate, decimal RegularPayment, bool AllowsCapitalPrepayment, bool HasPrepaymentPenalty,
    int? StatementClosingDay=null,int? PaymentDueDay=null,bool AutoGeneratePaymentObligations=false);
public sealed record RegisterDebtPaymentCommand(DateOnly PaymentDate, decimal TotalAmount,
    decimal? PrincipalAmount, PaymentType Type, bool IsAllocationConfirmed);
public sealed record ConfirmDebtBalanceCommand(Guid? StatementImportId,DateOnly StatementDate,decimal Balance);
public sealed record DebtBalanceSnapshotDto(Guid Id,Guid DebtId,Guid? StatementImportId,DateOnly StatementDate,decimal Balance,Currency Currency,DateTimeOffset CreatedAt);

public interface IDebtRepository
{
    Task<IReadOnlyList<Debt>> ListAsync(Guid userId, CancellationToken token);
    Task<Debt?> FindAsync(Guid id, Guid userId, CancellationToken token);
    Task AddAsync(Debt debt, CancellationToken token);
}

public interface IDebtPaymentRepository
{
    Task AddAsync(DebtPayment payment, CancellationToken token);
}
public interface IDebtBalanceSnapshotRepository
{
    Task<IReadOnlyList<DebtBalanceSnapshot>> ListAsync(Guid debtId,Guid userId,CancellationToken token);
    Task<bool> ExistsForImportAsync(Guid statementImportId,Guid userId,CancellationToken token);
    Task AddAsync(DebtBalanceSnapshot snapshot,CancellationToken token);
}
