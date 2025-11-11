using CampsiteBooking.Models;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class PaymentTests
{
    [Fact]
    public void Payment_Amount_MustBePositive()
    {
        // Arrange
        var payment = new Payment
        {
            BookingId = 1,
            TransactionId = "TXN123"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => payment.Amount = Money.Create(0));
        Assert.Throws<ArgumentException>(() => payment.Amount = Money.Create(-100));
    }

    [Fact]
    public void Payment_RefundAmount_CannotExceedOriginalAmount()
    {
        // Arrange
        var payment = new Payment
        {
            BookingId = 1,
            Amount = Money.Create(500),
            TransactionId = "TXN123"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => payment.RefundAmount = Money.Create(600));
    }

    [Fact]
    public void Payment_RefundAmount_CannotBeNegative()
    {
        // Arrange
        var payment = new Payment
        {
            BookingId = 1,
            Amount = Money.Create(500),
            TransactionId = "TXN123"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => payment.RefundAmount = Money.Create(-100));
    }
    
    [Fact]
    public void Payment_CanTransition_FromPendingToCompleted()
    {
        // Arrange
        var payment = new Payment
        {
            PaymentId = 1,
            BookingId = 1,
            Amount = Money.Create(500),
            TransactionId = "TXN123",
            Status = PaymentStatus.Pending
        };

        // Act
        payment.MarkAsCompleted();

        // Assert
        Assert.Equal(PaymentStatus.Completed, payment.Status);
        Assert.Single(payment.DomainEvents); // Should raise PaymentCompletedEvent
    }

    [Fact]
    public void Payment_CanTransition_FromPendingToFailed()
    {
        // Arrange
        var payment = new Payment
        {
            BookingId = 1,
            Amount = Money.Create(500),
            TransactionId = "TXN123",
            Status = PaymentStatus.Pending
        };

        // Act
        payment.MarkAsFailed();

        // Assert
        Assert.Equal(PaymentStatus.Failed, payment.Status);
    }

    [Fact]
    public void Payment_CannotBeCompleted_WhenNotPending()
    {
        // Arrange
        var payment = new Payment
        {
            BookingId = 1,
            Amount = Money.Create(500),
            TransactionId = "TXN123",
            Status = PaymentStatus.Completed
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => payment.MarkAsCompleted());
    }

    [Fact]
    public void Payment_ProcessRefund_SetsRefundAmountAndDate()
    {
        // Arrange
        var payment = new Payment
        {
            PaymentId = 1,
            BookingId = 1,
            Amount = Money.Create(500),
            TransactionId = "TXN123",
            Status = PaymentStatus.Completed
        };

        // Act
        payment.ProcessRefund(300);

        // Assert
        Assert.Equal(300m, payment.RefundAmount?.Amount);
        Assert.NotNull(payment.RefundDate);
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
        Assert.Single(payment.DomainEvents); // Should raise PaymentRefundedEvent
    }

    [Fact]
    public void Payment_ProcessRefund_ThrowsException_WhenNotCompleted()
    {
        // Arrange
        var payment = new Payment
        {
            BookingId = 1,
            Amount = Money.Create(500),
            TransactionId = "TXN123",
            Status = PaymentStatus.Pending
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => payment.ProcessRefund(300));
    }

    [Fact]
    public void Payment_ProcessRefund_ThrowsException_WhenAmountExceedsOriginal()
    {
        // Arrange
        var payment = new Payment
        {
            BookingId = 1,
            Amount = Money.Create(500),
            TransactionId = "TXN123",
            Status = PaymentStatus.Completed
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => payment.ProcessRefund(600));
    }

    [Theory]
    [InlineData("DKK", true)]
    [InlineData("EUR", true)]
    [InlineData("USD", true)]
    [InlineData("GBP", true)]
    [InlineData("SEK", true)]
    [InlineData("NOK", true)]
    [InlineData("INVALID", false)]
    public void Payment_IsValidCurrency_ValidatesCorrectly(string currency, bool expectedValid)
    {
        // Arrange
        var payment = new Payment
        {
            BookingId = 1,
            Amount = Money.Create(500),
            TransactionId = "TXN123",
            Currency = currency
        };

        // Act
        var isValid = payment.IsValidCurrency();

        // Assert
        Assert.Equal(expectedValid, isValid);
    }

    [Fact]
    public void Payment_TransactionId_CannotBeEmpty()
    {
        // Arrange
        var payment = new Payment
        {
            BookingId = 1,
            Amount = Money.Create(500)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => payment.TransactionId = "");
        Assert.Throws<ArgumentException>(() => payment.TransactionId = " ");
    }
}
