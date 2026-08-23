using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.Planning;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/recurring-commitments")]
public sealed class RecurringCommitmentsController(PlanningService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<IReadOnlyList<CommitmentDto>>> List(CancellationToken t)=>Ok(await service.CommitmentsAsync(UserId(),t));
 [HttpPost]public async Task<ActionResult<CommitmentDto>> Create(CreateCommitmentCommand c,CancellationToken t)=>Ok(await service.AddCommitmentAsync(UserId(),c,t));
 [HttpPut("{id:guid}")]public async Task<ActionResult<CommitmentDto>> Update(Guid id,CreateCommitmentCommand c,CancellationToken t){var x=await service.UpdateCommitmentAsync(UserId(),id,c,t);return x is null?NotFound():Ok(x);}
 [HttpDelete("{id:guid}")]public async Task<IActionResult> Delete(Guid id,CancellationToken t)=>await service.DeleteCommitmentAsync(UserId(),id,t)?NoContent():NotFound();
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
