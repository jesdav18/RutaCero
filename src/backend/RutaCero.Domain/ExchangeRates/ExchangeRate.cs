using RutaCero.Domain.Common;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.ExchangeRates;
public sealed class ExchangeRate
{
 public Guid Id{get;private set;}public Guid UserId{get;private set;}public Currency FromCurrency{get;private set;}public Currency ToCurrency{get;private set;}public decimal Rate{get;private set;}public DateOnly EffectiveDate{get;private set;}public string Source{get;private set;}public DateTimeOffset CreatedAt{get;private set;}
 public ExchangeRate(Guid userId,Currency from,Currency to,decimal rate,DateOnly date,string source,DateTimeOffset created){if(from==to||rate<=0)throw new DomainException("Exchange rate is invalid.");Id=Guid.NewGuid();UserId=userId;FromCurrency=from;ToCurrency=to;Rate=decimal.Round(rate,8);EffectiveDate=date;Source=source.Trim();CreatedAt=created.ToUniversalTime();}
 public Money Convert(Money money){if(money.Currency!=FromCurrency)throw new DomainException("Exchange rate source currency does not match.");return new(money.Amount*Rate,ToCurrency);}
 private ExchangeRate(){Source=string.Empty;}
}
