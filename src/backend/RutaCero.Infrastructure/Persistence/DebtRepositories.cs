using Microsoft.EntityFrameworkCore;
using RutaCero.Application.Debts;
using RutaCero.Domain.Debts;

namespace RutaCero.Infrastructure.Persistence;

public sealed class DebtRepository(RutaCeroDbContext db) : IDebtRepository
{
    public async Task<IReadOnlyList<Debt>> ListAsync(Guid userId, CancellationToken token) =>
        await db.Debts.AsNoTracking().Where(x => x.UserId == userId).ToListAsync(token);
    public Task<Debt?> FindAsync(Guid id, Guid userId, CancellationToken token) =>
        db.Debts.SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId, token);
    public async Task AddAsync(Debt debt, CancellationToken token) => await db.Debts.AddAsync(debt, token);
}

public sealed class DebtPaymentRepository(RutaCeroDbContext db) : IDebtPaymentRepository
{
    public async Task AddAsync(DebtPayment payment, CancellationToken token) => await db.DebtPayments.AddAsync(payment, token);
}
public sealed class DebtBalanceSnapshotRepository(RutaCeroDbContext db):IDebtBalanceSnapshotRepository
{
    public async Task<IReadOnlyList<DebtBalanceSnapshot>> ListAsync(Guid debtId,Guid userId,CancellationToken token)=>await db.DebtBalanceSnapshots.AsNoTracking().Where(x=>x.DebtId==debtId&&x.UserId==userId).OrderByDescending(x=>x.StatementDate).ToListAsync(token);
    public Task<bool> ExistsForImportAsync(Guid statementImportId,Guid userId,CancellationToken token)=>db.DebtBalanceSnapshots.AnyAsync(x=>x.StatementImportId==statementImportId&&x.UserId==userId,token);
    public async Task AddAsync(DebtBalanceSnapshot snapshot,CancellationToken token)=>await db.DebtBalanceSnapshots.AddAsync(snapshot,token);
}
