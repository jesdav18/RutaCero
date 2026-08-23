namespace RutaCero.Application.Transactions;

public sealed class CategoryService(ICategoryRepository repository)
{
    public async Task<IReadOnlyList<CategoryDto>> ListAsync(Guid userId,CancellationToken token)=>
        (await repository.ListAsync(userId,token)).Select(x=>new CategoryDto(x.Id,x.Name,x.IsIncome,x.IsSystem)).ToList();
}
