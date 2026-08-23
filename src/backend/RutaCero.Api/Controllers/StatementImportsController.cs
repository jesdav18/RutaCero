using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using RutaCero.Application.Imports;
namespace RutaCero.Api.Controllers;
[ApiController,Authorize,Route("api/v1/statement-imports")]
public sealed class StatementImportsController(StatementImportService service):ControllerBase
{
 [HttpGet]public async Task<ActionResult<IReadOnlyList<StatementImportDto>>> List(CancellationToken t)=>Ok(await service.ListAsync(UserId(),t));
 [HttpGet("{id:guid}/rows")]public async Task<ActionResult<IReadOnlyList<ImportRowDto>>> Rows(Guid id,CancellationToken t)=>Ok(await service.RowsAsync(UserId(),id,t));
 [HttpPost("{id:guid}/confirm")]public async Task<ActionResult<int>> Confirm(Guid id,ConfirmImportCommand command,CancellationToken t){var result=await service.ConfirmAsync(UserId(),id,command,t);return result.IsSuccess?Ok(result.Value):BadRequest(new ProblemDetails{Title=result.Error,Status=400});}
 [HttpPost,RequestSizeLimit(20_000_000)]public async Task<ActionResult<StatementImportDto>> Upload([FromForm]Guid financialAccountId,[FromForm]IFormFile file,CancellationToken t){await using var stream=file.OpenReadStream();var result=await service.UploadAsync(UserId(),financialAccountId,file.FileName,file.ContentType,file.Length,stream,t);return result.IsSuccess?Ok(result.Value):BadRequest(new ProblemDetails{Title=result.Error,Status=400});}
 private Guid UserId()=>Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)??throw new UnauthorizedAccessException());
}
