using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.Planning;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/expected-incomes")]
public sealed class ExpectedIncomesController(PlanningService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<IReadOnlyList<ExpectedIncomeDto>>> List(CancellationToken t)=>Ok(await service.IncomesAsync(UserId(),t));
 [HttpPost]public async Task<ActionResult<ExpectedIncomeDto>> Create(CreateExpectedIncomeCommand c,CancellationToken t)=>Ok(await service.AddIncomeAsync(UserId(),c,t));
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
