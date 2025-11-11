namespace CampsiteBooking.Models.DomainEvents;

/// <summary>
/// Domain event raised when a booking is cancelled.
/// This can trigger refund processing, availability updates, notifications, etc.
/// </summary>
public class BookingCancelledEvent : IDomainEvent
{
    public int BookingId { get; }
    public int GuestId { get; }
    public string CancellationReason { get; }
    public DateTime OccurredOn { get; }

    public BookingCancelledEvent(int bookingId, int guestId, string cancellationReason = "")
    {
        BookingId = bookingId;
        GuestId = guestId;
        CancellationReason = cancellationReason;
        OccurredOn = DateTime.UtcNow;
    }
}

