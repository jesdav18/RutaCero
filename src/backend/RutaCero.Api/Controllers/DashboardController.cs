using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.Dashboard;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/dashboard")]
public sealed class DashboardController(DashboardService service,ExpenseAnalyticsService expenses):ControllerBase
{
 [HttpGet]public async Task<ActionResult<DashboardDto>> Get(CancellationToken t)=>Ok(await service.GetAsync(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException()),t));
 [HttpGet("expenses")]public async Task<ActionResult<ExpenseAnalyticsDto>> Expenses(int? year,int? month,CancellationToken t){var today=DateOnly.FromDateTime(DateTime.UtcNow);return Ok(await expenses.GetAsync(UserId(),year??today.Year,month??today.Month,t));}
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
