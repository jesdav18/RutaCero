using RutaCero.Application.Transactions;
using RutaCero.Domain.Transactions;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Application.Dashboard;

public sealed record ExpenseCategoryDto(string Category,decimal CurrentAmount,decimal PreviousAmount,decimal Percentage,decimal? ChangePercentage);
public sealed record ExpenseCurrencyDto(Currency Currency,decimal CurrentTotal,decimal PreviousTotal,decimal? ChangePercentage,IReadOnlyList<ExpenseCategoryDto> Categories);
public sealed record ExpenseAnalyticsDto(int Year,int Month,IReadOnlyList<ExpenseCurrencyDto> Currencies);

public sealed class ExpenseAnalyticsService(ITransactionRepository transactions,ICategoryRepository categories)
{
    public async Task<ExpenseAnalyticsDto> GetAsync(Guid userId,int year,int month,CancellationToken token)
    {
        var start=new DateOnly(year,month,1);var end=start.AddMonths(1).AddDays(-1);
        var previousStart=start.AddMonths(-1);var items=await transactions.ListAsync(userId,previousStart,end,token);
        var names=(await categories.ListAsync(userId,token)).ToDictionary(x=>x.Id,x=>x.Name);
        var values=new[]{Currency.HNL,Currency.USD}.Select(x=>Build(x,items,names,start,end,previousStart)).ToList();
        return new(year,month,values);
    }

    private static ExpenseCurrencyDto Build(Currency currency,IReadOnlyList<Transaction> items,IReadOnlyDictionary<Guid,string> names,DateOnly start,DateOnly end,DateOnly previousStart)
    {
        var expenses=items.Where(x=>x.CountsAsExpense&&x.Amount.Currency==currency).ToList();
        var current=Group(expenses.Where(x=>x.TransactionDate>=start&&x.TransactionDate<=end),names);
        var previous=Group(expenses.Where(x=>x.TransactionDate>=previousStart&&x.TransactionDate<start),names);
        var currentTotal=current.Values.Sum();var previousTotal=previous.Values.Sum();
        var rows=current.Keys.Union(previous.Keys).Select(name=>new ExpenseCategoryDto(name,current.GetValueOrDefault(name),previous.GetValueOrDefault(name),currentTotal==0?0:decimal.Round(current.GetValueOrDefault(name)*100/currentTotal,1),Change(current.GetValueOrDefault(name),previous.GetValueOrDefault(name)))).OrderByDescending(x=>x.CurrentAmount).ToList();
        return new(currency,currentTotal,previousTotal,Change(currentTotal,previousTotal),rows);
    }

    private static Dictionary<string,decimal> Group(IEnumerable<Transaction> items,IReadOnlyDictionary<Guid,string> names)=>items.GroupBy(x=>Label(x,names)).ToDictionary(x=>x.Key,x=>x.Sum(y=>y.Amount.Amount));
    private static string Label(Transaction item,IReadOnlyDictionary<Guid,string> names)=>item.CategoryId is Guid id&&names.TryGetValue(id,out var name)?name:item.Type switch{TransactionType.Interest=>"Intereses",TransactionType.Fee=>"Comisiones",_=>"Sin clasificar"};
    private static decimal? Change(decimal current,decimal previous)=>previous==0?null:decimal.Round((current-previous)*100/previous,1);
}
