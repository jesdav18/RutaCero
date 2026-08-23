using Microsoft.EntityFrameworkCore;
using RutaCero.Application.Planning;
using RutaCero.Domain.Planning;
namespace RutaCero.Infrastructure.Persistence;
public sealed class PlanningRepository(RutaCeroDbContext db):IPlanningRepository
{
 public async Task<IReadOnlyList<ExpectedIncome>> ListIncomesAsync(Guid u,CancellationToken t)=>await db.ExpectedIncomes.AsNoTracking().Where(x=>x.UserId==u).OrderBy(x=>x.ExpectedDate).ToListAsync(t);
 public async Task AddIncomeAsync(ExpectedIncome x,CancellationToken t)=>await db.ExpectedIncomes.AddAsync(x,t);
 public async Task<IReadOnlyList<RecurringCommitment>> ListCommitmentsAsync(Guid u,CancellationToken t)=>await db.RecurringCommitments.AsNoTracking().Where(x=>x.UserId==u&&x.IsActive).OrderBy(x=>x.NextDueDate).ToListAsync(t);
 public async Task AddCommitmentAsync(RecurringCommitment x,CancellationToken t)=>await db.RecurringCommitments.AddAsync(x,t);
 public async Task<IReadOnlyList<MonthlyBudget>> ListBudgetsAsync(Guid u,int y,int m,CancellationToken t)=>await db.MonthlyBudgets.AsNoTracking().Where(x=>x.UserId==u&&x.Year==y&&x.Month==m).ToListAsync(t);
 public async Task UpsertBudgetAsync(MonthlyBudget x,CancellationToken t){var old=await db.MonthlyBudgets.SingleOrDefaultAsync(v=>v.UserId==x.UserId&&v.CategoryId==x.CategoryId&&v.Year==x.Year&&v.Month==x.Month,t);if(old is not null)db.MonthlyBudgets.Remove(old);await db.MonthlyBudgets.AddAsync(x,t);}
 public Task<UserFinancialSettings?> GetSettingsAsync(Guid u,CancellationToken t)=>db.UserFinancialSettings.AsNoTracking().SingleOrDefaultAsync(x=>x.UserId==u,t);
 public async Task UpsertSettingsAsync(UserFinancialSettings x,CancellationToken t){var old=await db.UserFinancialSettings.SingleOrDefaultAsync(v=>v.UserId==x.UserId,t);if(old is not null)db.UserFinancialSettings.Remove(old);await db.UserFinancialSettings.AddAsync(x,t);}
}
