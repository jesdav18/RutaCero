using Microsoft.EntityFrameworkCore;
using RutaCero.Application.Obligations;
using RutaCero.Domain.Obligations;

namespace RutaCero.Infrastructure.Persistence;

public sealed class ObligationRepository(RutaCeroDbContext db):IObligationRepository
{
    public async Task<IReadOnlyList<PaymentObligation>> ListAsync(Guid userId,DateOnly? from,DateOnly? to,CancellationToken token)
    {var query=db.PaymentObligations.AsNoTracking().Where(x=>x.UserId==userId);if(from is not null)query=query.Where(x=>x.DueDate>=from);if(to is not null)query=query.Where(x=>x.DueDate<=to);return await query.OrderBy(x=>x.DueDate).ToListAsync(token);}
    public Task<PaymentObligation?> FindAsync(Guid id,Guid userId,CancellationToken token)=>db.PaymentObligations.SingleOrDefaultAsync(x=>x.Id==id&&x.UserId==userId,token);
    public async Task AddAsync(PaymentObligation item,CancellationToken token)=>await db.PaymentObligations.AddAsync(item,token);
    public async Task<IReadOnlySet<ObligationScheduleKey>> ListScheduleKeysAsync(Guid userId,IReadOnlyCollection<Guid> debtIds,DateOnly from,DateOnly to,CancellationToken token)
    {
        var rows=await db.PaymentObligations.AsNoTracking()
            .Where(x=>x.UserId==userId&&x.DebtId.HasValue&&debtIds.Contains(x.DebtId.Value)&&x.DueDate>=from&&x.DueDate<=to)
            .Select(x=>new{x.DebtId,x.DueDate,x.Type}).ToListAsync(token);
        return rows.Select(x=>new ObligationScheduleKey(x.DebtId!.Value,x.DueDate,x.Type)).ToHashSet();
    }
}
