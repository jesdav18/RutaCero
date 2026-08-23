using RutaCero.Domain.ExchangeRates;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Tests;
public sealed class ExchangeRateTests
{
 [Fact]public void Converts_only_with_explicit_matching_rate(){var rate=new ExchangeRate(Guid.NewGuid(),Currency.USD,Currency.HNL,24.75m,new DateOnly(2026,8,19),"Manual",DateTimeOffset.UtcNow);var result=rate.Convert(new(10,Currency.USD));Assert.Equal(Currency.HNL,result.Currency);Assert.Equal(247.50m,result.Amount);}
}
