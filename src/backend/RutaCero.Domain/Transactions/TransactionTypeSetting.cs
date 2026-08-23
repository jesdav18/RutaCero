namespace RutaCero.Domain.Transactions;

public enum TransactionEffect { Positive, Negative, Neutral }

public sealed class TransactionTypeSetting
{
    public Guid UserId { get; private set; }
    public TransactionType Code { get; private set; }
    public string Label { get; private set; }
    public TransactionEffect Effect { get; private set; }

    public TransactionTypeSetting(Guid userId, TransactionType code, string label, TransactionEffect effect)
    { UserId=userId;Code=code;Label=label.Trim();Effect=effect; }
    public void Update(string label,TransactionEffect effect){Label=label.Trim();Effect=effect;}
    private TransactionTypeSetting(){Label=string.Empty;}
}
