using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class PaymentTests
{
    private Payment CreateValidPayment(
        int bookingId = 1,
        decimal amount = 500.0m,
        string currency = "DKK",
        string method = "CreditCard",
        string transactionId = "TXN123")
    {
        return Payment.Create(
            BookingId.Create(bookingId),
            Money.Create(amount, currency),
            method,
            transactionId
        );
    }

    [Fact]
    public void Payment_CanBeCreated_WithValidData()
    {
        var payment = CreateValidPayment();
        Assert.NotNull(payment);
        Assert.Equal(500.0m, payment.Amount.Amount);
        Assert.Equal("DKK", payment.Amount.Currency);
        Assert.Equal("CreditCard", payment.Method);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
    }

    [Fact]
    public void Payment_Create_ThrowsException_WhenMethodIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidPayment(method: ""));
    }

    [Fact]
    public void Payment_Create_ThrowsException_WhenMethodIsInvalid()
    {
        Assert.Throws<DomainException>(() => CreateValidPayment(method: "Invalid"));
    }

    [Theory]
    [InlineData("CreditCard")]
    [InlineData("DebitCard")]
    [InlineData("MobilePay")]
    [InlineData("BankTransfer")]
    [InlineData("Cash")]
    public void Payment_Create_AcceptsValidMethods(string method)
    {
        var payment = CreateValidPayment(method: method);
        Assert.Equal(method, payment.Method);
    }

    [Fact]
    public void Payment_Create_ThrowsException_WhenTransactionIdIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidPayment(transactionId: ""));
    }

    [Fact]
    public void Payment_MarkAsCompleted_ChangesStatusToCompleted()
    {
        var payment = CreateValidPayment();
        payment.MarkAsCompleted();
        Assert.Equal(PaymentStatus.Completed, payment.Status);
        Assert.True(payment.IsCompleted());
    }

    [Fact]
    public void Payment_MarkAsCompleted_ThrowsException_WhenNotPending()
    {
        var payment = CreateValidPayment();
        payment.MarkAsCompleted();
        Assert.Throws<DomainException>(() => payment.MarkAsCompleted());
    }

    [Fact]
    public void Payment_MarkAsFailed_ChangesStatusToFailed()
    {
        var payment = CreateValidPayment();
        payment.MarkAsFailed();
        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.True(payment.IsFailed());
    }

    [Fact]
    public void Payment_MarkAsFailed_ThrowsException_WhenNotPending()
    {
        var payment = CreateValidPayment();
        payment.MarkAsCompleted();
        Assert.Throws<DomainException>(() => payment.MarkAsFailed());
    }

    [Fact]
    public void Payment_ProcessRefund_ChangesStatusToRefunded()
    {
        var payment = CreateValidPayment(amount: 500.0m);
        payment.MarkAsCompleted();
        
        var refundAmount = Money.Create(300.0m, "DKK");
        payment.ProcessRefund(refundAmount);
        
        Assert.Equal(PaymentStatus.Refunded, payment.Status);
        Assert.Equal(300.0m, payment.RefundAmount?.Amount);
        Assert.NotNull(payment.RefundDate);
        Assert.True(payment.IsRefunded());
    }

    [Fact]
    public void Payment_ProcessRefund_ThrowsException_WhenNotCompleted()
    {
        var payment = CreateValidPayment();
        var refundAmount = Money.Create(100.0m, "DKK");
        Assert.Throws<DomainException>(() => payment.ProcessRefund(refundAmount));
    }

    [Fact]
    public void Payment_ProcessRefund_ThrowsException_WhenRefundExceedsOriginalAmount()
    {
        var payment = CreateValidPayment(amount: 500.0m);
        payment.MarkAsCompleted();
        
        var refundAmount = Money.Create(600.0m, "DKK");
        Assert.Throws<DomainException>(() => payment.ProcessRefund(refundAmount));
    }

    [Fact]
    public void Payment_ProcessRefund_ThrowsException_WhenCurrencyMismatch()
    {
        var payment = CreateValidPayment(amount: 500.0m, currency: "DKK");
        payment.MarkAsCompleted();
        
        var refundAmount = Money.Create(100.0m, "EUR");
        Assert.Throws<DomainException>(() => payment.ProcessRefund(refundAmount));
    }

    [Fact]
    public void Payment_IsValidCurrency_ReturnsTrueForValidCurrencies()
    {
        var payment = CreateValidPayment(currency: "DKK");
        Assert.True(payment.IsValidCurrency());
    }

    [Fact]
    public void Payment_IsPending_ReturnsTrueForPendingPayment()
    {
        var payment = CreateValidPayment();
        Assert.True(payment.IsPending());
        Assert.False(payment.IsCompleted());
    }
}

