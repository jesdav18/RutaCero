using RutaCero.Application.Accounts;using RutaCero.Application.Debts;using RutaCero.Application.Obligations;
using RutaCero.Application.Planning;using RutaCero.Domain.Accounts;using RutaCero.Domain.Obligations;
using RutaCero.Domain.Recommendations;using RutaCero.Domain.ValueObjects;
using RutaCero.Domain.Debts;
using RutaCero.Domain.Planning;
namespace RutaCero.Application.Recommendations;
public sealed class RecommendationApplicationService(IFinancialAccountRepository accounts,IDebtRepository debts,
 IObligationRepository obligations,IPlanningRepository planning)
{
 public async Task<IReadOnlyList<RecommendationDto>> GetAsync(Guid userId,RecommendationStrategy strategy,CancellationToken token)
 {
  var accountItems=await accounts.ListAsync(userId,token);var debtItems=await debts.ListAsync(userId,token);
  var obligationItems=await obligations.ListAsync(userId,null,null,token);var incomes=await planning.ListIncomesAsync(userId,token);
  var commitments=await planning.ListCommitmentsAsync(userId,token);var settings=await planning.GetSettingsAsync(userId,token)
   ??new(userId,0,SafetyReserveMode.FixedAmount,30,RecommendationProfile.Balanced,"America/Tegucigalpa",Currency.HNL,false);
  var today=DateOnly.FromDateTime(DateTime.UtcNow);foreach(var x in obligationItems)x.RefreshStatus(today);
  return new[]{Currency.HNL,Currency.USD}.Select(c=>Build(c,strategy,settings,accountItems,debtItems,obligationItems,incomes,commitments,today)).ToList();
 }
 private static RecommendationDto Build(Currency currency,RecommendationStrategy strategy,UserFinancialSettings settings,
  IReadOnlyList<FinancialAccount> accounts,IReadOnlyList<Debt> debts,IReadOnlyList<PaymentObligation> obligations,
  IReadOnlyList<ExpectedIncome> incomes,IReadOnlyList<RecurringCommitment> commitments,DateOnly today)
 {
  var liquid=accounts.Where(x=>x.IsIncludedInAvailableCash&&x.CurrentBalance.Currency==currency&&x.Type is AccountType.CheckingAccount or AccountType.SavingsAccount or AccountType.Cash).Sum(x=>x.CurrentBalance.Amount);
  var pending=obligations.Where(x=>x.Currency==currency&&x.Status is not(PaymentStatus.Paid or PaymentStatus.Cancelled)).Sum(x=>(x.ExpectedAmount??x.MinimumAmount??Money.Zero(currency)).Amount-x.PaidAmount.Amount);
  var essential=commitments.Where(x=>x.IsActive&&x.IsEssential&&x.Amount.Currency==currency).Sum(x=>x.Amount.Amount);
  var buffers=accounts.Where(x=>x.IsIncludedInAvailableCash&&x.MinimumBuffer.Currency==currency).Sum(x=>x.MinimumBuffer.Amount);
  var reserve=currency==settings.BaseCurrency?settings.CalculateReserve(essential/30):0;
  var available=new AvailableCashService().Calculate(new(new(liquid,currency),new(pending,currency),new(essential,currency),new(reserve,currency),new(0,currency),new(buffers,currency)));
  var context=new RecommendationContext(available.Available,debts.Where(x=>x.CurrentPrincipal.Currency==currency).Select(x=>new RecommendationDebt(x,null)).ToList(),
   obligations.Any(x=>x.Currency==currency&&x.Status==PaymentStatus.Overdue),incomes.Any(x=>x.Amount.Currency==currency&&x.ExpectedDate>=today),
   accounts.Any(x=>x.CurrentBalance.Currency==currency&&x.CurrentBalanceDate<today.AddDays(-7)),false,available.Deficit.Amount>0);
  var result=new CapitalPaymentRecommendationService().Recommend(context,strategy,settings.DefaultRecommendationProfile);
  return new(currency,liquid,pending,essential,reserve,buffers,available.Available.Amount,available.Deficit.Amount,result.Recommended.Amount,
   result.TargetDebtId,strategy,settings.DefaultRecommendationProfile,result.Explanation,result.Remaining.Amount,result.Confidence,result.Warnings,result.Blockers,result.EstimatedInterestSavings.Amount,result.EstimatedMonthsSaved);
 }
}
