using RutaCero.Domain.ExchangeRates;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Application.ExchangeRates;
public sealed record ExchangeRateDto(Guid Id,Currency FromCurrency,Currency ToCurrency,decimal Rate,DateOnly EffectiveDate,string Source);
public sealed record CreateExchangeRateCommand(Currency FromCurrency,Currency ToCurrency,decimal Rate,DateOnly EffectiveDate,string Source);
public interface IExchangeRateRepository{Task<IReadOnlyList<ExchangeRate>> ListAsync(Guid userId,CancellationToken t);Task AddAsync(ExchangeRate rate,CancellationToken t);}
public sealed class ExchangeRateService(IExchangeRateRepository repository,IUnitOfWork unitOfWork)
{
 public async Task<IReadOnlyList<ExchangeRateDto>> ListAsync(Guid userId,CancellationToken t)=>(await repository.ListAsync(userId,t)).Select(Map).ToList();
 public async Task<ExchangeRateDto> CreateAsync(Guid userId,CreateExchangeRateCommand c,CancellationToken t){var x=new ExchangeRate(userId,c.FromCurrency,c.ToCurrency,c.Rate,c.EffectiveDate,c.Source,DateTimeOffset.UtcNow);await repository.AddAsync(x,t);await unitOfWork.SaveChangesAsync(t);return Map(x);}
 private static ExchangeRateDto Map(ExchangeRate x)=>new(x.Id,x.FromCurrency,x.ToCurrency,x.Rate,x.EffectiveDate,x.Source);
}
