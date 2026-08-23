using Microsoft.EntityFrameworkCore;using RutaCero.Application.Imports;using RutaCero.Domain.Imports;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Infrastructure.Persistence;
public sealed class StatementImportRepository(RutaCeroDbContext db):IStatementImportRepository
{
 public Task<bool> ExistsAsync(Guid userId,ImportHash hash,CancellationToken token)=>db.StatementImports.AnyAsync(x=>x.UserId==userId&&x.Sha256==hash,token);
 public Task<StatementImport?> FindAsync(Guid id,Guid userId,CancellationToken token)=>db.StatementImports.SingleOrDefaultAsync(x=>x.Id==id&&x.UserId==userId,token);
 public async Task AddAsync(StatementImport item,CancellationToken token)=>await db.StatementImports.AddAsync(item,token);
 public async Task<IReadOnlyList<StatementImport>> ListAsync(Guid userId,CancellationToken token)=>await db.StatementImports.AsNoTracking().Where(x=>x.UserId==userId).OrderByDescending(x=>x.UploadedAt).ToListAsync(token);
 public async Task AddRowsAsync(IEnumerable<StatementImportRow> rows,CancellationToken token)=>await db.StatementImportRows.AddRangeAsync(rows,token);
 public async Task<IReadOnlyList<StatementImportRow>> ListRowsAsync(Guid importId,Guid userId,CancellationToken token)=>await db.StatementImportRows.Where(x=>x.StatementImportId==importId&&x.UserId==userId).OrderBy(x=>x.Date).ToListAsync(token);
 public Task<bool> FingerprintExistsAsync(Guid userId,string fingerprint,CancellationToken token)=>db.StatementImportRows.AnyAsync(x=>x.UserId==userId&&x.Fingerprint==fingerprint&&x.Status==ImportRowStatus.Accepted,token);
}
