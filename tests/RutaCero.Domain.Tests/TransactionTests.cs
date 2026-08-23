using RutaCero.Domain.Transactions;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Tests;

public sealed class TransactionTests
{
    [Fact]
    public void Internal_transfer_and_card_payment_are_not_double_counted()
    {
        var user=Guid.NewGuid();var account=Guid.NewGuid();var date=new DateOnly(2026,8,19);
        var items=new[]{
            new Transaction(user,account,null,null,TransactionType.Expense,new(1500,Currency.HNL),date,"Supermercado",DateTimeOffset.UtcNow),
            new Transaction(user,account,Guid.NewGuid(),null,TransactionType.DebtPayment,new(1500,Currency.HNL),date,"Pago tarjeta",DateTimeOffset.UtcNow)
        };
        Assert.Equal(1500,new SpendingService().Calculate(items,Currency.HNL).Amount);
    }
}
