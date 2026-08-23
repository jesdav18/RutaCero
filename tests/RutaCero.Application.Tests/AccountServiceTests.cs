using RutaCero.Application;
using RutaCero.Application.Accounts;
using RutaCero.Domain.Accounts;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Application.Tests;

public sealed class AccountServiceTests
{
    [Fact]
    public async Task Creates_account_for_authenticated_user()
    {
        var repository = new Accounts(); var service = new AccountService(repository, new UnitOfWork());
        var userId = Guid.NewGuid();
        var result = await service.CreateAsync(userId, new("BAC", "Ahorro", "1234", AccountType.SavingsAccount,
            500, Currency.HNL, new DateOnly(2026, 8, 19), 100, true), CancellationToken.None);
        Assert.Equal(userId, repository.Items.Single().UserId);
        Assert.Equal(500, result.Balance);
    }

    private sealed class Accounts : IFinancialAccountRepository
    {
        public List<FinancialAccount> Items { get; } = [];
        public Task<IReadOnlyList<FinancialAccount>> ListAsync(Guid id, CancellationToken token) => Task.FromResult<IReadOnlyList<FinancialAccount>>(Items);
        public Task<FinancialAccount?> FindAsync(Guid id,Guid userId,CancellationToken token)=>Task.FromResult(Items.SingleOrDefault(x=>x.Id==id&&x.UserId==userId));
        public Task AddAsync(FinancialAccount account, CancellationToken token) { Items.Add(account); return Task.CompletedTask; }
    }
    private sealed class UnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken token) => Task.FromResult(1);
    }
}
