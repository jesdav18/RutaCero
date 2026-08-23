using RutaCero.Domain.Transactions;
namespace RutaCero.Application.Transactions;
public sealed class CategorizationRuleService(ICategorizationRuleRepository repository,IUnitOfWork unitOfWork)
{
 public async Task<IReadOnlyList<CategorizationRuleDto>> ListAsync(Guid userId,CancellationToken t)=>(await repository.ListAsync(userId,t)).Select(Map).ToList();
 public async Task<CategorizationRuleDto> CreateAsync(Guid userId,CreateCategorizationRuleCommand c,CancellationToken t){var x=new CategorizationRule(userId,c.Name,c.CategoryId,c.MatchType,c.DescriptionPattern,c.InstitutionName,c.FinancialAccountId,c.MinimumAmount,c.MaximumAmount,c.Currency,c.TransactionType,c.Priority);await repository.AddAsync(x,t);await unitOfWork.SaveChangesAsync(t);return Map(x);}
 private static CategorizationRuleDto Map(CategorizationRule x)=>new(x.Id,x.Name,x.CategoryId,x.MatchType,x.DescriptionPattern,x.InstitutionName,x.FinancialAccountId,x.MinimumAmount,x.MaximumAmount,x.Currency,x.TransactionType,x.Priority,x.IsActive);
}
