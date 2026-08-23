using Microsoft.EntityFrameworkCore;
using RutaCero.Domain.Accounts;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Infrastructure.Persistence;

internal static class BalanceConfiguration
{
    public static void Configure(ModelBuilder builder)
    {
        var item=builder.Entity<BalanceSnapshot>();item.ToTable("balance_snapshots");item.HasKey(x=>x.Id);
        item.Property(x=>x.Id).HasColumnName("id");item.Property(x=>x.UserId).HasColumnName("user_id");
        item.Property(x=>x.FinancialAccountId).HasColumnName("financial_account_id");item.Ignore(x=>x.Balance);
        item.Property<decimal>("_amount").HasColumnName("balance").HasPrecision(18,2);
        item.Property<Currency>("_currency").HasColumnName("currency").HasConversion<string>();
        item.Property(x=>x.SnapshotDate).HasColumnName("snapshot_date");item.Property(x=>x.Source).HasColumnName("source").HasConversion<string>();
        item.Property(x=>x.Confidence).HasColumnName("confidence").HasConversion<string>();item.Property(x=>x.CreatedAt).HasColumnName("created_at");
        item.HasIndex(x=>new{x.UserId,x.FinancialAccountId,x.SnapshotDate});
    }
}
