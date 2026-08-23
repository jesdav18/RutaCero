using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.ExchangeRates;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/exchange-rates")]
public sealed class ExchangeRatesController(ExchangeRateService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<IReadOnlyList<ExchangeRateDto>>> List(CancellationToken t)=>Ok(await service.ListAsync(UserId(),t));
 [HttpPost]public async Task<ActionResult<ExchangeRateDto>> Create(CreateExchangeRateCommand c,CancellationToken t)=>Ok(await service.CreateAsync(UserId(),c,t));
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
