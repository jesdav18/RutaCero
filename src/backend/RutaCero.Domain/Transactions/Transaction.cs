using RutaCero.Domain.Common;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Transactions;

public enum TransactionType { Income, Expense, Transfer, DebtPayment, Interest, Fee, Refund, Adjustment }
public enum TransferDirection { Outgoing, Incoming }

public sealed class Transaction
{
    private decimal _amount;
    private Currency _currency;
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid FinancialAccountId { get; private set; }
    public Guid? RelatedFinancialAccountId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public Guid? DebtId { get; private set; }
    public Guid? RecurringCommitmentId { get; private set; }
    public TransactionType Type { get; private set; }
    public Money Amount => new(_amount, _currency);
    public DateOnly TransactionDate { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? TransferGroupId { get; private set; }
    public TransferDirection? TransferDirection { get; private set; }

    public Transaction(Guid userId, Guid accountId, Guid? relatedAccountId, Guid? categoryId,
        TransactionType type, Money amount, DateOnly date, string description, DateTimeOffset createdAt,
        Guid? transferGroupId=null, TransferDirection? transferDirection=null,Guid? debtId=null,Guid? recurringCommitmentId=null)
    {
        if(amount.Amount<=0) throw new DomainException("Transaction amount must be positive.");
        Id=Guid.NewGuid(); UserId=userId; FinancialAccountId=accountId; RelatedFinancialAccountId=relatedAccountId;
        CategoryId=categoryId; Type=type; _amount=amount.Amount; _currency=amount.Currency;
        TransactionDate=date; Description=description.Trim(); CreatedAt=createdAt.ToUniversalTime();
        TransferGroupId=transferGroupId;TransferDirection=transferDirection;DebtId=debtId;RecurringCommitmentId=recurringCommitmentId;
    }
    public bool CountsAsExpense => Type is TransactionType.Expense or TransactionType.Interest or TransactionType.Fee;
    public void Update(Guid accountId, Guid? relatedAccountId, Guid? categoryId, TransactionType type,
        Money amount, DateOnly date, string description,Guid? recurringCommitmentId=null)
    {
        if(amount.Amount<=0) throw new DomainException("Transaction amount must be positive.");
        if(string.IsNullOrWhiteSpace(description)) throw new DomainException("Transaction description is required.");
        FinancialAccountId=accountId; RelatedFinancialAccountId=relatedAccountId; CategoryId=categoryId;
        Type=type; _amount=amount.Amount; _currency=amount.Currency; TransactionDate=date; Description=description.Trim();RecurringCommitmentId=recurringCommitmentId;
    }
    public void ConfigureTransfer(Guid? groupId,TransferDirection? direction){TransferGroupId=groupId;TransferDirection=direction;}
    public void LinkDebt(Guid debtId)
    {
        if(Type!=TransactionType.DebtPayment||DebtId is not null)throw new DomainException("Transaction cannot be linked to this debt.");
        DebtId=debtId;
    }
    private Transaction(){Description=string.Empty;}
}
