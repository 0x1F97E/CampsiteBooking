namespace CampsiteBooking.Models;

/// <summary>
/// Booking entity representing a reservation
/// </summary>
public class Booking
{
    public int BookingId { get; set; }
    
    private int _userId;
    public int UserId
    {
        get => _userId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("UserId must be greater than 0", nameof(UserId));
            _userId = value;
        }
    }
    
    private int _campsiteId;
    public int CampsiteId
    {
        get => _campsiteId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("CampsiteId must be greater than 0", nameof(CampsiteId));
            _campsiteId = value;
        }
    }
    
    public string SpotId { get; set; } = string.Empty;
    
    private int _accommodationTypeId;
    public int AccommodationTypeId
    {
        get => _accommodationTypeId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("AccommodationTypeId must be greater than 0", nameof(AccommodationTypeId));
            _accommodationTypeId = value;
        }
    }
    
    private DateTime _checkInDate;
    public DateTime CheckInDate
    {
        get => _checkInDate;
        set
        {
            if (value < DateTime.UtcNow.Date && BookingId == 0) // Only validate for new bookings
                throw new ArgumentException("CheckInDate cannot be in the past", nameof(CheckInDate));
            _checkInDate = value;
        }
    }
    
    private DateTime _checkOutDate;
    public DateTime CheckOutDate
    {
        get => _checkOutDate;
        set
        {
            if (value <= CheckInDate)
                throw new ArgumentException("CheckOutDate must be after CheckInDate", nameof(CheckOutDate));
            _checkOutDate = value;
        }
    }
    
    private int _numberOfGuests;
    public int NumberOfGuests
    {
        get => _numberOfGuests;
        set
        {
            if (value <= 0)
                throw new ArgumentException("NumberOfGuests must be greater than 0", nameof(NumberOfGuests));
            _numberOfGuests = value;
        }
    }
    
    private int _numberOfChildren;
    public int NumberOfChildren
    {
        get => _numberOfChildren;
        set
        {
            if (value < 0)
                throw new ArgumentException("NumberOfChildren cannot be negative", nameof(NumberOfChildren));
            _numberOfChildren = value;
        }
    }
    
    private int _numberOfAdults;
    public int NumberOfAdults
    {
        get => _numberOfAdults;
        set
        {
            if (value < 0)
                throw new ArgumentException("NumberOfAdults cannot be negative", nameof(NumberOfAdults));
            _numberOfAdults = value;
        }
    }
    
    public decimal BasePrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Refunded
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    public DateTime? CancellationDate { get; set; }
    
    /// <summary>
    /// Validates that NumberOfAdults + NumberOfChildren equals NumberOfGuests
    /// </summary>
    public bool ValidateGuestCount()
    {
        return NumberOfAdults + NumberOfChildren == NumberOfGuests;
    }
    
    /// <summary>
    /// Confirms the booking
    /// </summary>
    public void Confirm()
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Only pending bookings can be confirmed");

        Status = "Confirmed";
        LastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Completes the booking
    /// </summary>
    public void Complete()
    {
        if (Status != "Confirmed")
            throw new InvalidOperationException("Only confirmed bookings can be completed");

        Status = "Completed";
        LastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the booking
    /// </summary>
    public void Cancel()
    {
        if (Status != "Pending" && Status != "Confirmed")
            throw new InvalidOperationException("Only pending or confirmed bookings can be cancelled");

        Status = "Cancelled";
        CancellationDate = DateTime.UtcNow;
        LastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the number of nights
    /// </summary>
    public int GetNumberOfNights()
    {
        return (CheckOutDate - CheckInDate).Days;
    }
}

