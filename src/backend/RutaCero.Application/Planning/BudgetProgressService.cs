using RutaCero.Application.Transactions;
namespace RutaCero.Application.Planning;
public sealed record BudgetProgressDto(Guid CategoryId,int Year,int Month,decimal Budgeted,decimal Consumed,decimal Remaining,decimal PercentageUsed,decimal Projected,string Currency);
public sealed class BudgetProgressService(IPlanningRepository planning,ITransactionRepository transactions)
{
 public async Task<IReadOnlyList<BudgetProgressDto>> GetAsync(Guid userId,int year,int month,CancellationToken token)
 {var budgets=await planning.ListBudgetsAsync(userId,year,month,token);var start=new DateOnly(year,month,1);var end=start.AddMonths(1).AddDays(-1);var today=DateOnly.FromDateTime(DateTime.UtcNow);var through=today<end&&today>=start?today:end;var items=await transactions.ListAsync(userId,start,through,token);var elapsed=Math.Max(1,(through.ToDateTime(TimeOnly.MinValue)-start.ToDateTime(TimeOnly.MinValue)).Days+1);var days=DateTime.DaysInMonth(year,month);return budgets.Select(x=>{var consumed=items.Where(t=>t.CategoryId==x.CategoryId&&t.Amount.Currency==x.Amount.Currency&&t.CountsAsExpense).Sum(t=>t.Amount.Amount);var remaining=Math.Max(0,x.Amount.Amount-consumed);var percent=x.Amount.Amount==0?0:decimal.Round(consumed/x.Amount.Amount*100,2);var projected=decimal.Round(consumed/elapsed*days,2);return new BudgetProgressDto(x.CategoryId,year,month,x.Amount.Amount,consumed,remaining,percent,projected,x.Amount.Currency.ToString());}).ToList();}
}
