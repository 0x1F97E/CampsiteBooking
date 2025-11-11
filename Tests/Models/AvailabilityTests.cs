using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class AvailabilityTests
{
    [Fact]
    public void CampsiteId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var availability = new Availability();

        // Act
        availability.CampsiteId = 5;

        // Assert
        Assert.Equal(5, availability.CampsiteId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CampsiteId_InvalidValue_ThrowsArgumentException(int invalidId)
    {
        // Arrange
        var availability = new Availability();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => availability.CampsiteId = invalidId);
    }

    [Fact]
    public void AccommodationTypeId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var availability = new Availability { CampsiteId = 1 };

        // Act
        availability.AccommodationTypeId = 3;

        // Assert
        Assert.Equal(3, availability.AccommodationTypeId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AccommodationTypeId_InvalidValue_ThrowsArgumentException(int invalidId)
    {
        // Arrange
        var availability = new Availability { CampsiteId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => availability.AccommodationTypeId = invalidId);
    }

    [Fact]
    public void Date_FutureDate_SetsCorrectly()
    {
        // Arrange
        var availability = new Availability { CampsiteId = 1, AccommodationTypeId = 1 };
        var futureDate = DateTime.UtcNow.AddDays(7);

        // Act
        availability.Date = futureDate;

        // Assert
        Assert.Equal(futureDate.Date, availability.Date);
    }

    [Fact]
    public void Date_PastDate_ThrowsArgumentException()
    {
        // Arrange
        var availability = new Availability { CampsiteId = 1, AccommodationTypeId = 1 };
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => availability.Date = pastDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(100)]
    public void AvailableUnits_ValidValue_SetsCorrectly(int validUnits)
    {
        // Arrange
        var availability = new Availability { CampsiteId = 1, AccommodationTypeId = 1, Date = DateTime.UtcNow };

        // Act
        availability.AvailableUnits = validUnits;

        // Assert
        Assert.Equal(validUnits, availability.AvailableUnits);
    }

    [Fact]
    public void AvailableUnits_NegativeValue_ThrowsArgumentException()
    {
        // Arrange
        var availability = new Availability { CampsiteId = 1, AccommodationTypeId = 1, Date = DateTime.UtcNow };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => availability.AvailableUnits = -1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(100)]
    public void ReservedUnits_ValidValue_SetsCorrectly(int validUnits)
    {
        // Arrange
        var availability = new Availability { CampsiteId = 1, AccommodationTypeId = 1, Date = DateTime.UtcNow };

        // Act
        availability.ReservedUnits = validUnits;

        // Assert
        Assert.Equal(validUnits, availability.ReservedUnits);
    }

    [Fact]
    public void ReservedUnits_NegativeValue_ThrowsArgumentException()
    {
        // Arrange
        var availability = new Availability { CampsiteId = 1, AccommodationTypeId = 1, Date = DateTime.UtcNow };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => availability.ReservedUnits = -1);
    }

    [Fact]
    public void ReserveUnits_ValidCount_UpdatesUnitsCorrectly()
    {
        // Arrange
        var availability = new Availability 
        { 
            CampsiteId = 1, 
            AccommodationTypeId = 1, 
            Date = DateTime.UtcNow,
            AvailableUnits = 10,
            ReservedUnits = 0
        };

        // Act
        availability.ReserveUnits(3);

        // Assert
        Assert.Equal(7, availability.AvailableUnits);
        Assert.Equal(3, availability.ReservedUnits);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ReserveUnits_InvalidCount_ThrowsArgumentException(int invalidCount)
    {
        // Arrange
        var availability = new Availability
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            Date = DateTime.UtcNow,
            AvailableUnits = 10
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => availability.ReserveUnits(invalidCount));
    }

    [Fact]
    public void ReserveUnits_ExceedsAvailable_ThrowsInvalidOperationException()
    {
        // Arrange
        var availability = new Availability
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            Date = DateTime.UtcNow,
            AvailableUnits = 5
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => availability.ReserveUnits(10));
    }

    [Fact]
    public void ReleaseUnits_ValidCount_UpdatesUnitsCorrectly()
    {
        // Arrange
        var availability = new Availability
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            Date = DateTime.UtcNow,
            AvailableUnits = 5,
            ReservedUnits = 5
        };

        // Act
        availability.ReleaseUnits(3);

        // Assert
        Assert.Equal(8, availability.AvailableUnits);
        Assert.Equal(2, availability.ReservedUnits);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ReleaseUnits_InvalidCount_ThrowsArgumentException(int invalidCount)
    {
        // Arrange
        var availability = new Availability
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            Date = DateTime.UtcNow,
            ReservedUnits = 5
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => availability.ReleaseUnits(invalidCount));
    }

    [Fact]
    public void ReleaseUnits_ExceedsReserved_ThrowsInvalidOperationException()
    {
        // Arrange
        var availability = new Availability
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            Date = DateTime.UtcNow,
            ReservedUnits = 3
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => availability.ReleaseUnits(5));
    }

    [Fact]
    public void GetTotalCapacity_ReturnsCorrectSum()
    {
        // Arrange
        var availability = new Availability
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            Date = DateTime.UtcNow,
            AvailableUnits = 7,
            ReservedUnits = 3
        };

        // Act
        var total = availability.GetTotalCapacity();

        // Assert
        Assert.Equal(10, total);
    }

    [Theory]
    [InlineData(5, true)]
    [InlineData(10, true)]
    [InlineData(11, false)]
    public void HasAvailability_ReturnsCorrectResult(int requestedUnits, bool expected)
    {
        // Arrange
        var availability = new Availability
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            Date = DateTime.UtcNow,
            AvailableUnits = 10
        };

        // Act
        var result = availability.HasAvailability(requestedUnits);

        // Assert
        Assert.Equal(expected, result);
    }
}

