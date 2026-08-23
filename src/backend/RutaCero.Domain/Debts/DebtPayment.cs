using RutaCero.Domain.Common;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Debts;

public enum PaymentType { RegularInstallment, MinimumPayment, FullStatementPayment, ExtraPrincipalPayment, Settlement, Adjustment }

public sealed class DebtPayment
{
    private decimal _totalAmount;
    private decimal? _principalAmount;
    private Currency _currency;
    public Guid Id { get; } = Guid.NewGuid();
    public Guid DebtId { get; }
    public DateOnly PaymentDate { get; }
    public Money TotalAmount => new(_totalAmount, _currency);
    public Money? PrincipalAmount => _principalAmount is null ? null : new(_principalAmount.Value, _currency);
    public PaymentType Type { get; }
    public bool IsAllocationConfirmed { get; }

    public DebtPayment(Guid debtId, DateOnly date, Money total, Money? principal,
        PaymentType type, bool allocationConfirmed)
    {
        if (total.Amount <= 0 || principal?.Amount > total.Amount)
            throw new DomainException("Payment allocation is invalid.");
        if (principal is not null && principal.Value.Currency != total.Currency)
            throw new DomainException("Payment currencies must match.");
        DebtId = debtId; PaymentDate = date; _totalAmount = total.Amount; _currency = total.Currency;
        _principalAmount = principal?.Amount; Type = type; IsAllocationConfirmed = allocationConfirmed;
    }

    public void ApplyTo(Debt debt)
    {
        if (DebtId != debt.Id) throw new DomainException("Payment belongs to another debt.");
        if (IsAllocationConfirmed && PrincipalAmount is not null)
            debt.ApplyConfirmedPrincipal(PrincipalAmount.Value);
    }

    private DebtPayment() { }
}
