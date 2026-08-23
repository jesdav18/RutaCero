using RutaCero.Domain.Common;using RutaCero.Domain.Transactions;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Imports;
public enum ImportRowStatus { Pending,Duplicate,Accepted,Rejected }
public sealed class StatementImportRow
{
 private decimal _amount;private Currency _currency;public Guid Id{get;private set;}public Guid UserId{get;private set;}public Guid StatementImportId{get;private set;}public string Fingerprint{get;private set;}public DateOnly Date{get;private set;}public string Description{get;private set;}public Money Amount=>new(_amount,_currency);public bool IsCredit{get;private set;}public ImportRowStatus Status{get;private set;}public Guid? CategoryId{get;private set;}public TransactionType? TransactionType{get;private set;}
 public StatementImportRow(Guid userId,Guid importId,string fingerprint,DateOnly date,string description,Money amount,bool credit,bool duplicate){if(amount.Amount<=0)throw new DomainException("Imported amount is invalid.");Id=Guid.NewGuid();UserId=userId;StatementImportId=importId;Fingerprint=fingerprint;Date=date;Description=description.Trim();_amount=amount.Amount;_currency=amount.Currency;IsCredit=credit;Status=duplicate?ImportRowStatus.Duplicate:ImportRowStatus.Pending;}
 public void Suggest(Guid? categoryId,TransactionType type){if(Status!=ImportRowStatus.Pending)return;CategoryId=categoryId;TransactionType=type;}
 public void Accept(Guid? categoryId,TransactionType type){if(Status==ImportRowStatus.Duplicate)throw new DomainException("A duplicate row cannot be accepted.");CategoryId=categoryId;TransactionType=type;Status=ImportRowStatus.Accepted;}public void Reject(){Status=ImportRowStatus.Rejected;}
 private StatementImportRow(){Fingerprint=Description=string.Empty;}
}
