using CampsiteBooking.Models.ValueObjects;
using CampsiteBooking.Models.DomainEvents;

namespace CampsiteBooking.Models;

/// <summary>
/// Payment entity representing a payment transaction
/// </summary>
public class Payment
{
    public int PaymentId { get; set; }

    private int _bookingId;
    public int BookingId
    {
        get => _bookingId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("BookingId must be greater than 0", nameof(BookingId));
            _bookingId = value;
        }
    }

    // Value Object: Money
    private Money? _amount;
    public Money? Amount
    {
        get => _amount;
        set
        {
            if (value != null && value.Amount <= 0)
                throw new ArgumentException("Payment amount must be greater than 0", nameof(Amount));
            _amount = value;
        }
    }

    public string Currency { get; set; } = "DKK"; // DKK, EUR, USD
    public string Method { get; set; } = string.Empty; // CreditCard, DebitCard, MobilePay, BankTransfer

    private string _transactionId = string.Empty;
    public string TransactionId
    {
        get => _transactionId;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("TransactionId cannot be empty", nameof(TransactionId));
            _transactionId = value;
        }
    }

    // Value Object: PaymentStatus
    private PaymentStatus _status = PaymentStatus.Pending;
    public PaymentStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public DateTime? RefundDate { get; set; }

    // Value Object: Money for refund
    private Money? _refundAmount;
    public Money? RefundAmount
    {
        get => _refundAmount;
        set
        {
            if (value != null && Amount != null && value.Amount > Amount.Amount)
                throw new ArgumentException("RefundAmount cannot exceed original Amount", nameof(RefundAmount));
            _refundAmount = value;
        }
    }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Domain Events
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    /// <summary>
    /// Marks the payment as completed
    /// </summary>
    public void MarkAsCompleted()
    {
        if (!Status.CanTransitionTo(PaymentStatus.Completed))
            throw new InvalidOperationException("Only pending payments can be completed");

        Status = PaymentStatus.Completed;
        PaymentDate = DateTime.UtcNow;

        // Raise domain event
        _domainEvents.Add(new PaymentCompletedEvent(
            PaymentId,
            BookingId,
            Amount?.Amount ?? 0m,
            Amount?.Currency ?? "DKK",
            Method
        ));
    }

    /// <summary>
    /// Marks the payment as failed
    /// </summary>
    public void MarkAsFailed()
    {
        if (!Status.CanTransitionTo(PaymentStatus.Failed))
            throw new InvalidOperationException("Only pending payments can be marked as failed");

        Status = PaymentStatus.Failed;
    }

    /// <summary>
    /// Processes a refund
    /// </summary>
    public void ProcessRefund(decimal amount)
    {
        if (!Status.CanTransitionTo(PaymentStatus.Refunded))
            throw new InvalidOperationException("Only completed payments can be refunded");

        if (amount <= 0)
            throw new ArgumentException("Refund amount must be positive", nameof(amount));

        if (Amount != null && amount > Amount.Amount)
            throw new ArgumentException("Refund amount cannot exceed original payment amount", nameof(amount));

        RefundAmount = Money.Create(amount, Amount?.Currency ?? "DKK");
        RefundDate = DateTime.UtcNow;
        Status = PaymentStatus.Refunded;

        // Raise domain event
        _domainEvents.Add(new PaymentRefundedEvent(
            PaymentId,
            BookingId,
            amount,
            Amount?.Currency ?? "DKK"
        ));
    }

    /// <summary>
    /// Validates currency code
    /// </summary>
    public bool IsValidCurrency()
    {
        var validCurrencies = new[] { "DKK", "EUR", "USD", "GBP", "SEK", "NOK" };
        return validCurrencies.Contains(Currency);
    }
}

