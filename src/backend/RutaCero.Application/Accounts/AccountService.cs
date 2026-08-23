using RutaCero.Domain.Accounts;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Application.Accounts;

public sealed class AccountService(IFinancialAccountRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<AccountDto>> ListAsync(Guid userId, CancellationToken token)
    {
        var items = await repository.ListAsync(userId, token);
        return items.Select(Map).ToList();
    }

    public async Task<AccountDto> CreateAsync(Guid userId, CreateAccountCommand command, CancellationToken token)
    {
        var account = new FinancialAccount(userId, command.InstitutionName, command.DisplayName,
            new AccountReference(command.Reference), command.Type, new Money(command.Balance, command.Currency),
            command.BalanceDate, new Money(command.MinimumBuffer, command.Currency), command.IsIncludedInAvailableCash);
        await repository.AddAsync(account, token);
        await unitOfWork.SaveChangesAsync(token);
        return Map(account);
    }
    public async Task<Common.Result<AccountDto>> UpdateAsync(Guid userId,Guid id,UpdateAccountCommand command,CancellationToken token)
    {
        var account=await repository.FindAsync(id,userId,token);if(account is null)return Common.Result<AccountDto>.Failure("La cuenta no existe.");
        account.UpdateDetails(command.InstitutionName,command.DisplayName,new AccountReference(command.Reference),new Money(command.MinimumBuffer,account.CurrentBalance.Currency),command.IsIncludedInAvailableCash);
        await unitOfWork.SaveChangesAsync(token);return Common.Result<AccountDto>.Success(Map(account));
    }

    private static AccountDto Map(FinancialAccount account) => new(account.Id, account.InstitutionName,
        account.DisplayName, account.Reference.Value, account.Type, account.CurrentBalance.Amount,
        account.CurrentBalance.Currency, account.CurrentBalanceDate, account.MinimumBuffer.Amount,
        account.IsIncludedInAvailableCash,account.BalanceSource,account.BalanceConfidence);
}
