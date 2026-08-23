using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.Planning;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/recurring-commitments")]
public sealed class RecurringCommitmentsController(PlanningService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<IReadOnlyList<CommitmentDto>>> List(CancellationToken t)=>Ok(await service.CommitmentsAsync(UserId(),t));
 [HttpPost]public async Task<ActionResult<CommitmentDto>> Create(CreateCommitmentCommand c,CancellationToken t)=>Ok(await service.AddCommitmentAsync(UserId(),c,t));
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
