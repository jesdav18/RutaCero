using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.Transactions;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/categorization-rules")]
public sealed class CategorizationRulesController(CategorizationRuleService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<IReadOnlyList<CategorizationRuleDto>>> List(CancellationToken t)=>Ok(await service.ListAsync(UserId(),t));
 [HttpPost]public async Task<ActionResult<CategorizationRuleDto>> Create(CreateCategorizationRuleCommand c,CancellationToken t)=>Ok(await service.CreateAsync(UserId(),c,t));
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
