using RutaCero.Domain.Accounts;using RutaCero.Domain.Planning;using RutaCero.Domain.Recommendations;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Application.Recommendations;
public sealed record RecommendationDto(Currency Currency,decimal LiquidBalances,decimal PendingObligations,
 decimal EssentialCommitments,decimal SafetyReserve,decimal AccountBuffers,decimal Available,decimal Deficit,
 decimal Recommended,Guid? TargetDebtId,RecommendationStrategy Strategy,RecommendationProfile Profile,
 string Explanation,decimal Remaining,DataConfidence Confidence,IReadOnlyList<string> Warnings,IReadOnlyList<string> Blockers,decimal EstimatedInterestSavings,int EstimatedMonthsSaved);
