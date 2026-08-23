using Microsoft.EntityFrameworkCore;using RutaCero.Application.ExchangeRates;using RutaCero.Domain.ExchangeRates;
namespace RutaCero.Infrastructure.Persistence;
public sealed class ExchangeRateRepository(RutaCeroDbContext db):IExchangeRateRepository
{
 public async Task<IReadOnlyList<ExchangeRate>> ListAsync(Guid userId,CancellationToken t)=>await db.ExchangeRates.AsNoTracking().Where(x=>x.UserId==userId).OrderByDescending(x=>x.EffectiveDate).ToListAsync(t);
 public async Task AddAsync(ExchangeRate rate,CancellationToken t)=>await db.ExchangeRates.AddAsync(rate,t);
}
