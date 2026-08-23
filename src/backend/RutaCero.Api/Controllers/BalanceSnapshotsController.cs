using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RutaCero.Application.Accounts;

namespace RutaCero.Api.Controllers;

[ApiController,Authorize,Route("api/v1/accounts/{accountId:guid}/balance-snapshots")]
public sealed class BalanceSnapshotsController(BalanceService service):ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BalanceSnapshotDto>>> List(Guid accountId,CancellationToken token)=>
        Ok(await service.ListAsync(UserId(),accountId,token));
    [HttpPost]
    public async Task<ActionResult<BalanceSnapshotDto>> Create(Guid accountId,CreateBalanceSnapshotCommand command,CancellationToken token)
    {
        var result=await service.CreateAsync(UserId(),accountId,command,token);
        return result.IsSuccess?Ok(result.Value):NotFound(new ProblemDetails{Title=result.Error,Status=404});
    }
    private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
