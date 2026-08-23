using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Planning;
public sealed class ExpectedIncome
{
 private decimal _amount;private Currency _currency;public Guid Id{get;private set;}public Guid UserId{get;private set;}public string Name{get;private set;}public Money Amount=>new(_amount,_currency);public DateOnly ExpectedDate{get;private set;}public Frequency Frequency{get;private set;}public bool IsConfirmed{get;private set;}public Guid? DestinationFinancialAccountId{get;private set;}
 public ExpectedIncome(Guid userId,string name,Money amount,DateOnly date,Frequency frequency,bool confirmed,Guid? accountId){Id=Guid.NewGuid();UserId=userId;Name=name.Trim();_amount=amount.Amount;_currency=amount.Currency;ExpectedDate=date;Frequency=frequency;IsConfirmed=confirmed;DestinationFinancialAccountId=accountId;}
 private ExpectedIncome(){Name=string.Empty;}
}
