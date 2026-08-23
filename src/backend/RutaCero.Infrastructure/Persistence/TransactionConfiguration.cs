using Microsoft.EntityFrameworkCore;
using RutaCero.Domain.Transactions;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Infrastructure.Persistence;

internal static class TransactionConfiguration
{
    public static void Configure(ModelBuilder builder)
    {
        var item=builder.Entity<Transaction>();item.ToTable("transactions");item.HasKey(x=>x.Id);
        item.Property(x=>x.Id).HasColumnName("id");item.Property(x=>x.UserId).HasColumnName("user_id");
        item.Property(x=>x.FinancialAccountId).HasColumnName("financial_account_id");
        item.Property(x=>x.RelatedFinancialAccountId).HasColumnName("related_financial_account_id");
        item.Property(x=>x.DebtId).HasColumnName("debt_id");
        item.Property(x=>x.CategoryId).HasColumnName("category_id");item.Property(x=>x.Type).HasColumnName("transaction_type").HasConversion<string>();
        item.Ignore(x=>x.Amount);item.Ignore(x=>x.CountsAsExpense);item.Property<decimal>("_amount").HasColumnName("amount").HasPrecision(18,2);
        item.Property<Currency>("_currency").HasColumnName("currency").HasConversion<string>();
        item.Property(x=>x.TransactionDate).HasColumnName("transaction_date");item.Property(x=>x.Description).HasColumnName("description");
        item.Property(x=>x.CreatedAt).HasColumnName("created_at");item.HasIndex(x=>new{x.UserId,x.TransactionDate});
        item.Property(x=>x.TransferGroupId).HasColumnName("transfer_group_id");
        item.Property(x=>x.TransferDirection).HasColumnName("transfer_direction").HasConversion<string>();
        item.HasIndex(x=>x.DebtId);
        var category=builder.Entity<TransactionCategory>();category.ToTable("transaction_categories");category.HasKey(x=>x.Id);
        category.Property(x=>x.Id).HasColumnName("id");category.Property(x=>x.UserId).HasColumnName("user_id");
        category.Property(x=>x.Name).HasColumnName("name");category.Property(x=>x.IsIncome).HasColumnName("is_income");
        category.Property(x=>x.IsSystem).HasColumnName("is_system");category.HasIndex(x=>new{x.UserId,x.Name}).IsUnique();
        var typeSetting=builder.Entity<TransactionTypeSetting>();typeSetting.ToTable("transaction_type_settings");
        typeSetting.HasKey(x=>new{x.UserId,x.Code});typeSetting.Property(x=>x.UserId).HasColumnName("user_id");
        typeSetting.Property(x=>x.Code).HasColumnName("code").HasConversion<string>();
        typeSetting.Property(x=>x.Label).HasColumnName("label").HasMaxLength(80);
        typeSetting.Property(x=>x.Effect).HasColumnName("effect").HasConversion<string>();
    }
}
