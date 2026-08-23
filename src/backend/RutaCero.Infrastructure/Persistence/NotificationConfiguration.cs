using Microsoft.EntityFrameworkCore;using RutaCero.Domain.Notifications;
namespace RutaCero.Infrastructure.Persistence;
internal static class NotificationConfiguration
{
 public static void Configure(ModelBuilder b){var x=b.Entity<Notification>();x.ToTable("notifications");x.HasKey(v=>v.Id);x.Property(v=>v.Id).HasColumnName("id");x.Property(v=>v.UserId).HasColumnName("user_id");x.Property(v=>v.Type).HasColumnName("notification_type").HasConversion<string>();x.Property(v=>v.Title).HasColumnName("title");x.Property(v=>v.Message).HasColumnName("message");x.Property(v=>v.RelatedEntityType).HasColumnName("related_entity_type");x.Property(v=>v.RelatedEntityId).HasColumnName("related_entity_id");x.Property(v=>v.ScheduledFor).HasColumnName("scheduled_for");x.Property(v=>v.SentAt).HasColumnName("sent_at");x.Property(v=>v.ReadAt).HasColumnName("read_at");x.Property(v=>v.Status).HasColumnName("status").HasConversion<string>();x.Property(v=>v.DeduplicationKey).HasColumnName("deduplication_key");x.Property(v=>v.CreatedAt).HasColumnName("created_at");x.HasIndex(v=>v.DeduplicationKey).IsUnique();x.HasIndex(v=>new{v.UserId,v.ReadAt});}
}
