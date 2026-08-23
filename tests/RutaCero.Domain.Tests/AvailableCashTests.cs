using RutaCero.Domain.Recommendations;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Tests;

public sealed class AvailableCashTests
{
    [Fact]
    public void Protects_all_commitments()
    {
        var service = new AvailableCashService();
        var result = service.Calculate(new(new(10000, Currency.HNL), new(2000, Currency.HNL),
            new(1500, Currency.HNL), new(3000, Currency.HNL), new(500, Currency.HNL), new(250, Currency.HNL)));
        Assert.Equal(2750, result.Available.Amount);
        Assert.Equal(0, result.Deficit.Amount);
    }

    [Fact]
    public void Negative_available_cash_becomes_deficit()
    {
        var service = new AvailableCashService();
        var result = service.Calculate(new(new(100, Currency.USD), new(200, Currency.USD),
            new(0, Currency.USD), new(0, Currency.USD), new(0, Currency.USD), new(0, Currency.USD)));
        Assert.Equal(0, result.Available.Amount);
        Assert.Equal(100, result.Deficit.Amount);
    }
}
