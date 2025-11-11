using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// Payment entity representing a payment transaction
/// Part of the Booking aggregate
/// </summary>
public class Payment : Entity<PaymentId>
{
    // ============================================================================
    // PRIVATE FIELDS
    // ============================================================================
    
    private BookingId _bookingId = null!;
    private Money _amount = null!;
    private string _method = string.Empty;
    private string _transactionId = string.Empty;
    private PaymentStatus _status = PaymentStatus.Pending;
    private DateTime _paymentDate;
    private DateTime? _refundDate;
    private Money? _refundAmount;
    private DateTime _createdDate;
    
    // ============================================================================
    // PUBLIC PROPERTIES (Read-only)
    // ============================================================================
    
    public BookingId BookingId => _bookingId;
    public Money Amount => _amount;
    public string Method => _method;
    public string TransactionId => _transactionId;
    public PaymentStatus Status => _status;
    public DateTime PaymentDate => _paymentDate;
    public DateTime? RefundDate => _refundDate;
    public Money? RefundAmount => _refundAmount;
    public DateTime CreatedDate => _createdDate;
    
    // ============================================================================
    // LEGACY PROPERTIES (for EF Core backward compatibility)
    // ============================================================================
    
    public int PaymentId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.PaymentId.Create(value) : ValueObjects.PaymentId.CreateNew();
    }
    
    public string Currency
    {
        get => _amount?.Currency ?? "DKK";
        set { } // Ignored - currency is part of Money value object
    }
    
    // ============================================================================
    // FACTORY METHOD
    // ============================================================================
    
    public static Payment Create(
        BookingId bookingId,
        Money amount,
        string method,
        string transactionId)
    {
        // Validate business rules
        if (string.IsNullOrWhiteSpace(method))
            throw new DomainException("Payment method cannot be empty");
        
        var validMethods = new[] { "CreditCard", "DebitCard", "MobilePay", "BankTransfer", "Cash" };
        if (!validMethods.Contains(method))
            throw new DomainException("Payment method must be CreditCard, DebitCard, MobilePay, BankTransfer, or Cash");
        
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new DomainException("Transaction ID cannot be empty");
        
        var payment = new Payment
        {
            Id = ValueObjects.PaymentId.CreateNew(),
            _bookingId = bookingId,
            _amount = amount,
            _method = method,
            _transactionId = transactionId.Trim(),
            _status = PaymentStatus.Pending,
            _paymentDate = DateTime.UtcNow,
            _createdDate = DateTime.UtcNow
        };
        
        return payment;
    }
    
    // ============================================================================
    // CONSTRUCTORS
    // ============================================================================
    
    /// <summary>
    /// Protected constructor for EF Core
    /// </summary>
    private Payment()
    {
    }
    
    // ============================================================================
    // DOMAIN BEHAVIOR
    // ============================================================================
    
    public void MarkAsCompleted()
    {
        if (!_status.CanTransitionTo(PaymentStatus.Completed))
            throw new DomainException("Only pending payments can be completed");
        
        _status = PaymentStatus.Completed;
        _paymentDate = DateTime.UtcNow;
    }
    
    public void MarkAsFailed()
    {
        if (!_status.CanTransitionTo(PaymentStatus.Failed))
            throw new DomainException("Only pending payments can be marked as failed");
        
        _status = PaymentStatus.Failed;
    }
    
    public void ProcessRefund(Money refundAmount)
    {
        if (!_status.CanTransitionTo(PaymentStatus.Refunded))
            throw new DomainException("Only completed payments can be refunded");
        
        if (refundAmount.Amount > _amount.Amount)
            throw new DomainException("Refund amount cannot exceed original payment amount");
        
        if (refundAmount.Currency != _amount.Currency)
            throw new DomainException("Refund currency must match original payment currency");
        
        _refundAmount = refundAmount;
        _refundDate = DateTime.UtcNow;
        _status = PaymentStatus.Refunded;
    }
    
    public bool IsValidCurrency()
    {
        var validCurrencies = new[] { "DKK", "EUR", "USD", "GBP", "SEK", "NOK" };
        return validCurrencies.Contains(_amount.Currency);
    }
    
    public bool IsCompleted() => _status == PaymentStatus.Completed;
    public bool IsPending() => _status == PaymentStatus.Pending;
    public bool IsFailed() => _status == PaymentStatus.Failed;
    public bool IsRefunded() => _status == PaymentStatus.Refunded;
}

