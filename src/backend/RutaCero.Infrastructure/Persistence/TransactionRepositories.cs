using Microsoft.EntityFrameworkCore;
using RutaCero.Application.Transactions;
using RutaCero.Domain.Transactions;

namespace RutaCero.Infrastructure.Persistence;

public sealed class TransactionRepository(RutaCeroDbContext db):ITransactionRepository
{
    public async Task<IReadOnlyList<Transaction>> ListAsync(Guid userId,DateOnly? from,DateOnly? to,CancellationToken token)
    {
        var query=db.Transactions.AsNoTracking().Where(x=>x.UserId==userId);
        if(from is not null)query=query.Where(x=>x.TransactionDate>=from);
        if(to is not null)query=query.Where(x=>x.TransactionDate<=to);
        return await query.OrderByDescending(x=>x.TransactionDate).ThenByDescending(x=>x.CreatedAt).ToListAsync(token);
    }
    public async Task AddAsync(Transaction transaction,CancellationToken token)=>await db.Transactions.AddAsync(transaction,token);
    public Task<Transaction?> FindAsync(Guid id,Guid userId,CancellationToken token)=>db.Transactions.FirstOrDefaultAsync(x=>x.Id==id&&x.UserId==userId,token);
    public async Task<IReadOnlyList<Transaction>> ListTransferGroupAsync(Guid groupId,Guid userId,CancellationToken token)=>
        await db.Transactions.Where(x=>x.UserId==userId&&x.TransferGroupId==groupId).ToListAsync(token);
    public void RemoveRange(IEnumerable<Transaction> items)=>db.Transactions.RemoveRange(items);
}
public sealed class CategoryRepository(RutaCeroDbContext db):ICategoryRepository
{
    public async Task<IReadOnlyList<TransactionCategory>> ListAsync(Guid userId,CancellationToken token)=>
        await db.TransactionCategories.AsNoTracking().Where(x=>x.UserId==null||x.UserId==userId).OrderBy(x=>x.Name).ToListAsync(token);
}
public sealed class TransactionTypeSettingRepository(RutaCeroDbContext db):ITransactionTypeSettingRepository
{
    public async Task<IReadOnlyList<TransactionTypeSetting>> ListAsync(Guid userId,CancellationToken token)=>
        await db.TransactionTypeSettings.Where(x=>x.UserId==userId).ToListAsync(token);
    public Task<TransactionTypeSetting?> FindAsync(Guid userId,TransactionType code,CancellationToken token)=>
        db.TransactionTypeSettings.FindAsync([userId,code],token).AsTask();
    public async Task AddAsync(TransactionTypeSetting setting,CancellationToken token)=>
        await db.TransactionTypeSettings.AddAsync(setting,token);
}
