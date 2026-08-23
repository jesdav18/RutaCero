using RutaCero.Domain.Debts;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Tests;

public sealed class DebtPaymentTests
{
    [Fact]
    public void Only_confirmed_principal_reduces_debt()
    {
        var debt = CreateDebt();
        new DebtPayment(debt.Id, DateOnly.FromDateTime(DateTime.Today), new(500, Currency.HNL),
            new(300, Currency.HNL), PaymentType.RegularInstallment, true).ApplyTo(debt);
        Assert.Equal(9700, debt.CurrentPrincipal.Amount);
    }

    [Fact]
    public void Unknown_allocation_does_not_reduce_debt()
    {
        var debt = CreateDebt();
        new DebtPayment(debt.Id, DateOnly.FromDateTime(DateTime.Today), new(500, Currency.HNL),
            null, PaymentType.RegularInstallment, false).ApplyTo(debt);
        Assert.Equal(10000, debt.CurrentPrincipal.Amount);
    }

    private static Debt CreateDebt() => new(Guid.NewGuid(), "Banco", "Préstamo", DebtType.PersonalLoan,
        new(10000, Currency.HNL), 18, new(500, Currency.HNL), true, false);
}
