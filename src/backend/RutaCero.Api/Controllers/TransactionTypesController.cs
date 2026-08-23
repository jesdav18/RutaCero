using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RutaCero.Application.Transactions;

namespace RutaCero.Api.Controllers;

public sealed record UpdateTransactionTypeSetting(string Label,string Effect);

[ApiController,Authorize,Route("api/v1/transaction-types")]
public sealed class TransactionTypesController(TransactionTypeSettingService service):ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TransactionTypeSettingDto>>> List(CancellationToken token)=>
        Ok(await service.ListAsync(UserId(),token));

    [HttpPut("{code}")]
    public async Task<ActionResult<TransactionTypeSettingDto>> Update(string code,UpdateTransactionTypeSetting command,CancellationToken token)
    {
        var result=await service.UpdateAsync(UserId(),code,command.Label,command.Effect,token);
        return result is null?BadRequest():Ok(result);
    }

    private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
