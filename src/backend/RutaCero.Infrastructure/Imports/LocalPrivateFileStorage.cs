using Microsoft.Extensions.Configuration;using RutaCero.Application.Imports;
namespace RutaCero.Infrastructure.Imports;
public sealed class LocalPrivateFileStorage(IConfiguration configuration):IPrivateFileStorage
{
 private readonly string _root=Path.GetFullPath(configuration["Storage:Root"]??"storage/statements");
 public async Task<string> SaveAsync(Guid userId,string storedName,Stream content,CancellationToken token)
 {var safe=Path.GetFileName(storedName);var directory=Path.GetFullPath(Path.Combine(_root,userId.ToString("N")));if(!directory.StartsWith(_root,StringComparison.OrdinalIgnoreCase))throw new UnauthorizedAccessException();Directory.CreateDirectory(directory);var path=Path.GetFullPath(Path.Combine(directory,safe));if(!path.StartsWith(directory,StringComparison.OrdinalIgnoreCase))throw new UnauthorizedAccessException();await using var output=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.None,81920,true);await content.CopyToAsync(output,token);return $"{userId:N}/{safe}";}
 public Task<Stream> OpenAsync(string storageKey,CancellationToken token){var path=Path.GetFullPath(Path.Combine(_root,storageKey.Replace('/',Path.DirectorySeparatorChar)));if(!path.StartsWith(_root,StringComparison.OrdinalIgnoreCase))throw new UnauthorizedAccessException();return Task.FromResult<Stream>(new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read,81920,true));}
}
