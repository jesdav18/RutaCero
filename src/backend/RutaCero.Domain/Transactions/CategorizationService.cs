using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Transactions;
public sealed class CategorizationService
{
 public Guid? Categorize(IEnumerable<CategorizationRule> rules,string description,string? institution,Guid accountId,Money amount,TransactionType type)=>rules.Where(x=>x.Matches(description,institution,accountId,amount,type)).OrderByDescending(x=>x.Priority).Select(x=>(Guid?)x.CategoryId).FirstOrDefault();
}
