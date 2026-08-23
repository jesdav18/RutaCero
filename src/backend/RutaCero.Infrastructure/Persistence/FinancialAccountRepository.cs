using Microsoft.EntityFrameworkCore;
using RutaCero.Application.Accounts;
using RutaCero.Domain.Accounts;

namespace RutaCero.Infrastructure.Persistence;

public sealed class FinancialAccountRepository(RutaCeroDbContext db) : IFinancialAccountRepository
{
    public async Task<IReadOnlyList<FinancialAccount>> ListAsync(Guid userId, CancellationToken token) =>
        await db.FinancialAccounts.AsNoTracking().Where(x => x.UserId == userId && x.IsActive).ToListAsync(token);
    public Task<FinancialAccount?> FindAsync(Guid id,Guid userId,CancellationToken token)=>
        db.FinancialAccounts.SingleOrDefaultAsync(x=>x.Id==id&&x.UserId==userId,token);

    public async Task AddAsync(FinancialAccount account, CancellationToken token) =>
        await db.FinancialAccounts.AddAsync(account, token);
}
