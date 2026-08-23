using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Recommendations;

public sealed record AvailableCashInput(Money LiquidBalances, Money PendingObligations,
    Money EssentialBudget, Money SafetyReserve, Money UnclearedTransactions, Money AccountBuffers);
public sealed record AvailableCashResult(Money Available, Money Deficit);

public sealed class AvailableCashService
{
    public AvailableCashResult Calculate(AvailableCashInput input)
    {
        var raw = input.LiquidBalances - input.PendingObligations - input.EssentialBudget
            - input.SafetyReserve - input.UnclearedTransactions - input.AccountBuffers;
        return raw.Amount >= 0
            ? new(raw, Money.Zero(raw.Currency))
            : new(Money.Zero(raw.Currency), new(-raw.Amount, raw.Currency));
    }
}
