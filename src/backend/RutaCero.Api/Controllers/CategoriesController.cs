using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RutaCero.Application.Transactions;

namespace RutaCero.Api.Controllers;

[ApiController,Authorize,Route("api/v1/categories")]
public sealed class CategoriesController(CategoryService service):ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> List(CancellationToken token)=>
        Ok(await service.ListAsync(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException()),token));
}
