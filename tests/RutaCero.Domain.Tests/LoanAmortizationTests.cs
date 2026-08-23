using RutaCero.Domain.Debts;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Tests;
public sealed class LoanAmortizationTests
{
 [Fact]public void Calculates_payment_and_interest_with_decimal_money(){var x=new LoanAmortizationService().Calculate(new(100000,Currency.HNL),12,12);Assert.Equal(8884.88m,x.MonthlyPayment.Amount);Assert.Equal(6618.56m,x.TotalInterest.Amount);}
 [Fact]public void Estimates_remaining_term(){var months=new LoanAmortizationService().EstimateRemainingMonths(new(10000,Currency.HNL),new(1000,Currency.HNL),12);Assert.Equal(11,months);}
}
