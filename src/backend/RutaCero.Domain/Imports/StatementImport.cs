using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Imports;
public enum ImportStatus { Uploaded,RequiresOcr,PendingReview,Confirmed,Rejected }
public sealed class StatementImport
{
 public Guid Id{get;private set;}public Guid UserId{get;private set;}public Guid FinancialAccountId{get;private set;}public string OriginalFileName{get;private set;}public string StoredFileName{get;private set;}public string ContentType{get;private set;}public long Size{get;private set;}public ImportHash Sha256{get;private set;}public string StorageKey{get;private set;}public ImportStatus Status{get;private set;}public int ExtractedCount{get;private set;}public DateTimeOffset UploadedAt{get;private set;}
 public StatementImport(Guid userId,Guid accountId,string original,string stored,string contentType,long size,ImportHash hash,string storageKey,DateTimeOffset uploaded){Id=Guid.NewGuid();UserId=userId;FinancialAccountId=accountId;OriginalFileName=original;StoredFileName=stored;ContentType=contentType;Size=size;Sha256=hash;StorageKey=storageKey;UploadedAt=uploaded.ToUniversalTime();Status=ImportStatus.Uploaded;}
 public void MarkRequiresOcr(){Status=ImportStatus.RequiresOcr;}public void MarkPendingReview(int count){ExtractedCount=count;Status=ImportStatus.PendingReview;}public void Confirm(){Status=ImportStatus.Confirmed;}
 private StatementImport(){OriginalFileName=StoredFileName=ContentType=StorageKey=string.Empty;}
}
