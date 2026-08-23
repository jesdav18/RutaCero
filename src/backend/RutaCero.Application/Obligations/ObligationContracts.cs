using RutaCero.Domain.Obligations;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Application.Obligations;

public sealed record ObligationDto(Guid Id,Guid? DebtId,ObligationType Type,string Description,Currency Currency,
    decimal? ExpectedAmount,decimal? MinimumAmount,decimal PaidAmount,DateOnly DueDate,bool IsAmountEstimated,PaymentStatus Status);
public sealed record CreateObligationCommand(Guid? DebtId,ObligationType Type,string Description,Currency Currency,
    decimal? ExpectedAmount,decimal? MinimumAmount,DateOnly DueDate,bool IsAmountEstimated);
public sealed record PayObligationCommand(decimal Amount);
public sealed record ObligationScheduleKey(Guid DebtId,DateOnly DueDate,ObligationType Type);
public interface IObligationRepository
{
    Task<IReadOnlyList<PaymentObligation>> ListAsync(Guid userId,DateOnly? from,DateOnly? to,CancellationToken token);
    Task<PaymentObligation?> FindAsync(Guid id,Guid userId,CancellationToken token);
    Task AddAsync(PaymentObligation item,CancellationToken token);
    Task<IReadOnlySet<ObligationScheduleKey>> ListScheduleKeysAsync(Guid userId,IReadOnlyCollection<Guid> debtIds,DateOnly from,DateOnly to,CancellationToken token);
}
