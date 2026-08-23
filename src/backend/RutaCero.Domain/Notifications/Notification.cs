namespace RutaCero.Domain.Notifications;
public enum NotificationType { PaymentUpcoming,PaymentDueToday,PaymentOverdue,StatementClosingSoon,LowAvailableCashForUpcomingPayments,InsufficientFundsForPayment,ImportPendingReview,ReconciliationDifference }
public enum NotificationStatus { Scheduled,Sent,Cancelled }
public sealed class Notification
{
 public Guid Id{get;private set;}public Guid UserId{get;private set;}public NotificationType Type{get;private set;}public string Title{get;private set;}public string Message{get;private set;}public string? RelatedEntityType{get;private set;}public Guid? RelatedEntityId{get;private set;}public DateTimeOffset ScheduledFor{get;private set;}public DateTimeOffset? SentAt{get;private set;}public DateTimeOffset? ReadAt{get;private set;}public NotificationStatus Status{get;private set;}public string DeduplicationKey{get;private set;}public DateTimeOffset CreatedAt{get;private set;}
 public Notification(Guid userId,NotificationType type,string title,string message,string? relatedType,Guid? relatedId,DateTimeOffset scheduled,string key,DateTimeOffset created){Id=Guid.NewGuid();UserId=userId;Type=type;Title=title.Trim();Message=message.Trim();RelatedEntityType=relatedType;RelatedEntityId=relatedId;ScheduledFor=scheduled.ToUniversalTime();DeduplicationKey=key;CreatedAt=created.ToUniversalTime();Status=NotificationStatus.Scheduled;}
 public void MarkSent(DateTimeOffset now){Status=NotificationStatus.Sent;SentAt=now.ToUniversalTime();}public void MarkRead(DateTimeOffset now){ReadAt=now.ToUniversalTime();}
 private Notification(){Title=Message=DeduplicationKey=string.Empty;}
}
