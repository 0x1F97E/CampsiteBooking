namespace CampsiteBooking.Models.DomainEvents;

/// <summary>
/// Domain event raised when a payment is refunded.
/// This can trigger notifications, accounting updates, etc.
/// </summary>
public class PaymentRefundedEvent : IDomainEvent
{
    public int PaymentId { get; }
    public int BookingId { get; }
    public decimal RefundAmount { get; }
    public string Currency { get; }
    public string Reason { get; }
    public DateTime OccurredOn { get; }

    public PaymentRefundedEvent(int paymentId, int bookingId, decimal refundAmount, string currency, string reason = "")
    {
        PaymentId = paymentId;
        BookingId = bookingId;
        RefundAmount = refundAmount;
        Currency = currency;
        Reason = reason;
        OccurredOn = DateTime.UtcNow;
    }
}

