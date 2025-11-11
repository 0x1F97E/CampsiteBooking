using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class BookingTests
{
    // Helper method to create a valid booking for testing
    private Booking CreateValidBooking(
        int numberOfAdults = 2,
        int numberOfChildren = 0,
        int daysFromNow = 5,
        int numberOfNights = 2)
    {
        var guestId = GuestId.Create(1);
        var campsiteId = CampsiteId.Create(1);
        var accommodationTypeId = AccommodationTypeId.Create(1);
        var checkIn = DateTime.UtcNow.Date.AddDays(daysFromNow);
        var checkOut = checkIn.AddDays(numberOfNights);
        var period = DateRange.Create(checkIn, checkOut);
        var basePrice = Money.Create(100.00m, "DKK");

        return Booking.Create(
            guestId,
            campsiteId,
            accommodationTypeId,
            period,
            basePrice,
            numberOfAdults,
            numberOfChildren
        );
    }

    [Fact]
    public void Booking_CanBeCreated_WithValidData()
    {
        // Arrange & Act
        var booking = CreateValidBooking(numberOfAdults: 2, numberOfChildren: 1);

        // Assert
        Assert.NotNull(booking);
        Assert.Equal(2, booking.NumberOfAdults);
        Assert.Equal(1, booking.NumberOfChildren);
        Assert.Equal(3, booking.GetTotalGuests());
        Assert.Equal(BookingStatus.Pending, booking.Status);
    }

    [Fact]
    public void Booking_Create_ThrowsException_WhenCheckInDateIsInPast()
    {
        // Arrange
        var guestId = GuestId.Create(1);
        var campsiteId = CampsiteId.Create(1);
        var accommodationTypeId = AccommodationTypeId.Create(1);
        var pastDate = DateTime.UtcNow.Date.AddDays(-1);
        var period = DateRange.Create(pastDate, pastDate.AddDays(2));
        var basePrice = Money.Create(100.00m, "DKK");

        // Act & Assert
        Assert.Throws<DomainException>(() => Booking.Create(
            guestId, campsiteId, accommodationTypeId, period, basePrice, 2));
    }

    [Fact]
    public void Booking_Create_ThrowsException_WhenNumberOfAdultsIsZero()
    {
        // Arrange
        var guestId = GuestId.Create(1);
        var campsiteId = CampsiteId.Create(1);
        var accommodationTypeId = AccommodationTypeId.Create(1);
        var checkIn = DateTime.UtcNow.Date.AddDays(5);
        var period = DateRange.Create(checkIn, checkIn.AddDays(2));
        var basePrice = Money.Create(100.00m, "DKK");

        // Act & Assert
        Assert.Throws<DomainException>(() => Booking.Create(
            guestId, campsiteId, accommodationTypeId, period, basePrice, 0));
    }

    [Fact]
    public void Booking_GetTotalGuests_ReturnsCorrectSum()
    {
        // Arrange
        var booking = CreateValidBooking(numberOfAdults: 2, numberOfChildren: 3);

        // Act
        var totalGuests = booking.GetTotalGuests();

        // Assert
        Assert.Equal(5, totalGuests);
    }

    [Fact]
    public void Booking_GetNumberOfNights_ReturnsCorrectValue()
    {
        // Arrange
        var booking = CreateValidBooking(numberOfNights: 3);

        // Act
        var nights = booking.GetNumberOfNights();

        // Assert
        Assert.Equal(3, nights);
    }

    [Fact]
    public void Booking_Confirm_ChangesStatusToConfirmed()
    {
        // Arrange
        var booking = CreateValidBooking();
        var spotId = AccommodationSpotId.Create(1);
        booking.AssignAccommodationSpot(spotId);

        // Act
        booking.Confirm();

        // Assert
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
        Assert.Contains(booking.DomainEvents, e => e is CampsiteBooking.Models.DomainEvents.BookingConfirmedEvent);
    }

    [Fact]
    public void Booking_Confirm_ThrowsException_WhenNoAccommodationSpotAssigned()
    {
        // Arrange
        var booking = CreateValidBooking();

        // Act & Assert
        Assert.Throws<DomainException>(() => booking.Confirm());
    }

    [Fact]
    public void Booking_Complete_ChangesStatusToCompleted()
    {
        // Arrange
        var booking = CreateValidBooking();
        var spotId = AccommodationSpotId.Create(1);
        booking.AssignAccommodationSpot(spotId);
        booking.Confirm();

        // Act
        booking.Complete();

        // Assert
        Assert.Equal(BookingStatus.Completed, booking.Status);
    }

    [Fact]
    public void Booking_Cancel_ChangesStatusToCancelled()
    {
        // Arrange
        var booking = CreateValidBooking();

        // Act
        booking.Cancel();

        // Assert
        Assert.Equal(BookingStatus.Cancelled, booking.Status);
        Assert.NotNull(booking.CancellationDate);
        Assert.Contains(booking.DomainEvents, e => e is CampsiteBooking.Models.DomainEvents.BookingCancelledEvent);
    }

    [Fact]
    public void Booking_Cancel_ThrowsException_WhenAlreadyCompleted()
    {
        // Arrange
        var booking = CreateValidBooking();
        var spotId = AccommodationSpotId.Create(1);
        booking.AssignAccommodationSpot(spotId);
        booking.Confirm();
        booking.Complete();

        // Act & Assert
        Assert.Throws<DomainException>(() => booking.Cancel());
    }

    [Fact]
    public void Booking_Create_ThrowsException_WhenNumberOfChildrenIsNegative()
    {
        // Arrange
        var guestId = GuestId.Create(1);
        var campsiteId = CampsiteId.Create(1);
        var accommodationTypeId = AccommodationTypeId.Create(1);
        var checkIn = DateTime.UtcNow.Date.AddDays(5);
        var period = DateRange.Create(checkIn, checkIn.AddDays(2));
        var basePrice = Money.Create(100.00m, "DKK");

        // Act & Assert
        Assert.Throws<DomainException>(() => Booking.Create(
            guestId, campsiteId, accommodationTypeId, period, basePrice, 2, -1));
    }

    [Fact]
    public void Booking_UpdateTotalPrice_UpdatesPrice()
    {
        // Arrange
        var booking = CreateValidBooking();
        var newPrice = Money.Create(150.00m, "DKK");

        // Act
        booking.UpdateTotalPrice(newPrice);

        // Assert
        Assert.Equal(150.00m, booking.TotalPrice.Amount);
    }

    [Fact]
    public void Booking_UpdateTotalPrice_ThrowsException_WhenBookingIsCancelled()
    {
        // Arrange
        var booking = CreateValidBooking();
        booking.Cancel();
        var newPrice = Money.Create(150.00m, "DKK");

        // Act & Assert
        Assert.Throws<DomainException>(() => booking.UpdateTotalPrice(newPrice));
    }

    [Fact]
    public void Booking_IsActive_ReturnsTrueForPendingBooking()
    {
        // Arrange
        var booking = CreateValidBooking();

        // Act
        var isActive = booking.IsActive();

        // Assert
        Assert.True(isActive);
    }

    [Fact]
    public void Booking_IsActive_ReturnsFalseForCancelledBooking()
    {
        // Arrange
        var booking = CreateValidBooking();
        booking.Cancel();

        // Act
        var isActive = booking.IsActive();

        // Assert
        Assert.False(isActive);
    }
}

