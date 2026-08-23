using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RutaCero.Application.Accounts;

namespace RutaCero.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/accounts")]
public sealed class AccountsController(AccountService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AccountDto>>> List(CancellationToken token) =>
        Ok(await service.ListAsync(GetUserId(), token));

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(CreateAccountCommand command, CancellationToken token)
    {
        var result = await service.CreateAsync(GetUserId(), command, token);
        return Created($"/api/v1/accounts/{result.Id}", result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AccountDto>> Update(Guid id,UpdateAccountCommand command,CancellationToken token)
    {
        var result=await service.UpdateAsync(GetUserId(),id,command,token);
        return result.IsSuccess?Ok(result.Value):NotFound(new ProblemDetails{Title=result.Error,Status=404});
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException());
}
