using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RutaCero.Application.Obligations;

namespace RutaCero.Api.Controllers;

[ApiController,Authorize,Route("api/v1/payment-obligations")]
public sealed class PaymentObligationsController(ObligationService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<IReadOnlyList<ObligationDto>>> List(DateOnly? from,DateOnly? to,CancellationToken token)=>Ok(await service.ListAsync(UserId(),from,to,token));
 [HttpPost]public async Task<ActionResult<ObligationDto>> Create(CreateObligationCommand command,CancellationToken token)=>Ok(await service.CreateAsync(UserId(),command,token));
 [HttpPost("{id:guid}/payment")]public async Task<ActionResult<ObligationDto>> Pay(Guid id,PayObligationCommand command,CancellationToken token){var result=await service.PayAsync(UserId(),id,command,token);return result.IsSuccess?Ok(result.Value):NotFound(new ProblemDetails{Title=result.Error,Status=404});}
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
