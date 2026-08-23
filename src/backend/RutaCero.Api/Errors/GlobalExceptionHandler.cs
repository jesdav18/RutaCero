using Microsoft.AspNetCore.Diagnostics;using Microsoft.AspNetCore.Mvc;using RutaCero.Domain.Common;
namespace RutaCero.Api.Errors;
public sealed class GlobalExceptionHandler(IProblemDetailsService problems,ILogger<GlobalExceptionHandler> logger):IExceptionHandler
{
 public async ValueTask<bool> TryHandleAsync(HttpContext context,Exception exception,CancellationToken token){var status=exception switch{DomainException=>StatusCodes.Status400BadRequest,UnauthorizedAccessException=>StatusCodes.Status401Unauthorized,_=>StatusCodes.Status500InternalServerError};if(status==500)logger.LogError(exception,"Unhandled request failure");var details=new ProblemDetails{Status=status,Title=status==500?"Ocurrió un error inesperado.":exception.Message};context.Response.StatusCode=status;return await problems.TryWriteAsync(new(){HttpContext=context,ProblemDetails=details,Exception=exception});}
}
