using RutaCero.Domain.Reconciliation;using RutaCero.Domain.Transactions;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Tests;
public sealed class ReconciliationTests
{
 [Fact]public void Calculates_balance_with_incoming_and_outgoing_movements(){var user=Guid.NewGuid();var account=Guid.NewGuid();var other=Guid.NewGuid();var date=new DateOnly(2026,8,19);var now=DateTimeOffset.UtcNow;var items=new[]{new Transaction(user,account,null,null,TransactionType.Expense,new(100,Currency.HNL),date,"Compra",now),new Transaction(user,other,account,null,TransactionType.Transfer,new(250,Currency.HNL),date,"Transferencia",now)};Assert.Equal(1150,new BalanceReconciliationService().Calculate(new(1000,Currency.HNL),account,items).Amount);}
}
