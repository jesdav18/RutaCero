using RutaCero.Domain.Transactions;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Tests;
public sealed class CategorizationTests
{
 [Fact]public void Highest_priority_matching_rule_wins(){var user=Guid.NewGuid();var account=Guid.NewGuid();var low=new CategorizationRule(user,"General",Guid.NewGuid(),DescriptionMatchType.Contains,"super",null,null,null,null,Currency.HNL,TransactionType.Expense,1);var category=Guid.NewGuid();var high=new CategorizationRule(user,"Cuenta",category,DescriptionMatchType.StartsWith,"Supermercado",null,account,100,2000,Currency.HNL,TransactionType.Expense,10);Assert.Equal(category,new CategorizationService().Categorize([low,high],"SUPERMERCADO LA COLONIA",null,account,new(500,Currency.HNL),TransactionType.Expense));}
}
