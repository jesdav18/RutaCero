using RutaCero.Domain.Common;

namespace RutaCero.Domain.ValueObjects;

public readonly record struct DateRange
{
    public DateOnly Start { get; }
    public DateOnly End { get; }

    public DateRange(DateOnly start, DateOnly end)
    {
        if (end < start)
            throw new DomainException("End date cannot precede start date.");
        Start = start;
        End = end;
    }
}
