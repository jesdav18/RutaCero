using RutaCero.Domain.Notifications;
namespace RutaCero.Application.Notifications;
public sealed record NotificationDto(Guid Id,NotificationType Type,string Title,string Message,string? RelatedEntityType,Guid? RelatedEntityId,DateTimeOffset ScheduledFor,DateTimeOffset? SentAt,DateTimeOffset? ReadAt,NotificationStatus Status);
public interface INotificationRepository
{
 Task<IReadOnlyList<Notification>> ListAsync(Guid userId,CancellationToken token);Task<bool> ExistsAsync(string key,CancellationToken token);
 Task<Notification?> FindAsync(Guid id,Guid userId,CancellationToken token);Task AddAsync(Notification item,CancellationToken token);
}
