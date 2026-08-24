using RutaCero.Application.Obligations;using RutaCero.Domain.Notifications;using RutaCero.Domain.Obligations;
namespace RutaCero.Application.Notifications;
public sealed class NotificationService(INotificationRepository notifications,IObligationRepository obligations,IUnitOfWork unitOfWork)
{
 public async Task<IReadOnlyList<NotificationDto>> ListAsync(Guid userId,CancellationToken token)=>(await notifications.ListAsync(userId,token)).Select(Map).ToList();
 public async Task GenerateAsync(Guid userId,CancellationToken token)
 {var today=DateOnly.FromDateTime(DateTime.UtcNow);var items=await obligations.ListAsync(userId,today.AddDays(-365),today.AddDays(7),token);foreach(var x in items){x.RefreshStatus(today);var type=x.Status switch{PaymentStatus.Overdue=>NotificationType.PaymentOverdue,PaymentStatus.DueToday=>NotificationType.PaymentDueToday,PaymentStatus.DueSoon=>NotificationType.PaymentUpcoming,_=>(NotificationType?)null};if(type is null)continue;var key=$"{type}:{x.Id}:{today:yyyyMMdd}";if(await notifications.ExistsAsync(key,token))continue;var item=new Notification(userId,type.Value,Title(type.Value),$"{x.Description} vence el {x.DueDate:dd/MM/yyyy}.","PaymentObligation",x.Id,DateTimeOffset.UtcNow,key,DateTimeOffset.UtcNow);item.MarkSent(DateTimeOffset.UtcNow);await notifications.AddAsync(item,token);}await unitOfWork.SaveChangesAsync(token);}
 public async Task<bool> ReadAsync(Guid userId,Guid id,CancellationToken token){var x=await notifications.FindAsync(id,userId,token);if(x is null)return false;x.MarkRead(DateTimeOffset.UtcNow);await unitOfWork.SaveChangesAsync(token);return true;}
 public async Task ReadAllAsync(Guid userId,CancellationToken token){var items=await notifications.ListAsync(userId,token);var now=DateTimeOffset.UtcNow;foreach(var x in items.Where(x=>x.ReadAt is null))x.MarkRead(now);await unitOfWork.SaveChangesAsync(token);}
 private static string Title(NotificationType x)=>x switch{NotificationType.PaymentOverdue=>"Pago vencido",NotificationType.PaymentDueToday=>"Pago vence hoy",_=>"Pago próximo"};
 private static NotificationDto Map(Notification x)=>new(x.Id,x.Type,x.Title,x.Message,x.RelatedEntityType,x.RelatedEntityId,x.ScheduledFor,x.SentAt,x.ReadAt,x.Status);
}
