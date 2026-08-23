using Microsoft.EntityFrameworkCore;using RutaCero.Application.Transactions;using RutaCero.Domain.Transactions;
namespace RutaCero.Infrastructure.Persistence;
public sealed class CategorizationRuleRepository(RutaCeroDbContext db):ICategorizationRuleRepository
{
 public async Task<IReadOnlyList<CategorizationRule>> ListAsync(Guid userId,CancellationToken token)=>await db.CategorizationRules.AsNoTracking().Where(x=>x.UserId==userId&&x.IsActive).OrderByDescending(x=>x.Priority).ToListAsync(token);
 public async Task AddAsync(CategorizationRule rule,CancellationToken token)=>await db.CategorizationRules.AddAsync(rule,token);
}
