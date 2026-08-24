using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.Notifications;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/notifications")]
public sealed class NotificationsController(NotificationService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<IReadOnlyList<NotificationDto>>> List(CancellationToken t){await service.GenerateAsync(UserId(),t);return Ok(await service.ListAsync(UserId(),t));}
 [HttpPost("{id:guid}/read")]public async Task<IActionResult> Read(Guid id,CancellationToken t)=>await service.ReadAsync(UserId(),id,t)?NoContent():NotFound();
 [HttpPost("read-all")]public async Task<IActionResult> ReadAll(CancellationToken t){await service.ReadAllAsync(UserId(),t);return NoContent();}
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
