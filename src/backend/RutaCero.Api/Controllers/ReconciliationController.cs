using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.Reconciliation;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/reconciliation")]
public sealed class ReconciliationController(ReconciliationService service):ControllerBase
{
 [HttpGet("{accountId:guid}")]public async Task<ActionResult<ReconciliationDto>> Get(Guid accountId,CancellationToken t){var r=await service.GetAsync(UserId(),accountId,t);return r.IsSuccess?Ok(r.Value):NotFound(new ProblemDetails{Title=r.Error,Status=404});}
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
