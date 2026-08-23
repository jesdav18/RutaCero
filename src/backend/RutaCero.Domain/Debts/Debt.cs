using RutaCero.Domain.Common;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Debts;

public enum DebtType { CreditCard, Mortgage, PersonalLoan, ExtraFinancing, Other }
public enum DebtStatus { Active, Paid, Settled, Closed }

public sealed class Debt
{
    private decimal _originalPrincipalAmount;
    private decimal _currentPrincipalAmount;
    private decimal _regularPaymentAmount;
    private Currency _currency;
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; }
    public string InstitutionName { get; private set; }
    public DebtType Type { get; private set; }
    public Money OriginalPrincipal => new(_originalPrincipalAmount, _currency);
    public Money CurrentPrincipal => new(_currentPrincipalAmount, _currency);
    public decimal? AnnualInterestRate { get; private set; }
    public Money RegularPayment => new(_regularPaymentAmount, _currency);
    public bool AllowsCapitalPrepayment { get; private set; }
    public bool HasPrepaymentPenalty { get; private set; }
    public DebtStatus Status { get; private set; } = DebtStatus.Active;
    public int? StatementClosingDay { get; private set; }
    public int? PaymentDueDay { get; private set; }
    public bool AutoGeneratePaymentObligations { get; private set; }

    public Debt(Guid userId, string institution, string name, DebtType type, Money principal, decimal? annualRate,
        Money regularPayment, bool allowsPrepayment, bool hasPenalty)
    {
        if (principal.Amount <= 0 || regularPayment.Amount < 0)
            throw new DomainException("Debt amounts are invalid.");
        if (principal.Currency != regularPayment.Currency)
            throw new DomainException("Debt currencies must match.");
        Id = Guid.NewGuid(); UserId = userId; InstitutionName = institution.Trim(); Name = name.Trim(); Type = type;
        _originalPrincipalAmount = _currentPrincipalAmount = principal.Amount; _currency = principal.Currency;
        AnnualInterestRate = annualRate; _regularPaymentAmount = regularPayment.Amount; AllowsCapitalPrepayment = allowsPrepayment;
        HasPrepaymentPenalty = hasPenalty;
    }

    public void ApplyConfirmedPrincipal(Money principal)
    {
        if (principal.Amount < 0 || principal.Currency != CurrentPrincipal.Currency)
            throw new DomainException("Principal allocation is invalid.");
        _currentPrincipalAmount = Math.Max(0, _currentPrincipalAmount - principal.Amount);
        if (CurrentPrincipal.Amount == 0) Status = DebtStatus.Paid;
    }
    public void ConfirmCurrentBalance(Money balance)
    {
        if(balance.Amount<0||balance.Currency!=CurrentPrincipal.Currency)throw new DomainException("Confirmed debt balance is invalid.");
        _currentPrincipalAmount=balance.Amount;Status=balance.Amount==0?DebtStatus.Paid:DebtStatus.Active;
    }

    public void UpdateDetails(string institution, string name, DebtType type, decimal? annualRate,
        Money regularPayment, bool allowsPrepayment, bool hasPenalty, int? statementClosingDay=null,
        int? paymentDueDay=null,bool autoGeneratePaymentObligations=false)
    {
        if (string.IsNullOrWhiteSpace(institution) || string.IsNullOrWhiteSpace(name))
            throw new DomainException("Debt institution and name are required.");
        if (regularPayment.Amount < 0 || regularPayment.Currency != CurrentPrincipal.Currency)
            throw new DomainException("Debt payment amount is invalid.");
        InstitutionName = institution.Trim(); Name = name.Trim(); Type = type; AnnualInterestRate = annualRate;
        _regularPaymentAmount = regularPayment.Amount; AllowsCapitalPrepayment = allowsPrepayment;
        HasPrepaymentPenalty = hasPenalty;
        ConfigureCreditCardSchedule(statementClosingDay,paymentDueDay,autoGeneratePaymentObligations);
    }
    public void ConfigureCreditCardSchedule(int? closingDay,int? dueDay,bool autoGenerate)
    {
        if((closingDay is not null&&(closingDay<1||closingDay>31))||(dueDay is not null&&(dueDay<1||dueDay>31)))
            throw new DomainException("Credit card schedule days must be between 1 and 31.");
        StatementClosingDay=closingDay;PaymentDueDay=dueDay;
        AutoGeneratePaymentObligations=autoGenerate&&dueDay is not null;
    }

    private Debt() { Name = InstitutionName = string.Empty; }
}
