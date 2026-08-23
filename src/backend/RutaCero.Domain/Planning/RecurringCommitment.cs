using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Planning;
public sealed class RecurringCommitment
{
 private decimal _amount;private Currency _currency;public Guid Id{get;private set;}public Guid UserId{get;private set;}public string Name{get;private set;}public Guid? CategoryId{get;private set;}public Money Amount=>new(_amount,_currency);public Frequency Frequency{get;private set;}public DateOnly NextDueDate{get;private set;}public DateOnly? EndDate{get;private set;}public bool IsEssential{get;private set;}public bool IsActive{get;private set;}=true;public Guid? SourceFinancialAccountId{get;private set;}public Guid? DebtId{get;private set;}
 public RecurringCommitment(Guid userId,string name,Guid? categoryId,Money amount,Frequency frequency,DateOnly due,DateOnly? end,bool essential,Guid? accountId,Guid? debtId){Id=Guid.NewGuid();UserId=userId;Name=name.Trim();CategoryId=categoryId;_amount=amount.Amount;_currency=amount.Currency;Frequency=frequency;NextDueDate=due;EndDate=end;IsEssential=essential;SourceFinancialAccountId=accountId;DebtId=debtId;}
 private RecurringCommitment(){Name=string.Empty;}
}
