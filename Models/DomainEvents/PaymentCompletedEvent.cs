namespace CampsiteBooking.Models.DomainEvents;

/// <summary>
/// Domain event raised when a payment is successfully completed.
/// This can trigger booking confirmation, receipt generation, notifications, etc.
/// </summary>
public class PaymentCompletedEvent : IDomainEvent
{
    public int PaymentId { get; }
    public int BookingId { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public string PaymentMethod { get; }
    public DateTime OccurredOn { get; }

    public PaymentCompletedEvent(int paymentId, int bookingId, decimal amount, string currency, string paymentMethod)
    {
        PaymentId = paymentId;
        BookingId = bookingId;
        Amount = amount;
        Currency = currency;
        PaymentMethod = paymentMethod;
        OccurredOn = DateTime.UtcNow;
    }
}

