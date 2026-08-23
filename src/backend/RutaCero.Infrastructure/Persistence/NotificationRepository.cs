using Microsoft.EntityFrameworkCore;using RutaCero.Application.Notifications;using RutaCero.Domain.Notifications;
namespace RutaCero.Infrastructure.Persistence;
public sealed class NotificationRepository(RutaCeroDbContext db):INotificationRepository
{
 public async Task<IReadOnlyList<Notification>> ListAsync(Guid userId,CancellationToken token)=>await db.Notifications.AsNoTracking().Where(x=>x.UserId==userId).OrderByDescending(x=>x.CreatedAt).ToListAsync(token);
 public Task<bool> ExistsAsync(string key,CancellationToken token)=>db.Notifications.AnyAsync(x=>x.DeduplicationKey==key,token);
 public Task<Notification?> FindAsync(Guid id,Guid userId,CancellationToken token)=>db.Notifications.SingleOrDefaultAsync(x=>x.Id==id&&x.UserId==userId,token);
 public async Task AddAsync(Notification item,CancellationToken token)=>await db.Notifications.AddAsync(item,token);
}
