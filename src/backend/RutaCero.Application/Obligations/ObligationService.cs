using RutaCero.Application.Common;
using RutaCero.Domain.Obligations;
using RutaCero.Domain.ValueObjects;
using RutaCero.Application.Debts;
using RutaCero.Domain.Debts;

namespace RutaCero.Application.Obligations;

public sealed class ObligationService(IObligationRepository repository,IDebtRepository debts,IUnitOfWork unitOfWork)
{
    public async Task<IReadOnlyList<ObligationDto>> ListAsync(Guid userId,DateOnly? from,DateOnly? to,CancellationToken token)
    {
        var today=DateOnly.FromDateTime(DateTime.UtcNow);var items=await repository.ListAsync(userId,from,to,token);
        foreach(var item in items)item.RefreshStatus(today);
        return items.Select(Map).ToList();
    }
    public async Task GenerateScheduledAsync(Guid userId,DateOnly from,DateOnly to,CancellationToken token)
    {
        var today=DateOnly.FromDateTime(DateTime.UtcNow);
        var scheduledDebts=(await debts.ListAsync(userId,token)).Where(x=>x.Status==DebtStatus.Active&&x.AutoGeneratePaymentObligations&&x.PaymentDueDay is not null).ToList();
        var first=from;var last=to;var month=new DateOnly(first.Year,first.Month,1);var lastMonth=new DateOnly(last.Year,last.Month,1);var added=false;
        if(scheduledDebts.Count==0)return;
        var existing=(await repository.ListScheduleKeysAsync(userId,scheduledDebts.Select(x=>x.Id).ToArray(),first,last,token)).ToHashSet();
        foreach(var debt in scheduledDebts)for(var cursor=month;cursor<=lastMonth;cursor=cursor.AddMonths(1))
        {
            var dueDate=new DateOnly(cursor.Year,cursor.Month,Math.Min(debt.PaymentDueDay!.Value,DateTime.DaysInMonth(cursor.Year,cursor.Month)));
            if(dueDate<first||dueDate>last)continue;
            var obligationType=debt.Type switch{DebtType.CreditCard=>ObligationType.CreditCardMinimumPayment,DebtType.Mortgage=>ObligationType.MortgageInstallment,DebtType.ExtraFinancing=>ObligationType.ExtraFinancingInstallment,_=>ObligationType.LoanInstallment};
            var key=new ObligationScheduleKey(debt.Id,dueDate,obligationType);if(existing.Contains(key))continue;
            decimal? amount=debt.RegularPayment.Amount>0?debt.RegularPayment.Amount:null;
            var item=new PaymentObligation(userId,debt.Id,obligationType,$"Pago de {debt.Name}",debt.CurrentPrincipal.Currency,amount,null,dueDate,amount is null,DateTimeOffset.UtcNow);
            item.RefreshStatus(today);await repository.AddAsync(item,token);existing.Add(key);added=true;
        }
        if(added)await unitOfWork.SaveChangesAsync(token);
    }
    public async Task<ObligationDto> CreateAsync(Guid userId,CreateObligationCommand command,CancellationToken token)
    {
        var item=new PaymentObligation(userId,command.DebtId,command.Type,command.Description,command.Currency,
            command.ExpectedAmount,command.MinimumAmount,command.DueDate,command.IsAmountEstimated,DateTimeOffset.UtcNow);
        item.RefreshStatus(DateOnly.FromDateTime(DateTime.UtcNow));await repository.AddAsync(item,token);await unitOfWork.SaveChangesAsync(token);return Map(item);
    }
    public async Task<Result<ObligationDto>> PayAsync(Guid userId,Guid id,PayObligationCommand command,CancellationToken token)
    {
        var item=await repository.FindAsync(id,userId,token);if(item is null)return Result<ObligationDto>.Failure("La obligación no existe.");
        item.ApplyPayment(new Money(command.Amount,item.Currency),DateTimeOffset.UtcNow);await unitOfWork.SaveChangesAsync(token);
        return Result<ObligationDto>.Success(Map(item));
    }
    private static ObligationDto Map(PaymentObligation x)=>new(x.Id,x.DebtId,x.Type,x.Description,x.Currency,
        x.ExpectedAmount?.Amount,x.MinimumAmount?.Amount,x.PaidAmount.Amount,x.DueDate,x.IsAmountEstimated,x.Status);
}
