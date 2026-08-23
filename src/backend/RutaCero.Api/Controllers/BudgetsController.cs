using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.Planning;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/budgets")]
public sealed class BudgetsController(PlanningService service,BudgetProgressService progress):ControllerBase
{
 [HttpGet]public async Task<ActionResult<IReadOnlyList<BudgetDto>>> List(int year,int month,CancellationToken t)=>Ok(await service.BudgetsAsync(UserId(),year,month,t));
 [HttpPut]public async Task<ActionResult<BudgetDto>> Set(CreateBudgetCommand c,CancellationToken t)=>Ok(await service.SetBudgetAsync(UserId(),c,t));
 [HttpGet("progress")]public async Task<ActionResult<IReadOnlyList<BudgetProgressDto>>> Progress(int year,int month,CancellationToken t)=>Ok(await progress.GetAsync(UserId(),year,month,t));
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
