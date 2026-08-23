using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RutaCero.Application.Debts;

namespace RutaCero.Api.Controllers;

[ApiController, Authorize, Route("api/v1/debts")]
public sealed class DebtsController(DebtService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DebtDto>>> List(CancellationToken token) =>
        Ok(await service.ListAsync(UserId(), token));

    [HttpPost]
    public async Task<ActionResult<DebtDto>> Create(CreateDebtCommand command, CancellationToken token)
    {
        var debt = await service.CreateAsync(UserId(), command, token);
        return Created($"/api/v1/debts/{debt.Id}", debt);
    }

    [HttpPost("{id:guid}/payments")]
    public async Task<ActionResult<DebtDto>> Pay(Guid id, RegisterDebtPaymentCommand command, CancellationToken token)
    {
        var result = await service.PayAsync(UserId(), id, command, token);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new ProblemDetails { Title = result.Error, Status = 404 });
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DebtDto>> Update(Guid id, UpdateDebtCommand command, CancellationToken token)
    {
        var result = await service.UpdateAsync(UserId(), id, command, token);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new ProblemDetails { Title = result.Error, Status = 404 });
    }
    [HttpGet("{id:guid}/balance-history")]
    public async Task<ActionResult<IReadOnlyList<DebtBalanceSnapshotDto>>> BalanceHistory(Guid id,CancellationToken token)=>Ok(await service.BalanceHistoryAsync(UserId(),id,token));
    [HttpPost("{id:guid}/balance-confirmations")]
    public async Task<ActionResult<DebtBalanceSnapshotDto>> ConfirmBalance(Guid id,ConfirmDebtBalanceCommand command,CancellationToken token)
    {var result=await service.ConfirmBalanceAsync(UserId(),id,command,token);return result.IsSuccess?Ok(result.Value):BadRequest(new ProblemDetails{Title=result.Error,Status=400});}

    private Guid UserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException());
}
