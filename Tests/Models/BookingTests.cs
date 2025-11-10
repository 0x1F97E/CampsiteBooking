using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class BookingTests
{
    [Fact]
    public void Booking_CheckOutDate_MustBeAfterCheckInDate()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            CampsiteId = 1,
            AccommodationTypeId = 1,
            CheckInDate = DateTime.UtcNow.AddDays(5)
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => booking.CheckOutDate = DateTime.UtcNow.AddDays(3));
    }
    
    [Fact]
    public void Booking_NumberOfGuests_MustBeGreaterThanZero()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            CampsiteId = 1,
            AccommodationTypeId = 1
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => booking.NumberOfGuests = 0);
        Assert.Throws<ArgumentException>(() => booking.NumberOfGuests = -1);
    }
    
    [Fact]
    public void Booking_ValidateGuestCount_ReturnsTrue_WhenAdultsAndChildrenMatchTotal()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            CampsiteId = 1,
            AccommodationTypeId = 1,
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(7),
            NumberOfGuests = 4,
            NumberOfAdults = 2,
            NumberOfChildren = 2
        };
        
        // Act
        var isValid = booking.ValidateGuestCount();
        
        // Assert
        Assert.True(isValid);
    }
    
    [Fact]
    public void Booking_ValidateGuestCount_ReturnsFalse_WhenMismatch()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            CampsiteId = 1,
            AccommodationTypeId = 1,
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(7),
            NumberOfGuests = 5,
            NumberOfAdults = 2,
            NumberOfChildren = 2
        };
        
        // Act
        var isValid = booking.ValidateGuestCount();
        
        // Assert
        Assert.False(isValid);
    }
    
    [Fact]
    public void Booking_CanTransition_FromPendingToConfirmed()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            CampsiteId = 1,
            AccommodationTypeId = 1,
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(7),
            NumberOfGuests = 2,
            Status = "Pending"
        };
        
        // Act
        booking.Confirm();
        
        // Assert
        Assert.Equal("Confirmed", booking.Status);
    }
    
    [Fact]
    public void Booking_CanTransition_FromConfirmedToCompleted()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            CampsiteId = 1,
            AccommodationTypeId = 1,
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(7),
            NumberOfGuests = 2,
            Status = "Confirmed"
        };
        
        // Act
        booking.Complete();
        
        // Assert
        Assert.Equal("Completed", booking.Status);
    }
    
    [Fact]
    public void Booking_CanBeCancelled_WhenPendingOrConfirmed()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            CampsiteId = 1,
            AccommodationTypeId = 1,
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(7),
            NumberOfGuests = 2,
            Status = "Pending"
        };
        
        // Act
        booking.Cancel();
        
        // Assert
        Assert.Equal("Cancelled", booking.Status);
        Assert.NotNull(booking.CancellationDate);
    }

    [Fact]
    public void Booking_CannotBeCancelled_WhenCompleted()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            CampsiteId = 1,
            AccommodationTypeId = 1,
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(7),
            NumberOfGuests = 2,
            Status = "Completed"
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => booking.Cancel());
    }

    [Fact]
    public void Booking_GetNumberOfNights_CalculatesCorrectly()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            CampsiteId = 1,
            AccommodationTypeId = 1,
            CheckInDate = DateTime.UtcNow.AddDays(5),
            CheckOutDate = DateTime.UtcNow.AddDays(8),
            NumberOfGuests = 2
        };

        // Act
        var nights = booking.GetNumberOfNights();

        // Assert
        Assert.Equal(3, nights);
    }

    [Fact]
    public void Booking_NumberOfChildren_CannotBeNegative()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            CampsiteId = 1,
            AccommodationTypeId = 1
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => booking.NumberOfChildren = -1);
    }

    [Fact]
    public void Booking_NumberOfAdults_CannotBeNegative()
    {
        // Arrange
        var booking = new Booking
        {
            UserId = 1,
            CampsiteId = 1,
            AccommodationTypeId = 1
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => booking.NumberOfAdults = -1);
    }
}

