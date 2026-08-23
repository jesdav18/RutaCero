using RutaCero.Domain.Common;
using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Planning;
public sealed class MonthlyBudget
{
 private decimal _amount;private Currency _currency;public Guid Id{get;private set;}public Guid UserId{get;private set;}public Guid CategoryId{get;private set;}public int Year{get;private set;}public int Month{get;private set;}public Money Amount=>new(_amount,_currency);
 public MonthlyBudget(Guid userId,Guid categoryId,int year,int month,Money amount){if(month is<1 or>12||amount.Amount<0)throw new DomainException("Budget is invalid.");Id=Guid.NewGuid();UserId=userId;CategoryId=categoryId;Year=year;Month=month;_amount=amount.Amount;_currency=amount.Currency;}
 private MonthlyBudget(){ }
}
