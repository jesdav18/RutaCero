using RutaCero.Domain.Planning;
using RutaCero.Domain.ValueObjects;
namespace RutaCero.Application.Planning;
public sealed record ExpectedIncomeDto(Guid Id,string Name,decimal Amount,Currency Currency,DateOnly ExpectedDate,Frequency Frequency,bool IsConfirmed,Guid? DestinationFinancialAccountId);
public sealed record CreateExpectedIncomeCommand(string Name,decimal Amount,Currency Currency,DateOnly ExpectedDate,Frequency Frequency,bool IsConfirmed,Guid? DestinationFinancialAccountId);
public sealed record CommitmentDto(Guid Id,string Name,Guid? CategoryId,decimal Amount,Currency Currency,Frequency Frequency,DateOnly NextDueDate,DateOnly? EndDate,bool IsEssential,bool IsActive,Guid? SourceFinancialAccountId,Guid? DebtId);
public sealed record CreateCommitmentCommand(string Name,Guid? CategoryId,decimal Amount,Currency Currency,Frequency Frequency,DateOnly NextDueDate,DateOnly? EndDate,bool IsEssential,Guid? SourceFinancialAccountId,Guid? DebtId);
public sealed record BudgetDto(Guid Id,Guid CategoryId,int Year,int Month,decimal Amount,Currency Currency);
public sealed record CreateBudgetCommand(Guid CategoryId,int Year,int Month,decimal Amount,Currency Currency);
public sealed record SettingsDto(decimal SafetyReserveAmount,SafetyReserveMode SafetyReserveMode,int MinimumDaysOfEssentialExpenses,RecommendationProfile DefaultRecommendationProfile,string DefaultTimeZone,Currency BaseCurrency,bool AllowEstimatedBalancesInRecommendations);
public interface IPlanningRepository
{
 Task<IReadOnlyList<ExpectedIncome>> ListIncomesAsync(Guid userId,CancellationToken token);Task AddIncomeAsync(ExpectedIncome item,CancellationToken token);
 Task<ExpectedIncome?> FindIncomeAsync(Guid id,Guid userId,CancellationToken token);void RemoveIncome(ExpectedIncome item);
 Task<IReadOnlyList<RecurringCommitment>> ListCommitmentsAsync(Guid userId,CancellationToken token);Task AddCommitmentAsync(RecurringCommitment item,CancellationToken token);
 Task<RecurringCommitment?> FindCommitmentAsync(Guid id,Guid userId,CancellationToken token);
 Task<IReadOnlyList<MonthlyBudget>> ListBudgetsAsync(Guid userId,int year,int month,CancellationToken token);Task UpsertBudgetAsync(MonthlyBudget item,CancellationToken token);
 Task<UserFinancialSettings?> GetSettingsAsync(Guid userId,CancellationToken token);Task UpsertSettingsAsync(UserFinancialSettings item,CancellationToken token);
}
