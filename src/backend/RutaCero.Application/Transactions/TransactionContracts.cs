using RutaCero.Domain.Transactions;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Application.Transactions;

public sealed record TransactionDto(Guid Id,Guid FinancialAccountId,Guid? RelatedFinancialAccountId,
    Guid? CategoryId,TransactionType Type,decimal Amount,Currency Currency,DateOnly TransactionDate,string Description,
    Guid? TransferGroupId,TransferDirection? TransferDirection,Guid? DebtId,Guid? RecurringCommitmentId);
public sealed record CreateTransactionCommand(Guid FinancialAccountId,Guid? RelatedFinancialAccountId,
    Guid? CategoryId,TransactionType Type,decimal Amount,Currency Currency,DateOnly TransactionDate,string Description,
    decimal? RelatedAmount=null,Currency? RelatedCurrency=null,Guid? DebtId=null,decimal? PrincipalAmount=null,bool IsAllocationConfirmed=false,Guid? RecurringCommitmentId=null);
public sealed record UpdateTransactionCommand(Guid FinancialAccountId,Guid? RelatedFinancialAccountId,
    Guid? CategoryId,TransactionType Type,decimal Amount,Currency Currency,DateOnly TransactionDate,string Description,
    Guid? DebtId=null,decimal? PrincipalAmount=null,bool IsAllocationConfirmed=false,Guid? RecurringCommitmentId=null);
public sealed record CategoryDto(Guid Id,string Name,bool IsIncome,bool IsSystem);
public interface ITransactionRepository
{
    Task<IReadOnlyList<Transaction>> ListAsync(Guid userId,DateOnly? from,DateOnly? to,CancellationToken token);
    Task AddAsync(Transaction transaction,CancellationToken token);
    Task<Transaction?> FindAsync(Guid id,Guid userId,CancellationToken token);
    Task<IReadOnlyList<Transaction>> ListTransferGroupAsync(Guid groupId,Guid userId,CancellationToken token);
    void RemoveRange(IEnumerable<Transaction> items);
}
public interface ICategoryRepository
{
    Task<IReadOnlyList<TransactionCategory>> ListAsync(Guid userId,CancellationToken token);
}
