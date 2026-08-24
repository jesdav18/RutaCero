using RutaCero.Domain.Transactions;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Reconciliation;
public sealed class BalanceReconciliationService
{
 public Money Calculate(Money opening,Guid accountId,IEnumerable<Transaction> transactions)
 {var balance=opening;foreach(var x in transactions.Where(x=>x.Amount.Currency==opening.Currency)){if(x.Type is TransactionType.Income or TransactionType.Refund)balance+=x.Amount;else if(x.Type==TransactionType.Transfer){if(x.TransferDirection==TransferDirection.Incoming&&x.FinancialAccountId==accountId)balance+=x.Amount;else if(x.TransferDirection==TransferDirection.Outgoing&&x.FinancialAccountId==accountId)balance-=x.Amount;else if(x.TransferDirection is null&&x.FinancialAccountId==accountId)balance-=x.Amount;else if(x.TransferDirection is null&&x.RelatedFinancialAccountId==accountId)balance+=x.Amount;}else if(x.FinancialAccountId==accountId&&x.Type is (TransactionType.Expense or TransactionType.DebtPayment or TransactionType.Interest or TransactionType.Fee))balance-=x.Amount;}return balance;}
}
