using RutaCero.Domain.Transactions;using RutaCero.Domain.ValueObjects;
namespace RutaCero.Application.Transactions;
public sealed record CategorizationRuleDto(Guid Id,string Name,Guid CategoryId,DescriptionMatchType? MatchType,string? DescriptionPattern,string? InstitutionName,Guid? FinancialAccountId,decimal? MinimumAmount,decimal? MaximumAmount,Currency? Currency,TransactionType? TransactionType,int Priority,bool IsActive);
public sealed record CreateCategorizationRuleCommand(string Name,Guid CategoryId,DescriptionMatchType? MatchType,string? DescriptionPattern,string? InstitutionName,Guid? FinancialAccountId,decimal? MinimumAmount,decimal? MaximumAmount,Currency? Currency,TransactionType? TransactionType,int Priority);
public interface ICategorizationRuleRepository{Task<IReadOnlyList<CategorizationRule>> ListAsync(Guid userId,CancellationToken token);Task AddAsync(CategorizationRule rule,CancellationToken token);}
