using RutaCero.Domain.Planning;
using RutaCero.Domain.ValueObjects;
namespace RutaCero.Domain.Tests;
public sealed class SafetyReserveTests
{
 [Theory][InlineData(SafetyReserveMode.FixedAmount,5000)][InlineData(SafetyReserveMode.EssentialExpenseDays,3000)][InlineData(SafetyReserveMode.HighestOfBoth,5000)]
 public void Calculates_configured_reserve(SafetyReserveMode mode,decimal expected){var settings=new UserFinancialSettings(Guid.NewGuid(),5000,mode,30,RecommendationProfile.Balanced,"America/Tegucigalpa",Currency.HNL,false);Assert.Equal(expected,settings.CalculateReserve(100));}
}
