using RutaCero.Domain.Common;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Obligations;

public enum ObligationType { CreditCardMinimumPayment,CreditCardStatementPayment,LoanInstallment,MortgageInstallment,ExtraFinancingInstallment,RecurringCommitment,Other }
public enum PaymentStatus { Upcoming,DueSoon,DueToday,PartiallyPaid,Paid,Overdue,Cancelled }

public sealed class PaymentObligation
{
    private decimal? _expectedAmount;private decimal? _minimumAmount;private decimal _paidAmount;private Currency _currency;
    public Guid Id{get;private set;}public Guid UserId{get;private set;}public Guid? DebtId{get;private set;}
    public ObligationType Type{get;private set;}public string Description{get;private set;}public Currency Currency=>_currency;
    public Money? ExpectedAmount=>_expectedAmount is null?null:new(_expectedAmount.Value,_currency);
    public Money? MinimumAmount=>_minimumAmount is null?null:new(_minimumAmount.Value,_currency);
    public Money PaidAmount=>new(_paidAmount,_currency);public DateOnly DueDate{get;private set;}
    public bool IsAmountEstimated{get;private set;}public PaymentStatus Status{get;private set;}
    public DateTimeOffset? PaidAt{get;private set;}public DateTimeOffset CreatedAt{get;private set;}
    public PaymentObligation(Guid userId,Guid? debtId,ObligationType type,string description,Currency currency,
        decimal? expected,decimal? minimum,DateOnly dueDate,bool estimated,DateTimeOffset createdAt)
    {
        if(expected<0||minimum<0)throw new DomainException("Obligation amounts are invalid.");
        Id=Guid.NewGuid();UserId=userId;DebtId=debtId;Type=type;Description=description.Trim();_currency=currency;
        _expectedAmount=expected;_minimumAmount=minimum;DueDate=dueDate;IsAmountEstimated=estimated;CreatedAt=createdAt.ToUniversalTime();Status=PaymentStatus.Upcoming;
    }
    public void RefreshStatus(DateOnly today,int dueSoonDays=7)
    {
        if(Status is PaymentStatus.Paid or PaymentStatus.Cancelled)return;
        if(_paidAmount>0){Status=PaymentStatus.PartiallyPaid;return;}
        Status=DueDate<today?PaymentStatus.Overdue:DueDate==today?PaymentStatus.DueToday:DueDate<=today.AddDays(dueSoonDays)?PaymentStatus.DueSoon:PaymentStatus.Upcoming;
    }
    public void ApplyPayment(Money amount,DateTimeOffset paidAt)
    {
        if(amount.Currency!=_currency||amount.Amount<=0)throw new DomainException("Obligation payment is invalid.");
        _paidAmount+=amount.Amount;var target=_expectedAmount??_minimumAmount;
        if(target is not null&&_paidAmount>=target){Status=PaymentStatus.Paid;PaidAt=paidAt.ToUniversalTime();}else Status=PaymentStatus.PartiallyPaid;
    }
    private PaymentObligation(){Description=string.Empty;}
}
