using RutaCero.Domain.Debts;using RutaCero.Domain.Planning;using RutaCero.Domain.ValueObjects;
using RutaCero.Domain.Accounts;
namespace RutaCero.Domain.Recommendations;
public enum RecommendationStrategy { Avalanche,Snowball,CashFlowRelease,Hybrid }
public sealed record RecommendationDebt(Debt Debt,int? RemainingMonths);
public sealed record RecommendationContext(Money Available,IReadOnlyList<RecommendationDebt> Debts,
 bool HasOverdueObligation,bool HasNextIncome,bool HasStaleBalances,bool HasReconciliationDifference,bool HasUncoveredEssentials);
public sealed record CapitalPaymentRecommendation(Money Available,Money Recommended,Guid? TargetDebtId,
 RecommendationStrategy Strategy,RecommendationProfile Profile,string Explanation,Money Remaining,
 DataConfidence Confidence,IReadOnlyList<string> Warnings,IReadOnlyList<string> Blockers,Money EstimatedInterestSavings,int EstimatedMonthsSaved);
public sealed class CapitalPaymentRecommendationService
{
 public CapitalPaymentRecommendation Recommend(RecommendationContext context,RecommendationStrategy strategy,RecommendationProfile profile)
 {
  var blockers=Blockers(context);var zero=Money.Zero(context.Available.Currency);
  if(blockers.Count>0)return new(context.Available,zero,null,strategy,profile,"No es seguro realizar un abono.",context.Available,DataConfidence.Low,[],blockers,zero,0);
  var eligible=context.Debts.Where(x=>x.Debt.Status==DebtStatus.Active&&x.Debt.AllowsCapitalPrepayment&&x.Debt.CurrentPrincipal.Currency==context.Available.Currency&&!x.Debt.HasPrepaymentPenalty).ToList();
  if(eligible.Count==0)return new(context.Available,zero,null,strategy,profile,"No hay deudas elegibles para abono.",context.Available,DataConfidence.Medium,[],["No existe una deuda elegible en esta moneda."],zero,0);
  var target=Select(eligible,strategy);var factor=profile switch{RecommendationProfile.Conservative=>.35m,RecommendationProfile.Balanced=>.65m,_=>1m};
  var amount=new Money(Math.Min(context.Available.Amount*factor,target.Debt.CurrentPrincipal.Amount),context.Available.Currency);
  var months=Math.Min(target.RemainingMonths??0,(int)Math.Ceiling(amount.Amount/Math.Max(1,target.Debt.RegularPayment.Amount)));var savings=new Money(decimal.Round(amount.Amount*(target.Debt.AnnualInterestRate??0)/1200*months,2),amount.Currency);
  return new(context.Available,amount,target.Debt.Id,strategy,profile,$"Prioriza {target.Debt.Name} con la estrategia {strategy}.",context.Available-amount,DataConfidence.High,[],[],savings,months);
 }
 private static RecommendationDebt Select(List<RecommendationDebt> debts,RecommendationStrategy strategy)=>strategy switch
 {RecommendationStrategy.Avalanche=>debts.OrderByDescending(x=>x.Debt.AnnualInterestRate??0).First(),RecommendationStrategy.Snowball=>debts.OrderBy(x=>x.Debt.CurrentPrincipal.Amount).First(),RecommendationStrategy.CashFlowRelease=>debts.OrderBy(x=>x.RemainingMonths??int.MaxValue).ThenByDescending(x=>x.Debt.RegularPayment.Amount).First(),_=>debts.OrderByDescending(x=>(x.Debt.AnnualInterestRate??0)*x.Debt.RegularPayment.Amount/Math.Max(1,x.Debt.CurrentPrincipal.Amount)).First()};
 private static List<string> Blockers(RecommendationContext x){var b=new List<string>();if(x.Available.Amount<=0)b.Add("No hay efectivo disponible.");if(x.HasOverdueObligation)b.Add("Existe un pago vencido.");if(!x.HasNextIncome)b.Add("El siguiente ingreso no está registrado.");if(x.HasStaleBalances)b.Add("Existen saldos desactualizados.");if(x.HasReconciliationDifference)b.Add("Existe una diferencia de conciliación importante.");if(x.HasUncoveredEssentials)b.Add("Faltan gastos esenciales por cubrir.");return b;}
}
