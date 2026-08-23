using RutaCero.Domain.Common;

namespace RutaCero.Domain.ValueObjects;

public readonly record struct Percentage
{
    public decimal Value { get; }

    public Percentage(decimal value)
    {
        if (value is < 0 or > 100)
            throw new DomainException("Percentage must be between zero and one hundred.");
        Value = value;
    }
}
