using RutaCero.Domain.Imports;using RutaCero.Domain.ValueObjects;using RutaCero.Domain.Transactions;
namespace RutaCero.Application.Imports;
public sealed record ParsedStatementRow(DateOnly Date,string Description,decimal Amount,bool IsCredit);
public sealed record StatementImportDto(Guid Id,Guid FinancialAccountId,string OriginalFileName,string ContentType,long Size,string Sha256,ImportStatus Status,int ExtractedCount,DateTimeOffset UploadedAt);
public sealed record ImportRowDto(Guid Id,DateOnly Date,string Description,decimal Amount,Currency Currency,bool IsCredit,ImportRowStatus Status,Guid? CategoryId,TransactionType? TransactionType);
public sealed record ConfirmImportRow(Guid Id,bool Accept,Guid? CategoryId,TransactionType TransactionType);
public sealed record ConfirmImportCommand(IReadOnlyList<ConfirmImportRow> Rows);
public interface IStatementImportRepository{Task<bool> ExistsAsync(Guid userId,ImportHash hash,CancellationToken token);Task<StatementImport?> FindAsync(Guid id,Guid userId,CancellationToken token);Task AddAsync(StatementImport item,CancellationToken token);Task<IReadOnlyList<StatementImport>> ListAsync(Guid userId,CancellationToken token);Task AddRowsAsync(IEnumerable<StatementImportRow> rows,CancellationToken token);Task<IReadOnlyList<StatementImportRow>> ListRowsAsync(Guid importId,Guid userId,CancellationToken token);Task<bool> FingerprintExistsAsync(Guid userId,string fingerprint,CancellationToken token);}
public interface IPrivateFileStorage{Task<string> SaveAsync(Guid userId,string storedName,Stream content,CancellationToken token);Task<Stream> OpenAsync(string storageKey,CancellationToken token);}
public interface IStatementParser{bool CanParse(string contentType,string extension);Task<IReadOnlyList<ParsedStatementRow>> ParseAsync(Stream stream,CancellationToken token);}
public interface IOcrStatementReader{Task<IReadOnlyList<ParsedStatementRow>> ReadAsync(Stream stream,CancellationToken token);}
