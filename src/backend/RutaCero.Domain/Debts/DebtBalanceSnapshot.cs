using RutaCero.Domain.Common;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Debts;

public sealed class DebtBalanceSnapshot
{
    private decimal _balanceAmount;
    private Currency _currency;
    public Guid Id{get;private set;}=Guid.NewGuid();
    public Guid UserId{get;private set;}
    public Guid DebtId{get;private set;}
    public Guid? StatementImportId{get;private set;}
    public DateOnly StatementDate{get;private set;}
    public Money Balance=>new(_balanceAmount,_currency);
    public DateTimeOffset CreatedAt{get;private set;}
    public DebtBalanceSnapshot(Guid userId,Guid debtId,Guid? statementImportId,DateOnly statementDate,Money balance,DateTimeOffset createdAt)
    {if(balance.Amount<0)throw new DomainException("Debt balance cannot be negative.");UserId=userId;DebtId=debtId;StatementImportId=statementImportId;StatementDate=statementDate;_balanceAmount=balance.Amount;_currency=balance.Currency;CreatedAt=createdAt.ToUniversalTime();}
    private DebtBalanceSnapshot(){}
}
