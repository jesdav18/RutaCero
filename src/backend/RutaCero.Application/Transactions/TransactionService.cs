using RutaCero.Application.Common;
using RutaCero.Application.Accounts;
using RutaCero.Domain.Transactions;
using RutaCero.Domain.Accounts;
using RutaCero.Domain.ValueObjects;
using RutaCero.Application.Debts;
using RutaCero.Domain.Debts;
using RutaCero.Application.Planning;

namespace RutaCero.Application.Transactions;

public sealed class TransactionService(ITransactionRepository transactions,IFinancialAccountRepository accounts,IUnitOfWork unitOfWork,IDebtRepository debts,IDebtPaymentRepository debtPayments,IPlanningRepository planning)
{
    public async Task<IReadOnlyList<TransactionDto>> ListAsync(Guid userId,DateOnly? from,DateOnly? to,CancellationToken token)=>
        (await transactions.ListAsync(userId,from,to,token)).Select(Map).ToList();
    public async Task<Result<TransactionDto>> CreateAsync(Guid userId,CreateTransactionCommand command,CancellationToken token)
    {
        var account=await accounts.FindAsync(command.FinancialAccountId,userId,token);
        if(account is null)return Result<TransactionDto>.Failure("La cuenta no existe.");
        FinancialAccount? relatedAccount=null;
        if(command.RelatedFinancialAccountId is Guid related&&(relatedAccount=await accounts.FindAsync(related,userId,token)) is null)
            return Result<TransactionDto>.Failure("La cuenta relacionada no existe.");
        if(command.RecurringCommitmentId is Guid commitmentId)
        {
            var commitment=await planning.FindCommitmentAsync(commitmentId,userId,token);
            if(commitment is null)return Result<TransactionDto>.Failure("El compromiso recurrente no existe.");
            if(commitment.Amount.Currency!=command.Currency)return Result<TransactionDto>.Failure("El movimiento y el compromiso deben usar la misma moneda.");
        }
        if(command.Type==TransactionType.Transfer)
        {
            if(relatedAccount is null||command.RelatedFinancialAccountId is not Guid destinationId||destinationId==command.FinancialAccountId)
                return Result<TransactionDto>.Failure("Selecciona una cuenta destino diferente.");
            if(command.RelatedAmount is null or <=0||command.RelatedCurrency is null)
                return Result<TransactionDto>.Failure("El monto y la moneda de destino son obligatorios.");
            var groupId=Guid.NewGuid();var createdAt=DateTimeOffset.UtcNow;
            var outgoing=new Transaction(userId,command.FinancialAccountId,command.RelatedFinancialAccountId,command.CategoryId,
                command.Type,new Money(command.Amount,command.Currency),command.TransactionDate,command.Description,createdAt,groupId,TransferDirection.Outgoing,recurringCommitmentId:command.RecurringCommitmentId);
            var incoming=new Transaction(userId,destinationId,command.FinancialAccountId,command.CategoryId,
                command.Type,new Money(command.RelatedAmount.Value,command.RelatedCurrency.Value),command.TransactionDate,command.Description,createdAt,groupId,TransferDirection.Incoming);
            await transactions.AddAsync(outgoing,token);await transactions.AddAsync(incoming,token);await unitOfWork.SaveChangesAsync(token);
            return Result<TransactionDto>.Success(Map(outgoing));
        }
        Debt? debt=null;
        if(command.Type==TransactionType.DebtPayment)
        {
            if(command.DebtId is not Guid debtId||(debt=await debts.FindAsync(debtId,userId,token)) is null)
                return Result<TransactionDto>.Failure("Selecciona la deuda que estás pagando.");
            if(debt.CurrentPrincipal.Currency!=command.Currency||account.CurrentBalance.Currency!=command.Currency)
                return Result<TransactionDto>.Failure("La cuenta, el movimiento y la deuda deben usar la misma moneda.");
            if(command.PrincipalAmount is null or <=0||!command.IsAllocationConfirmed)
                return Result<TransactionDto>.Failure("Confirma cuánto del pago corresponde a capital.");
            var debtPayment=new DebtPayment(debt.Id,command.TransactionDate,new Money(command.Amount,command.Currency),
                new Money(command.PrincipalAmount.Value,command.Currency),PaymentType.RegularInstallment,true);
            debtPayment.ApplyTo(debt);await debtPayments.AddAsync(debtPayment,token);
        }
        var item=new Transaction(userId,command.FinancialAccountId,command.RelatedFinancialAccountId,
            command.CategoryId,command.Type,new Money(command.Amount,command.Currency),
            command.TransactionDate,command.Description,DateTimeOffset.UtcNow,debtId:debt?.Id,recurringCommitmentId:command.RecurringCommitmentId);
        await transactions.AddAsync(item,token);await unitOfWork.SaveChangesAsync(token);
        return Result<TransactionDto>.Success(Map(item));
    }
    public async Task<Result<TransactionDto>> UpdateAsync(Guid userId,Guid id,UpdateTransactionCommand command,CancellationToken token)
    {
        var item=await transactions.FindAsync(id,userId,token);
        if(item is null)return Result<TransactionDto>.Failure("El movimiento no existe.");
        if(item.DebtId is not null)return Result<TransactionDto>.Failure("Los pagos ya vinculados deben corregirse desde Deudas.");
        if(await accounts.FindAsync(command.FinancialAccountId,userId,token) is null)
            return Result<TransactionDto>.Failure("La cuenta no existe.");
        if(command.RelatedFinancialAccountId is Guid related&&await accounts.FindAsync(related,userId,token) is null)
            return Result<TransactionDto>.Failure("La cuenta relacionada no existe.");
        if(command.RecurringCommitmentId is Guid commitmentId)
        {
            var commitment=await planning.FindCommitmentAsync(commitmentId,userId,token);
            if(commitment is null)return Result<TransactionDto>.Failure("El compromiso recurrente no existe.");
            if(commitment.Amount.Currency!=command.Currency)return Result<TransactionDto>.Failure("El movimiento y el compromiso deben usar la misma moneda.");
        }
        Debt? linkedDebt=null;
        if(command.Type==TransactionType.DebtPayment)
        {
            if(command.DebtId is not Guid debtId||(linkedDebt=await debts.FindAsync(debtId,userId,token)) is null)
                return Result<TransactionDto>.Failure("Selecciona la deuda que estás pagando.");
            if(linkedDebt.CurrentPrincipal.Currency!=command.Currency||command.PrincipalAmount is null or <=0||!command.IsAllocationConfirmed)
                return Result<TransactionDto>.Failure("Confirma la moneda y el capital aplicado a la deuda.");
            var payment=new DebtPayment(linkedDebt.Id,command.TransactionDate,new Money(command.Amount,command.Currency),
                new Money(command.PrincipalAmount.Value,command.Currency),PaymentType.RegularInstallment,true);
            payment.ApplyTo(linkedDebt);await debtPayments.AddAsync(payment,token);
        }
        item.Update(command.FinancialAccountId,command.RelatedFinancialAccountId,command.CategoryId,command.Type,
            new Money(command.Amount,command.Currency),command.TransactionDate,command.Description,command.RecurringCommitmentId);
        if(linkedDebt is not null)item.LinkDebt(linkedDebt.Id);
        await unitOfWork.SaveChangesAsync(token);
        return Result<TransactionDto>.Success(Map(item));
    }
    public async Task<Result<bool>> DeleteAsync(Guid userId,Guid id,CancellationToken token)
    {
        var item=await transactions.FindAsync(id,userId,token);
        if(item is null)return Result<bool>.Failure("El movimiento no existe.");
        if(item.DebtId is not null)return Result<bool>.Failure("El pago vinculado no puede eliminarse sin revertir la deuda.");
        var items=item.TransferGroupId is Guid groupId
            ?await transactions.ListTransferGroupAsync(groupId,userId,token)
            :new List<Transaction>{item};
        transactions.RemoveRange(items);await unitOfWork.SaveChangesAsync(token);
        return Result<bool>.Success(true);
    }
    private static TransactionDto Map(Transaction x)=>new(x.Id,x.FinancialAccountId,x.RelatedFinancialAccountId,
        x.CategoryId,x.Type,x.Amount.Amount,x.Amount.Currency,x.TransactionDate,x.Description,x.TransferGroupId,x.TransferDirection,x.DebtId,x.RecurringCommitmentId);
}
