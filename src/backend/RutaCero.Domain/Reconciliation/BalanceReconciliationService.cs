using RutaCero.Domain.Transactions;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Reconciliation;
public sealed class BalanceReconciliationService
{
 public Money Calculate(Money opening,Guid accountId,IEnumerable<Transaction> transactions)
 {var balance=opening;foreach(var x in transactions.Where(x=>x.Amount.Currency==opening.Currency)){if(x.Type is TransactionType.Income or TransactionType.Refund)balance+=x.Amount;else if(x.FinancialAccountId==accountId&&x.Type is (TransactionType.Expense or TransactionType.DebtPayment or TransactionType.Interest or TransactionType.Fee or TransactionType.Transfer))balance-=x.Amount;else if(x.RelatedFinancialAccountId==accountId&&x.Type==TransactionType.Transfer)balance+=x.Amount;}return balance;}
}
