using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.ValueObjects;

public class BookingStatusTests
{
    [Theory]
    [InlineData("Pending")]
    [InlineData("Confirmed")]
    [InlineData("Completed")]
    [InlineData("Cancelled")]
    public void FromString_ValidStatus_ReturnsCorrectStatus(string statusValue)
    {
        // Act
        var status = BookingStatus.FromString(statusValue);

        // Assert
        Assert.Equal(statusValue, status.Value);
    }

    [Fact]
    public void FromString_InvalidStatus_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => BookingStatus.FromString("Invalid"));
    }

    [Theory]
    [InlineData("Confirmed", true)]
    [InlineData("Cancelled", true)]
    public void CanTransitionTo_FromPending_ReturnsCorrectResult(string targetStatus, bool expected)
    {
        // Arrange
        var status = BookingStatus.Pending;
        var target = BookingStatus.FromString(targetStatus);

        // Act
        var result = status.CanTransitionTo(target);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CanTransitionTo_FromPendingToCompleted_ReturnsFalse()
    {
        // Arrange
        var status = BookingStatus.Pending;

        // Act
        var result = status.CanTransitionTo(BookingStatus.Completed);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("Completed", true)]
    [InlineData("Cancelled", true)]
    public void CanTransitionTo_FromConfirmed_ReturnsCorrectResult(string targetStatus, bool expected)
    {
        // Arrange
        var status = BookingStatus.Confirmed;
        var target = BookingStatus.FromString(targetStatus);

        // Act
        var result = status.CanTransitionTo(target);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CanTransitionTo_FromCompleted_ReturnsFalse()
    {
        // Arrange
        var status = BookingStatus.Completed;

        // Act
        var canConfirm = status.CanTransitionTo(BookingStatus.Confirmed);
        var canCancel = status.CanTransitionTo(BookingStatus.Cancelled);

        // Assert
        Assert.False(canConfirm);
        Assert.False(canCancel);
    }

    [Fact]
    public void CanTransitionTo_FromCancelled_ReturnsFalse()
    {
        // Arrange
        var status = BookingStatus.Cancelled;

        // Act
        var canConfirm = status.CanTransitionTo(BookingStatus.Confirmed);
        var canComplete = status.CanTransitionTo(BookingStatus.Completed);

        // Assert
        Assert.False(canConfirm);
        Assert.False(canComplete);
    }

    [Fact]
    public void IsPending_PendingStatus_ReturnsTrue()
    {
        // Arrange
        var status = BookingStatus.Pending;

        // Assert
        Assert.True(status.IsPending);
        Assert.False(status.IsConfirmed);
        Assert.False(status.IsCompleted);
        Assert.False(status.IsCancelled);
    }

    [Fact]
    public void IsActive_PendingOrConfirmed_ReturnsTrue()
    {
        // Assert
        Assert.True(BookingStatus.Pending.IsActive);
        Assert.True(BookingStatus.Confirmed.IsActive);
        Assert.False(BookingStatus.Completed.IsActive);
        Assert.False(BookingStatus.Cancelled.IsActive);
    }

    [Fact]
    public void IsFinal_CompletedOrCancelled_ReturnsTrue()
    {
        // Assert
        Assert.False(BookingStatus.Pending.IsFinal);
        Assert.False(BookingStatus.Confirmed.IsFinal);
        Assert.True(BookingStatus.Completed.IsFinal);
        Assert.True(BookingStatus.Cancelled.IsFinal);
    }

    [Fact]
    public void Equals_SameStatus_ReturnsTrue()
    {
        // Arrange
        var status1 = BookingStatus.Pending;
        var status2 = BookingStatus.FromString("Pending");

        // Act & Assert
        Assert.Equal(status1, status2);
        Assert.True(status1 == status2);
    }

    [Fact]
    public void Equals_DifferentStatus_ReturnsFalse()
    {
        // Arrange
        var status1 = BookingStatus.Pending;
        var status2 = BookingStatus.Confirmed;

        // Act & Assert
        Assert.NotEqual(status1, status2);
        Assert.True(status1 != status2);
    }

    [Fact]
    public void ToString_ReturnsStatusValue()
    {
        // Arrange
        var status = BookingStatus.Confirmed;

        // Act
        var result = status.ToString();

        // Assert
        Assert.Equal("Confirmed", result);
    }

    [Fact]
    public void ImplicitConversion_ToString_Works()
    {
        // Arrange
        var status = BookingStatus.Pending;

        // Act
        string statusString = status;

        // Assert
        Assert.Equal("Pending", statusString);
    }
}

