using RutaCero.Application.Accounts;using RutaCero.Application.Common;using RutaCero.Application.Transactions;using RutaCero.Domain.Reconciliation;
namespace RutaCero.Application.Reconciliation;
public sealed record ReconciliationDto(Guid FinancialAccountId,decimal ConfirmedBalance,decimal CalculatedBalance,decimal Difference,string Currency,DateOnly ConfirmedDate,string Confidence);
public sealed class ReconciliationService(IFinancialAccountRepository accounts,IBalanceSnapshotRepository snapshots,ITransactionRepository transactions)
{
 public async Task<Result<ReconciliationDto>> GetAsync(Guid userId,Guid accountId,CancellationToken token)
 {var account=await accounts.FindAsync(accountId,userId,token);if(account is null)return Result<ReconciliationDto>.Failure("La cuenta no existe.");var history=await snapshots.ListAsync(accountId,userId,token);var opening=history.FirstOrDefault();var openingBalance=opening?.Balance??account.CurrentBalance;var from=opening is null?(DateOnly?)null:opening.SnapshotDate.AddDays(1);var movements=await transactions.ListAsync(userId,from,null,token);var calculated=new BalanceReconciliationService().Calculate(openingBalance,accountId,movements);return Result<ReconciliationDto>.Success(new(accountId,account.CurrentBalance.Amount,calculated.Amount,account.CurrentBalance.Amount-calculated.Amount,account.CurrentBalance.Currency.ToString(),account.CurrentBalanceDate,"Media"));}
}
