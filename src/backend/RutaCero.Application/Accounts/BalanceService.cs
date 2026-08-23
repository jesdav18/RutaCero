using RutaCero.Application.Common;
using RutaCero.Domain.Accounts;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Application.Accounts;

public sealed class BalanceService(IFinancialAccountRepository accounts,IBalanceSnapshotRepository snapshots,IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<BalanceSnapshotDto>> ListAsync(Guid userId,Guid accountId,CancellationToken token)=>
        (await snapshots.ListAsync(accountId,userId,token)).Select(Map).ToList();
    public async Task<Result<BalanceSnapshotDto>> CreateAsync(Guid userId,Guid accountId,CreateBalanceSnapshotCommand command,CancellationToken token)
    {
        var account=await accounts.FindAsync(accountId,userId,token);
        if(account is null)return Result<BalanceSnapshotDto>.Failure("La cuenta no existe.");
        var money=new Money(command.Balance,account.CurrentBalance.Currency);
        var snapshot=new BalanceSnapshot(userId,accountId,money,command.SnapshotDate,command.Source,command.Confidence,DateTimeOffset.UtcNow);
        account.ConfirmBalance(money,command.SnapshotDate,command.Source,command.Confidence);await snapshots.AddAsync(snapshot,token);await unitOfWork.SaveChangesAsync(token);
        return Result<BalanceSnapshotDto>.Success(Map(snapshot));
    }
    private static BalanceSnapshotDto Map(BalanceSnapshot x)=>new(x.Id,x.FinancialAccountId,x.Balance.Amount,x.Balance.Currency,x.SnapshotDate,x.Source,x.Confidence);
}
