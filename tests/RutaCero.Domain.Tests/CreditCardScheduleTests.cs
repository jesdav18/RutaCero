using RutaCero.Domain.Debts;
namespace RutaCero.Domain.Tests;
public sealed class CreditCardScheduleTests
{
 [Fact]public void Statement_dates_override_estimates(){var close=new DateOnly(2026,8,10);var due=new DateOnly(2026,9,2);var x=new CreditCardScheduleService().Calculate(new(2026,8,19),10,2,close,due);Assert.Equal(due,x.DueDate);Assert.False(x.IsEstimated);}
 [Fact]public void Estimates_due_date_after_closing(){var x=new CreditCardScheduleService().Calculate(new(2026,8,19),25,15);Assert.Equal(new DateOnly(2026,9,15),x.DueDate);Assert.True(x.IsEstimated);}
}
