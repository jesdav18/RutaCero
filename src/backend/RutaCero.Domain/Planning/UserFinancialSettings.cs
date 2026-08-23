using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Planning;
public enum SafetyReserveMode { FixedAmount,EssentialExpenseDays,HighestOfBoth }
public enum RecommendationProfile { Conservative,Balanced,Aggressive }
public sealed class UserFinancialSettings
{
 public Guid UserId{get;private set;}public decimal SafetyReserveAmount{get;private set;}public SafetyReserveMode SafetyReserveMode{get;private set;}public int MinimumDaysOfEssentialExpenses{get;private set;}public RecommendationProfile DefaultRecommendationProfile{get;private set;}public string DefaultTimeZone{get;private set;}public Currency BaseCurrency{get;private set;}public bool AllowEstimatedBalancesInRecommendations{get;private set;}
 public UserFinancialSettings(Guid userId,decimal reserve,SafetyReserveMode mode,int days,RecommendationProfile profile,string timeZone,Currency baseCurrency,bool allowEstimated){UserId=userId;SafetyReserveAmount=Math.Max(0,reserve);SafetyReserveMode=mode;MinimumDaysOfEssentialExpenses=Math.Max(0,days);DefaultRecommendationProfile=profile;DefaultTimeZone=timeZone;BaseCurrency=baseCurrency;AllowEstimatedBalancesInRecommendations=allowEstimated;}
 public decimal CalculateReserve(decimal dailyEssentialExpense)=>SafetyReserveMode switch{SafetyReserveMode.FixedAmount=>SafetyReserveAmount,SafetyReserveMode.EssentialExpenseDays=>dailyEssentialExpense*MinimumDaysOfEssentialExpenses,_=>Math.Max(SafetyReserveAmount,dailyEssentialExpense*MinimumDaysOfEssentialExpenses)};
 private UserFinancialSettings(){DefaultTimeZone="America/Tegucigalpa";}
}
