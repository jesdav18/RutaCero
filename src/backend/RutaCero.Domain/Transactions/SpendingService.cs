using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Transactions;

public sealed class SpendingService
{
    public Money Calculate(IEnumerable<Transaction> transactions, Currency currency) =>
        transactions.Where(x=>x.CountsAsExpense&&x.Amount.Currency==currency)
            .Aggregate(Money.Zero(currency),(total,item)=>total+item.Amount);
}
