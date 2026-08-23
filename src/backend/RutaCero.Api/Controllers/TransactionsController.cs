using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RutaCero.Application.Transactions;

namespace RutaCero.Api.Controllers;

[ApiController,Authorize,Route("api/v1/transactions")]
public sealed class TransactionsController(TransactionService service):ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TransactionDto>>> List(DateOnly? from,DateOnly? to,CancellationToken token)=>
        Ok(await service.ListAsync(UserId(),from,to,token));
    [HttpPost]
    public async Task<ActionResult<TransactionDto>> Create(CreateTransactionCommand command,CancellationToken token)
    {
        var result=await service.CreateAsync(UserId(),command,token);
        return result.IsSuccess?Ok(result.Value):BadRequest(new ProblemDetails{Title=result.Error,Status=400});
    }
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TransactionDto>> Update(Guid id,UpdateTransactionCommand command,CancellationToken token)
    {
        var result=await service.UpdateAsync(UserId(),id,command,token);
        return result.IsSuccess?Ok(result.Value):BadRequest(new ProblemDetails{Title=result.Error,Status=400});
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id,CancellationToken token)
    {
        var result=await service.DeleteAsync(UserId(),id,token);
        return result.IsSuccess?NoContent():NotFound(new ProblemDetails{Title=result.Error,Status=404});
    }
    private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
