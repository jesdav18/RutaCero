using RutaCero.Domain.Common;
namespace RutaCero.Domain.Debts;
public sealed record CreditCardSchedule(DateOnly ClosingDate,DateOnly DueDate,bool IsEstimated);
public sealed class CreditCardScheduleService
{
 public CreditCardSchedule Calculate(DateOnly today,int closingDay,int dueDay,DateOnly? statementClosing=null,DateOnly? statementDue=null)
 {if(closingDay is<1 or>31||dueDay is<1 or>31)throw new DomainException("Card days are invalid.");if(statementClosing is not null&&statementDue is not null)return new(statementClosing.Value,statementDue.Value,false);var close=Date(today.Year,today.Month,closingDay);if(close<today)close=Date(today.AddMonths(1).Year,today.AddMonths(1).Month,closingDay);var dueMonth=dueDay>closingDay?close:close.AddMonths(1);var due=Date(dueMonth.Year,dueMonth.Month,dueDay);return new(close,due,true);}
 private static DateOnly Date(int year,int month,int day)=>new(year,month,Math.Min(day,DateTime.DaysInMonth(year,month)));
}
