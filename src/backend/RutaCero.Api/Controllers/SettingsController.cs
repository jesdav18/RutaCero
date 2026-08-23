using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.Planning;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/settings")]
public sealed class SettingsController(PlanningService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<SettingsDto>> Get(CancellationToken t)=>Ok(await service.SettingsAsync(UserId(),t));
 [HttpPut]public async Task<ActionResult<SettingsDto>> Save(SettingsDto c,CancellationToken t)=>Ok(await service.SaveSettingsAsync(UserId(),c,t));
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
