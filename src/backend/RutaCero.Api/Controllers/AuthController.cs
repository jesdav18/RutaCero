using Microsoft.AspNetCore.Mvc;
using RutaCero.Application.Auth;

namespace RutaCero.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(AuthService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterCommand command, CancellationToken token) =>
        ToAction(await service.RegisterAsync(command, token));

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command, CancellationToken token) =>
        ToAction(await service.LoginAsync(command, token));

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshCommand command, CancellationToken token) =>
        ToAction(await service.RefreshAsync(command, token));

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke(RefreshCommand command,CancellationToken token)=>
        await service.RevokeAsync(command,token)?NoContent():BadRequest();

    private ActionResult<AuthResponse> ToAction(Application.Common.Result<AuthResponse> result) =>
        result.IsSuccess ? Ok(result.Value) : BadRequest(new ProblemDetails { Title = result.Error, Status = 400 });
}
