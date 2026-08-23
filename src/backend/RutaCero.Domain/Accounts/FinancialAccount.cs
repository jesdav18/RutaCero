using RutaCero.Domain.ValueObjects;
using RutaCero.Domain.Common;

namespace RutaCero.Domain.Accounts;

public enum AccountType { CheckingAccount, SavingsAccount, Cash, CreditCard, Mortgage, PersonalLoan, ExtraFinancing, OtherAsset, OtherLiability }
public enum BalanceSource { Manual, Statement, Calculated }
public enum DataConfidence { Low, Medium, High }

public sealed class FinancialAccount
{
    private decimal _currentBalanceAmount;
    private Currency _currency;
    private decimal _minimumBufferAmount;
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string InstitutionName { get; private set; }
    public string DisplayName { get; private set; }
    public AccountReference Reference { get; private set; }
    public AccountType Type { get; private set; }
    public Money CurrentBalance => new(_currentBalanceAmount, _currency);
    public DateOnly CurrentBalanceDate { get; private set; }
    public BalanceSource BalanceSource { get; private set; }
    public DataConfidence BalanceConfidence { get; private set; }
    public Money MinimumBuffer => new(_minimumBufferAmount, _currency);
    public bool IsIncludedInAvailableCash { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public void ConfirmBalance(Money balance, DateOnly date,BalanceSource source,DataConfidence confidence)
    {
        if(balance.Currency!=_currency) throw new DomainException("Balance currency cannot change.");
        _currentBalanceAmount=balance.Amount;CurrentBalanceDate=date;BalanceSource=source;BalanceConfidence=confidence;UpdatedAt=DateTimeOffset.UtcNow;
    }
    public void UpdateDetails(string institution,string name,AccountReference reference,Money minimumBuffer,bool included)
    {
        if(minimumBuffer.Currency!=_currency)throw new DomainException("Buffer currency cannot change.");
        InstitutionName=institution.Trim();DisplayName=name.Trim();Reference=reference;_minimumBufferAmount=Math.Max(0,minimumBuffer.Amount);IsIncludedInAvailableCash=included;
    }

    public FinancialAccount(Guid userId, string institution, string name, AccountReference reference,
        AccountType type, Money balance, DateOnly balanceDate, Money minimumBuffer, bool included,BalanceSource source=BalanceSource.Manual,DataConfidence confidence=DataConfidence.High)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        InstitutionName = institution.Trim();
        DisplayName = name.Trim();
        Reference = reference;
        Type = type;
        _currentBalanceAmount = balance.Amount;
        _currency = balance.Currency;
        CurrentBalanceDate = balanceDate;
        BalanceSource=source;BalanceConfidence=confidence;
        _minimumBufferAmount = minimumBuffer.Amount;
        IsIncludedInAvailableCash = included;
        CreatedAt=UpdatedAt=DateTimeOffset.UtcNow;
    }

    private FinancialAccount() { InstitutionName = DisplayName = string.Empty; }
}
