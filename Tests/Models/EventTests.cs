using CampsiteBooking.Models;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class EventTests
{
    [Fact]
    public void Event_CanBeCreated_WithValidData()
    {
        // Arrange & Act
        var evt = new Event
        {
            CampsiteId = 1,
            Title = "Summer BBQ",
            Description = "Annual summer barbecue event",
            EventDate = DateTime.UtcNow.Date.AddDays(7),
            MaxParticipants = 50,
            CurrentParticipants = 0,
            Price = Money.Create(150, "DKK"),
            IsActive = true
        };

        // Assert
        Assert.Equal(1, evt.CampsiteId);
        Assert.Equal("Summer BBQ", evt.Title);
        Assert.Equal("Annual summer barbecue event", evt.Description);
        Assert.Equal(50, evt.MaxParticipants);
        Assert.Equal(0, evt.CurrentParticipants);
        Assert.Equal(150m, evt.Price?.Amount);
        Assert.True(evt.IsActive);
    }

    [Fact]
    public void Event_CampsiteId_MustBePositive()
    {
        // Arrange
        var evt = new Event();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => evt.CampsiteId = 0);
        Assert.Throws<ArgumentException>(() => evt.CampsiteId = -1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Event_Title_CannotBeEmpty(string invalidTitle)
    {
        // Arrange
        var evt = new Event();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => evt.Title = invalidTitle);
    }

    [Fact]
    public void Event_Title_CannotExceed200Characters()
    {
        // Arrange
        var evt = new Event();
        var longTitle = new string('A', 201);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => evt.Title = longTitle);
    }

    [Fact]
    public void Event_EventDate_CannotBeInPast()
    {
        // Arrange
        var evt = new Event();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => evt.EventDate = DateTime.UtcNow.Date.AddDays(-1));
    }

    [Fact]
    public void Event_MaxParticipants_MustBePositive()
    {
        // Arrange
        var evt = new Event();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => evt.MaxParticipants = 0);
        Assert.Throws<ArgumentException>(() => evt.MaxParticipants = -10);
    }

    [Fact]
    public void Event_MaxParticipants_CannotExceed1000()
    {
        // Arrange
        var evt = new Event();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => evt.MaxParticipants = 1001);
    }

    [Fact]
    public void Event_CurrentParticipants_CannotBeNegative()
    {
        // Arrange
        var evt = new Event { MaxParticipants = 50 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => evt.CurrentParticipants = -1);
    }

    [Fact]
    public void Event_CurrentParticipants_CannotExceedMaxParticipants()
    {
        // Arrange
        var evt = new Event { MaxParticipants = 50 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => evt.CurrentParticipants = 51);
    }

    [Fact]
    public void Event_IsFull_ReturnsTrueWhenFull()
    {
        // Arrange
        var evt = new Event
        {
            MaxParticipants = 50,
            CurrentParticipants = 50
        };

        // Act & Assert
        Assert.True(evt.IsFull());
    }

    [Fact]
    public void Event_IsFull_ReturnsFalseWhenNotFull()
    {
        // Arrange
        var evt = new Event
        {
            MaxParticipants = 50,
            CurrentParticipants = 30
        };

        // Act & Assert
        Assert.False(evt.IsFull());
    }

    [Fact]
    public void Event_AvailableSpots_ReturnsCorrectCount()
    {
        // Arrange
        var evt = new Event
        {
            MaxParticipants = 50,
            CurrentParticipants = 30
        };

        // Act
        var available = evt.AvailableSpots();

        // Assert
        Assert.Equal(20, available);
    }

    [Fact]
    public void Event_RegisterParticipants_IncreasesCount()
    {
        // Arrange
        var evt = new Event
        {
            CampsiteId = 1,
            Title = "Test Event",
            EventDate = DateTime.UtcNow.Date.AddDays(7),
            MaxParticipants = 50,
            CurrentParticipants = 10,
            IsActive = true
        };

        // Act
        evt.RegisterParticipants(5);

        // Assert
        Assert.Equal(15, evt.CurrentParticipants);
    }

    [Fact]
    public void Event_RegisterParticipants_ThrowsWhenCountIsZeroOrNegative()
    {
        // Arrange
        var evt = new Event
        {
            CampsiteId = 1,
            Title = "Test Event",
            EventDate = DateTime.UtcNow.Date.AddDays(7),
            MaxParticipants = 50,
            IsActive = true
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => evt.RegisterParticipants(0));
        Assert.Throws<ArgumentException>(() => evt.RegisterParticipants(-5));
    }

    [Fact]
    public void Event_RegisterParticipants_ThrowsWhenEventIsInactive()
    {
        // Arrange
        var evt = new Event
        {
            CampsiteId = 1,
            Title = "Test Event",
            EventDate = DateTime.UtcNow.Date.AddDays(7),
            MaxParticipants = 50,
            IsActive = false
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => evt.RegisterParticipants(5));
    }

    [Fact]
    public void Event_RegisterParticipants_ThrowsWhenNotEnoughSpots()
    {
        // Arrange
        var evt = new Event
        {
            CampsiteId = 1,
            Title = "Test Event",
            EventDate = DateTime.UtcNow.Date.AddDays(7),
            MaxParticipants = 50,
            CurrentParticipants = 48,
            IsActive = true
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => evt.RegisterParticipants(5));
    }

    [Fact]
    public void Event_CancelRegistrations_DecreasesCount()
    {
        // Arrange
        var evt = new Event
        {
            CampsiteId = 1,
            Title = "Test Event",
            EventDate = DateTime.UtcNow.Date.AddDays(7),
            MaxParticipants = 50,
            CurrentParticipants = 20
        };

        // Act
        evt.CancelRegistrations(5);

        // Assert
        Assert.Equal(15, evt.CurrentParticipants);
    }

    [Fact]
    public void Event_CancelRegistrations_ThrowsWhenCountIsZeroOrNegative()
    {
        // Arrange
        var evt = new Event
        {
            CampsiteId = 1,
            Title = "Test Event",
            EventDate = DateTime.UtcNow.Date.AddDays(7),
            MaxParticipants = 50,
            CurrentParticipants = 20
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => evt.CancelRegistrations(0));
        Assert.Throws<ArgumentException>(() => evt.CancelRegistrations(-5));
    }

    [Fact]
    public void Event_CancelRegistrations_ThrowsWhenCancellingMoreThanRegistered()
    {
        // Arrange
        var evt = new Event
        {
            CampsiteId = 1,
            Title = "Test Event",
            EventDate = DateTime.UtcNow.Date.AddDays(7),
            MaxParticipants = 50,
            CurrentParticipants = 10
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => evt.CancelRegistrations(15));
    }

    [Fact]
    public void Event_Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var evt = new Event
        {
            CampsiteId = 1,
            Title = "Test Event",
            EventDate = DateTime.UtcNow.Date.AddDays(7),
            MaxParticipants = 50,
            IsActive = true
        };

        // Act
        evt.Deactivate();

        // Assert
        Assert.False(evt.IsActive);
    }

    [Fact]
    public void Event_Activate_SetsIsActiveToTrue()
    {
        // Arrange
        var evt = new Event
        {
            CampsiteId = 1,
            Title = "Test Event",
            EventDate = DateTime.UtcNow.Date.AddDays(7),
            MaxParticipants = 50,
            IsActive = false
        };

        // Act
        evt.Activate();

        // Assert
        Assert.True(evt.IsActive);
    }
}
