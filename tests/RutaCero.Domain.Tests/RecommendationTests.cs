using RutaCero.Domain.Debts;using RutaCero.Domain.Planning;using RutaCero.Domain.Recommendations;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Tests;
public sealed class RecommendationTests
{
 [Theory][InlineData(RecommendationStrategy.Avalanche,"Alta")][InlineData(RecommendationStrategy.Snowball,"Pequeña")][InlineData(RecommendationStrategy.CashFlowRelease,"Corta")]
 public void Selects_target_by_strategy(RecommendationStrategy strategy,string expected)
 {var context=Context(false);var result=new CapitalPaymentRecommendationService().Recommend(context,strategy,RecommendationProfile.Aggressive);Assert.Equal(expected,context.Debts.Single(x=>x.Debt.Id==result.TargetDebtId).Debt.Name);}
 [Fact]public void Blocks_when_an_obligation_is_overdue(){var result=new CapitalPaymentRecommendationService().Recommend(Context(true),RecommendationStrategy.Avalanche,RecommendationProfile.Balanced);Assert.Null(result.TargetDebtId);Assert.Contains(result.Blockers,x=>x.Contains("vencido"));}
 [Fact]public void Estimates_savings_for_a_recommended_payment(){var result=new CapitalPaymentRecommendationService().Recommend(Context(false),RecommendationStrategy.Avalanche,RecommendationProfile.Aggressive);Assert.True(result.EstimatedInterestSavings.Amount>0);Assert.True(result.EstimatedMonthsSaved>0);}
 private static RecommendationContext Context(bool overdue)
 {
  var u=Guid.NewGuid();var debts=new List<RecommendationDebt>
  {
   new(new Debt(u,"Banco","Alta",DebtType.PersonalLoan,new Money(10000,Currency.HNL),30,new Money(500,Currency.HNL),true,false),24),
   new(new Debt(u,"Banco","Pequeña",DebtType.PersonalLoan,new Money(2000,Currency.HNL),10,new Money(200,Currency.HNL),true,false),10),
   new(new Debt(u,"Banco","Corta",DebtType.PersonalLoan,new Money(5000,Currency.HNL),12,new Money(900,Currency.HNL),true,false),3)
  };
  return new(new Money(1000,Currency.HNL),debts,overdue,true,false,false,false);
 }
}
