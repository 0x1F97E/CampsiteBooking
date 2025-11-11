using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class EventTests
{
    private Event CreateValidEvent(string title = "Summer BBQ", int maxParticipants = 50)
    {
        return Event.Create(CampsiteId.Create(1), title, "Description", DateTime.UtcNow.AddDays(10), maxParticipants, Money.Create(100m));
    }

    [Fact]
    public void Event_CanBeCreated_WithValidData()
    {
        var evt = CreateValidEvent();
        Assert.NotNull(evt);
        Assert.Equal("Summer BBQ", evt.Title);
        Assert.Equal(50, evt.MaxParticipants);
        Assert.Equal(0, evt.CurrentParticipants);
        Assert.True(evt.IsActive);
    }

    [Fact]
    public void Event_Create_ThrowsException_WhenTitleIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidEvent(title: ""));
    }

    [Fact]
    public void Event_Create_ThrowsException_WhenTitleIsTooLong()
    {
        var longTitle = new string('a', 201);
        Assert.Throws<DomainException>(() => CreateValidEvent(title: longTitle));
    }

    [Fact]
    public void Event_Create_ThrowsException_WhenMaxParticipantsIsZero()
    {
        Assert.Throws<DomainException>(() => CreateValidEvent(maxParticipants: 0));
    }

    [Fact]
    public void Event_Create_ThrowsException_WhenMaxParticipantsExceeds1000()
    {
        Assert.Throws<DomainException>(() => CreateValidEvent(maxParticipants: 1001));
    }

    [Fact]
    public void Event_IsFull_ReturnsTrueWhenFull()
    {
        var evt = CreateValidEvent(maxParticipants: 10);
        evt.RegisterParticipants(10);
        Assert.True(evt.IsFull());
    }

    [Fact]
    public void Event_AvailableSpots_ReturnsCorrectCount()
    {
        var evt = CreateValidEvent(maxParticipants: 50);
        evt.RegisterParticipants(20);
        Assert.Equal(30, evt.AvailableSpots());
    }

    [Fact]
    public void Event_RegisterParticipants_IncreasesCurrentParticipants()
    {
        var evt = CreateValidEvent();
        evt.RegisterParticipants(10);
        Assert.Equal(10, evt.CurrentParticipants);
    }

    [Fact]
    public void Event_RegisterParticipants_ThrowsException_WhenNotEnoughSpots()
    {
        var evt = CreateValidEvent(maxParticipants: 10);
        evt.RegisterParticipants(8);
        Assert.Throws<DomainException>(() => evt.RegisterParticipants(5));
    }

    [Fact]
    public void Event_RegisterParticipants_ThrowsException_WhenInactive()
    {
        var evt = CreateValidEvent();
        evt.Deactivate();
        Assert.Throws<DomainException>(() => evt.RegisterParticipants(5));
    }

    [Fact]
    public void Event_CancelRegistrations_DecreasesCurrentParticipants()
    {
        var evt = CreateValidEvent();
        evt.RegisterParticipants(20);
        evt.CancelRegistrations(5);
        Assert.Equal(15, evt.CurrentParticipants);
    }

    [Fact]
    public void Event_CancelRegistrations_ThrowsException_WhenCancellingMoreThanRegistered()
    {
        var evt = CreateValidEvent();
        evt.RegisterParticipants(10);
        Assert.Throws<DomainException>(() => evt.CancelRegistrations(15));
    }

    [Fact]
    public void Event_Deactivate_SetsIsActiveToFalse()
    {
        var evt = CreateValidEvent();
        evt.Deactivate();
        Assert.False(evt.IsActive);
    }

    [Fact]
    public void Event_Activate_SetsIsActiveToTrue()
    {
        var evt = CreateValidEvent();
        evt.Deactivate();
        evt.Activate();
        Assert.True(evt.IsActive);
    }
}
