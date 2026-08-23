using RutaCero.Application.Common;
using RutaCero.Domain.Debts;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Application.Debts;

public sealed class DebtService(IDebtRepository debts, IDebtPaymentRepository payments, IUnitOfWork unitOfWork,RutaCero.Application.Obligations.ObligationService obligations,IDebtBalanceSnapshotRepository snapshots)
{
    public async Task<IReadOnlyList<DebtDto>> ListAsync(Guid userId, CancellationToken token) =>
        (await debts.ListAsync(userId, token)).Select(Map).ToList();

    public async Task<DebtDto> CreateAsync(Guid userId, CreateDebtCommand command, CancellationToken token)
    {
        var debt = new Debt(userId, command.InstitutionName, command.Name, command.Type,
            new(command.OriginalPrincipal, command.Currency), command.AnnualInterestRate,
            new(command.RegularPayment, command.Currency), command.AllowsCapitalPrepayment, command.HasPrepaymentPenalty);
        debt.ConfigureCreditCardSchedule(command.StatementClosingDay,command.PaymentDueDay,command.AutoGeneratePaymentObligations);
        await debts.AddAsync(debt, token); await unitOfWork.SaveChangesAsync(token);
        if(debt.AutoGeneratePaymentObligations){var today=DateOnly.FromDateTime(DateTime.UtcNow);await obligations.GenerateScheduledAsync(userId,today,today.AddMonths(24),token);}
        return Map(debt);
    }

    public async Task<Result<DebtDto>> UpdateAsync(Guid userId, Guid debtId, UpdateDebtCommand command, CancellationToken token)
    {
        var debt = await debts.FindAsync(debtId, userId, token);
        if (debt is null) return Result<DebtDto>.Failure("La deuda no existe.");
        debt.UpdateDetails(command.InstitutionName, command.Name, command.Type, command.AnnualInterestRate,
            new Money(command.RegularPayment, debt.CurrentPrincipal.Currency), command.AllowsCapitalPrepayment,
            command.HasPrepaymentPenalty,command.StatementClosingDay,command.PaymentDueDay,command.AutoGeneratePaymentObligations);
        await unitOfWork.SaveChangesAsync(token);
        if(debt.AutoGeneratePaymentObligations){var today=DateOnly.FromDateTime(DateTime.UtcNow);await obligations.GenerateScheduledAsync(userId,today,today.AddMonths(24),token);}
        return Result<DebtDto>.Success(Map(debt));
    }

    public async Task<Result<DebtDto>> PayAsync(Guid userId, Guid debtId, RegisterDebtPaymentCommand command, CancellationToken token)
    {
        var debt = await debts.FindAsync(debtId, userId, token);
        if (debt is null) return Result<DebtDto>.Failure("La deuda no existe.");
        var payment = new DebtPayment(debt.Id, command.PaymentDate,
            new(command.TotalAmount, debt.CurrentPrincipal.Currency),
            command.PrincipalAmount is null ? null : new Money(command.PrincipalAmount.Value, debt.CurrentPrincipal.Currency),
            command.Type, command.IsAllocationConfirmed);
        payment.ApplyTo(debt); await payments.AddAsync(payment, token); await unitOfWork.SaveChangesAsync(token);
        return Result<DebtDto>.Success(Map(debt));
    }
    public async Task<Result<DebtBalanceSnapshotDto>> ConfirmBalanceAsync(Guid userId,Guid debtId,ConfirmDebtBalanceCommand command,CancellationToken token)
    {
        var debt=await debts.FindAsync(debtId,userId,token);if(debt is null)return Result<DebtBalanceSnapshotDto>.Failure("La deuda no existe.");
        if(command.StatementImportId is Guid importId&&await snapshots.ExistsForImportAsync(importId,userId,token))return Result<DebtBalanceSnapshotDto>.Failure("Este estado ya confirmó un saldo de deuda.");
        var balance=new Money(command.Balance,debt.CurrentPrincipal.Currency);var snapshot=new DebtBalanceSnapshot(userId,debtId,command.StatementImportId,command.StatementDate,balance,DateTimeOffset.UtcNow);
        debt.ConfirmCurrentBalance(balance);await snapshots.AddAsync(snapshot,token);await unitOfWork.SaveChangesAsync(token);return Result<DebtBalanceSnapshotDto>.Success(Map(snapshot));
    }
    public async Task<IReadOnlyList<DebtBalanceSnapshotDto>> BalanceHistoryAsync(Guid userId,Guid debtId,CancellationToken token)=>(await snapshots.ListAsync(debtId,userId,token)).Select(Map).ToList();

    private static DebtDto Map(Debt debt) => new(debt.Id, debt.InstitutionName, debt.Name, debt.Type,
        debt.OriginalPrincipal.Amount, debt.CurrentPrincipal.Amount, debt.CurrentPrincipal.Currency,
        debt.AnnualInterestRate, debt.RegularPayment.Amount, debt.AllowsCapitalPrepayment,
        debt.HasPrepaymentPenalty, debt.Status,
        decimal.Round((debt.OriginalPrincipal.Amount - debt.CurrentPrincipal.Amount) / debt.OriginalPrincipal.Amount * 100, 2),
        debt.StatementClosingDay,debt.PaymentDueDay,debt.AutoGeneratePaymentObligations);
    private static DebtBalanceSnapshotDto Map(DebtBalanceSnapshot x)=>new(x.Id,x.DebtId,x.StatementImportId,x.StatementDate,x.Balance.Amount,x.Balance.Currency,x.CreatedAt);
}
