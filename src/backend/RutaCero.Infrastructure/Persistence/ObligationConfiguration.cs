using Microsoft.EntityFrameworkCore;
using RutaCero.Domain.Obligations;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Infrastructure.Persistence;

internal static class ObligationConfiguration
{
 public static void Configure(ModelBuilder builder){var x=builder.Entity<PaymentObligation>();x.ToTable("payment_obligations");x.HasKey(v=>v.Id);
 x.Property(v=>v.Id).HasColumnName("id");x.Property(v=>v.UserId).HasColumnName("user_id");x.Property(v=>v.DebtId).HasColumnName("debt_id");x.Property(v=>v.Type).HasColumnName("obligation_type").HasConversion<string>();x.Property(v=>v.Description).HasColumnName("description");x.Ignore(v=>v.Currency);x.Ignore(v=>v.ExpectedAmount);x.Ignore(v=>v.MinimumAmount);x.Ignore(v=>v.PaidAmount);x.Property<Currency>("_currency").HasColumnName("currency").HasConversion<string>();x.Property<decimal?>("_expectedAmount").HasColumnName("expected_amount").HasPrecision(18,2);x.Property<decimal?>("_minimumAmount").HasColumnName("minimum_amount").HasPrecision(18,2);x.Property<decimal>("_paidAmount").HasColumnName("paid_amount").HasPrecision(18,2);x.Property(v=>v.DueDate).HasColumnName("due_date");x.Property(v=>v.IsAmountEstimated).HasColumnName("is_amount_estimated");x.Property(v=>v.Status).HasColumnName("payment_status").HasConversion<string>();x.Property(v=>v.PaidAt).HasColumnName("paid_at");x.Property(v=>v.CreatedAt).HasColumnName("created_at");x.HasIndex(v=>new{v.UserId,v.DueDate});}
}
