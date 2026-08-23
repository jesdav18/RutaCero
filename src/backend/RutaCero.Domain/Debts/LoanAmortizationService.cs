using RutaCero.Domain.Common;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Debts;
public sealed record AmortizationResult(Money MonthlyPayment,Money TotalInterest,int Months);
public sealed class LoanAmortizationService
{
 public AmortizationResult Calculate(Money principal,decimal annualRate,int months)
 {if(principal.Amount<=0||annualRate<0||months<=0)throw new DomainException("Amortization inputs are invalid.");var monthlyRate=annualRate/100/12;var factor=Pow(1+monthlyRate,months);var payment=monthlyRate==0?principal.Amount/months:principal.Amount*monthlyRate*factor/(factor-1);payment=decimal.Round(payment,2);return new(new(payment,principal.Currency),new(payment*months-principal.Amount,principal.Currency),months);}
 public int EstimateRemainingMonths(Money principal,Money payment,decimal annualRate)
 {if(principal.Currency!=payment.Currency||payment.Amount<=0)throw new DomainException("Payment is invalid.");var balance=principal.Amount;var rate=annualRate/100/12;var months=0;while(balance>0&&months<1200){var interest=balance*rate;if(payment.Amount<=interest)throw new DomainException("Payment does not cover interest.");balance-=payment.Amount-interest;months++;}return months;}
 private static decimal Pow(decimal value,int exponent){var result=1m;for(var i=0;i<exponent;i++)result*=value;return result;}
}
