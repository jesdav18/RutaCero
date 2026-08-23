using RutaCero.Domain.Common;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Tests;

public sealed class MoneyTests
{
    [Fact]
    public void Adds_matching_currencies() =>
        Assert.Equal(new Money(15, Currency.HNL), new Money(10, Currency.HNL) + new Money(5, Currency.HNL));

    [Fact]
    public void Rejects_mixed_currencies() =>
        Assert.Throws<DomainException>(() => new Money(1, Currency.HNL) + new Money(1, Currency.USD));
}
