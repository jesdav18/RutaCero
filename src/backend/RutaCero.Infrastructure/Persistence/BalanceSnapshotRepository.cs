using Microsoft.EntityFrameworkCore;
using RutaCero.Application.Accounts;
using RutaCero.Domain.Accounts;

namespace RutaCero.Infrastructure.Persistence;

public sealed class BalanceSnapshotRepository(RutaCeroDbContext db):IBalanceSnapshotRepository
{
    public async Task<IReadOnlyList<BalanceSnapshot>> ListAsync(Guid accountId,Guid userId,CancellationToken token)=>
        await db.BalanceSnapshots.AsNoTracking().Where(x=>x.FinancialAccountId==accountId&&x.UserId==userId)
            .OrderByDescending(x=>x.SnapshotDate).ToListAsync(token);
    public async Task AddAsync(BalanceSnapshot snapshot,CancellationToken token)=>await db.BalanceSnapshots.AddAsync(snapshot,token);
}
