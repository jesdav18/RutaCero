using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Accounts;

public sealed class BalanceSnapshot
{
    private decimal _amount;
    private Currency _currency;
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid FinancialAccountId { get; private set; }
    public Money Balance => new(_amount, _currency);
    public DateOnly SnapshotDate { get; private set; }
    public BalanceSource Source { get; private set; }
    public DataConfidence Confidence { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public BalanceSnapshot(Guid userId, Guid accountId, Money balance, DateOnly date,
        BalanceSource source, DataConfidence confidence, DateTimeOffset createdAt)
    {
        Id=Guid.NewGuid(); UserId=userId; FinancialAccountId=accountId; _amount=balance.Amount;
        _currency=balance.Currency; SnapshotDate=date; Source=source; Confidence=confidence;
        CreatedAt=createdAt.ToUniversalTime();
    }
    private BalanceSnapshot(){ }
}
