using RutaCero.Domain.Accounts;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Application.Accounts;

public sealed record AccountDto(Guid Id, string InstitutionName, string DisplayName,
    string Reference, AccountType Type, decimal Balance, Currency Currency, DateOnly BalanceDate,
    decimal MinimumBuffer, bool IsIncludedInAvailableCash,BalanceSource BalanceSource,DataConfidence BalanceConfidence);
public sealed record CreateAccountCommand(string InstitutionName, string DisplayName,
    string Reference, AccountType Type, decimal Balance, Currency Currency, DateOnly BalanceDate,
    decimal MinimumBuffer, bool IsIncludedInAvailableCash);
public sealed record UpdateAccountCommand(string InstitutionName,string DisplayName,string Reference,decimal MinimumBuffer,bool IsIncludedInAvailableCash);

public interface IFinancialAccountRepository
{
    Task<IReadOnlyList<FinancialAccount>> ListAsync(Guid userId, CancellationToken cancellationToken);
    Task<FinancialAccount?> FindAsync(Guid id, Guid userId, CancellationToken cancellationToken);
    Task AddAsync(FinancialAccount account, CancellationToken cancellationToken);
}

public sealed record BalanceSnapshotDto(Guid Id,Guid FinancialAccountId,decimal Balance,Currency Currency,
    DateOnly SnapshotDate,BalanceSource Source,DataConfidence Confidence);
public sealed record CreateBalanceSnapshotCommand(decimal Balance,DateOnly SnapshotDate,
    BalanceSource Source,DataConfidence Confidence);
public interface IBalanceSnapshotRepository
{
    Task<IReadOnlyList<BalanceSnapshot>> ListAsync(Guid accountId,Guid userId,CancellationToken token);
    Task AddAsync(BalanceSnapshot snapshot,CancellationToken token);
}
