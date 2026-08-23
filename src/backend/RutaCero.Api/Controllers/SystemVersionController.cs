using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RutaCero.Api.Controllers;

[ApiController,AllowAnonymous,Route("api/v1/system/version")]
public sealed class SystemVersionController:ControllerBase
{
    [HttpGet]
    public ActionResult Get()
    {
        var assembly=typeof(SystemVersionController).Assembly;
        var version=assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion??FormatVersion(assembly.GetName().Version);
        var commit=assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(x=>x.Key=="Commit")?.Value;
        return Ok(new{version,commit=ShortCommit(commit)});
    }

    private static string? ShortCommit(string? commit)=>string.IsNullOrWhiteSpace(commit)?null:commit[..Math.Min(7,commit.Length)];
    private static string FormatVersion(Version? version)=>version is null?string.Empty:$"{version.Major}.{version.Minor}.{version.Build}";
}
