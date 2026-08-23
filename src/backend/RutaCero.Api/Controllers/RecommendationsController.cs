using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;
using RutaCero.Application.Recommendations;using RutaCero.Domain.Recommendations;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/recommendations")]
public sealed class RecommendationsController(RecommendationApplicationService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<IReadOnlyList<RecommendationDto>>> Get(RecommendationStrategy strategy=RecommendationStrategy.Avalanche,CancellationToken token=default)=>Ok(await service.GetAsync(UserId(),strategy,token));
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
