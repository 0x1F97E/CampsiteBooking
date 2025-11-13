namespace CampsiteBooking.Controllers;

/// <summary>
/// Data Transfer Object for Booking responses.
/// Used for API responses to decouple domain model from API contract.
/// </summary>
public class BookingDto
{
    public int BookingId { get; set; }
    public int GuestId { get; set; }
    public int CampsiteId { get; set; }
    public int AccommodationTypeId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal BasePriceAmount { get; set; }
    public decimal TotalPriceAmount { get; set; }
    public string Currency { get; set; } = "DKK";
    public int NumberOfAdults { get; set; }
    public int NumberOfChildren { get; set; }
    public string SpecialRequests { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Request DTO for creating a new booking.
/// </summary>
public class CreateBookingRequest
{
    public int GuestId { get; set; }
    public int CampsiteId { get; set; }
    public int AccommodationTypeId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal BasePriceAmount { get; set; }
    public string? Currency { get; set; }
    public int NumberOfAdults { get; set; }
    public int NumberOfChildren { get; set; }
    public string? SpecialRequests { get; set; }
}

/// <summary>
/// Request DTO for updating an existing booking.
/// Simple version - only allows updating special requests.
/// </summary>
public class UpdateBookingRequest
{
    public string? SpecialRequests { get; set; }
}

