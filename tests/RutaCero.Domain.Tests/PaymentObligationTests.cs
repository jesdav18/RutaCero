using RutaCero.Domain.Obligations;
using RutaCero.Domain.ValueObjects;

namespace RutaCero.Domain.Tests;

public sealed class PaymentObligationTests
{
    [Theory]
    [InlineData(-1,PaymentStatus.Overdue)][InlineData(0,PaymentStatus.DueToday)]
    [InlineData(3,PaymentStatus.DueSoon)][InlineData(10,PaymentStatus.Upcoming)]
    public void Calculates_status_from_due_date(int offset,PaymentStatus expected)
    {
        var today=new DateOnly(2026,8,19);var item=Create(today.AddDays(offset));item.RefreshStatus(today);
        Assert.Equal(expected,item.Status);
    }
    [Fact]
    public void Tracks_partial_and_complete_payment()
    {
        var item=Create(new DateOnly(2026,8,20));item.ApplyPayment(new(400,Currency.HNL),DateTimeOffset.UtcNow);
        Assert.Equal(PaymentStatus.PartiallyPaid,item.Status);item.ApplyPayment(new(600,Currency.HNL),DateTimeOffset.UtcNow);
        Assert.Equal(PaymentStatus.Paid,item.Status);
    }
    private static PaymentObligation Create(DateOnly due)=>new(Guid.NewGuid(),null,ObligationType.Other,"Pago",Currency.HNL,1000,500,due,false,DateTimeOffset.UtcNow);
}
