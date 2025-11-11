namespace CampsiteBooking.Models.DomainEvents;

/// <summary>
/// Domain event raised when a booking is confirmed.
/// This can trigger notifications, update availability, etc.
/// </summary>
public class BookingConfirmedEvent : IDomainEvent
{
    public int BookingId { get; }
    public int GuestId { get; }
    public DateTime CheckInDate { get; }
    public DateTime CheckOutDate { get; }
    public DateTime OccurredOn { get; }

    public BookingConfirmedEvent(int bookingId, int guestId, DateTime checkInDate, DateTime checkOutDate)
    {
        BookingId = bookingId;
        GuestId = guestId;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        OccurredOn = DateTime.UtcNow;
    }
}

