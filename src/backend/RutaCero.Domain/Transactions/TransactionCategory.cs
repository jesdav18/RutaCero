namespace RutaCero.Domain.Transactions;

public sealed class TransactionCategory
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public string Name { get; private set; }
    public bool IsIncome { get; private set; }
    public bool IsSystem { get; private set; }
    public TransactionCategory(Guid? userId,string name,bool isIncome,bool isSystem)
    { Id=Guid.NewGuid();UserId=userId;Name=name.Trim();IsIncome=isIncome;IsSystem=isSystem; }
    private TransactionCategory(){Name=string.Empty;}
}
