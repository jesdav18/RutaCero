using Microsoft.EntityFrameworkCore;using RutaCero.Domain.ExchangeRates;
namespace RutaCero.Infrastructure.Persistence;
internal static class ExchangeRateConfiguration
{
 public static void Configure(ModelBuilder b){var x=b.Entity<ExchangeRate>();x.ToTable("exchange_rates");x.HasKey(v=>v.Id);x.Property(v=>v.Id).HasColumnName("id");x.Property(v=>v.UserId).HasColumnName("user_id");x.Property(v=>v.FromCurrency).HasColumnName("from_currency").HasConversion<string>();x.Property(v=>v.ToCurrency).HasColumnName("to_currency").HasConversion<string>();x.Property(v=>v.Rate).HasColumnName("rate").HasPrecision(18,8);x.Property(v=>v.EffectiveDate).HasColumnName("effective_date");x.Property(v=>v.Source).HasColumnName("source");x.Property(v=>v.CreatedAt).HasColumnName("created_at");x.HasIndex(v=>new{v.UserId,v.FromCurrency,v.ToCurrency,v.EffectiveDate}).IsUnique();}
}
