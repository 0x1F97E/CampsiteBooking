namespace CampsiteBooking.Models.DomainEvents;

/// <summary>
/// Domain event raised when a new booking is created
/// </summary>
public class BookingCreatedEvent : IDomainEvent
{
    public int BookingId { get; }
    public int GuestId { get; }
    public int CampsiteId { get; }
    public DateTime CheckInDate { get; }
    public DateTime CheckOutDate { get; }
    public DateTime OccurredOn { get; }

    public BookingCreatedEvent(int bookingId, int guestId, int campsiteId, DateTime checkInDate, DateTime checkOutDate)
    {
        BookingId = bookingId;
        GuestId = guestId;
        CampsiteId = campsiteId;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        OccurredOn = DateTime.UtcNow;
    }
}

