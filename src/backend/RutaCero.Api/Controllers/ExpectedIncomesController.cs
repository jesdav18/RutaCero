using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.Planning;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/expected-incomes")]
public sealed class ExpectedIncomesController(PlanningService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<IReadOnlyList<ExpectedIncomeDto>>> List(CancellationToken t)=>Ok(await service.IncomesAsync(UserId(),t));
 [HttpPost]public async Task<ActionResult<ExpectedIncomeDto>> Create(CreateExpectedIncomeCommand c,CancellationToken t)=>Ok(await service.AddIncomeAsync(UserId(),c,t));
 [HttpPut("{id:guid}")]public async Task<ActionResult<ExpectedIncomeDto>> Update(Guid id,CreateExpectedIncomeCommand c,CancellationToken t){var x=await service.UpdateIncomeAsync(UserId(),id,c,t);return x is null?NotFound():Ok(x);}
 [HttpDelete("{id:guid}")]public async Task<IActionResult> Delete(Guid id,CancellationToken t)=>await service.DeleteIncomeAsync(UserId(),id,t)?NoContent():NotFound();
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
