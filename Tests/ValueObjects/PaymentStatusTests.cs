using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.ValueObjects;

public class PaymentStatusTests
{
    [Theory]
    [InlineData("Pending")]
    [InlineData("Completed")]
    [InlineData("Failed")]
    [InlineData("Refunded")]
    public void FromString_ValidStatus_ReturnsCorrectStatus(string statusValue)
    {
        // Act
        var status = PaymentStatus.FromString(statusValue);

        // Assert
        Assert.Equal(statusValue, status.Value);
    }

    [Fact]
    public void FromString_InvalidStatus_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => PaymentStatus.FromString("Invalid"));
    }

    [Theory]
    [InlineData("Completed", true)]
    [InlineData("Failed", true)]
    public void CanTransitionTo_FromPending_ReturnsCorrectResult(string targetStatus, bool expected)
    {
        // Arrange
        var status = PaymentStatus.Pending;
        var target = PaymentStatus.FromString(targetStatus);

        // Act
        var result = status.CanTransitionTo(target);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CanTransitionTo_FromCompletedToRefunded_ReturnsTrue()
    {
        // Arrange
        var status = PaymentStatus.Completed;

        // Act
        var result = status.CanTransitionTo(PaymentStatus.Refunded);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanTransitionTo_FromFailedToPending_ReturnsTrue()
    {
        // Arrange
        var status = PaymentStatus.Failed;

        // Act
        var result = status.CanTransitionTo(PaymentStatus.Pending);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanTransitionTo_FromRefunded_ReturnsFalse()
    {
        // Arrange
        var status = PaymentStatus.Refunded;

        // Act
        var canPending = status.CanTransitionTo(PaymentStatus.Pending);
        var canCompleted = status.CanTransitionTo(PaymentStatus.Completed);

        // Assert
        Assert.False(canPending);
        Assert.False(canCompleted);
    }

    [Fact]
    public void IsPending_PendingStatus_ReturnsTrue()
    {
        // Arrange
        var status = PaymentStatus.Pending;

        // Assert
        Assert.True(status.IsPending);
        Assert.False(status.IsCompleted);
        Assert.False(status.IsFailed);
        Assert.False(status.IsRefunded);
    }

    [Fact]
    public void IsSuccessful_CompletedOrRefunded_ReturnsTrue()
    {
        // Assert
        Assert.False(PaymentStatus.Pending.IsSuccessful);
        Assert.True(PaymentStatus.Completed.IsSuccessful);
        Assert.False(PaymentStatus.Failed.IsSuccessful);
        Assert.True(PaymentStatus.Refunded.IsSuccessful);
    }

    [Fact]
    public void IsFinal_CompletedOrRefunded_ReturnsTrue()
    {
        // Assert
        Assert.False(PaymentStatus.Pending.IsFinal);
        Assert.True(PaymentStatus.Completed.IsFinal);
        Assert.False(PaymentStatus.Failed.IsFinal);
        Assert.True(PaymentStatus.Refunded.IsFinal);
    }

    [Fact]
    public void Equals_SameStatus_ReturnsTrue()
    {
        // Arrange
        var status1 = PaymentStatus.Completed;
        var status2 = PaymentStatus.FromString("Completed");

        // Act & Assert
        Assert.Equal(status1, status2);
        Assert.True(status1 == status2);
    }

    [Fact]
    public void Equals_DifferentStatus_ReturnsFalse()
    {
        // Arrange
        var status1 = PaymentStatus.Pending;
        var status2 = PaymentStatus.Completed;

        // Act & Assert
        Assert.NotEqual(status1, status2);
        Assert.True(status1 != status2);
    }

    [Fact]
    public void ToString_ReturnsStatusValue()
    {
        // Arrange
        var status = PaymentStatus.Completed;

        // Act
        var result = status.ToString();

        // Assert
        Assert.Equal("Completed", result);
    }

    [Fact]
    public void ImplicitConversion_ToString_Works()
    {
        // Arrange
        var status = PaymentStatus.Pending;

        // Act
        string statusString = status;

        // Assert
        Assert.Equal("Pending", statusString);
    }
}

