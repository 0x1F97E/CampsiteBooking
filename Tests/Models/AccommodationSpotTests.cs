using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class AccommodationSpotTests
{
    [Fact]
    public void SpotId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var spot = new AccommodationSpot();

        // Act
        spot.SpotId = "A-101";

        // Assert
        Assert.Equal("A-101", spot.SpotId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SpotId_EmptyValue_ThrowsArgumentException(string invalidSpotId)
    {
        // Arrange
        var spot = new AccommodationSpot();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => spot.SpotId = invalidSpotId);
    }

    [Fact]
    public void CampsiteId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101" };

        // Act
        spot.CampsiteId = 5;

        // Assert
        Assert.Equal(5, spot.CampsiteId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CampsiteId_InvalidValue_ThrowsArgumentException(int invalidCampsiteId)
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => spot.CampsiteId = invalidCampsiteId);
    }

    [Fact]
    public void AccommodationTypeId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101", CampsiteId = 1 };

        // Act
        spot.AccommodationTypeId = 3;

        // Assert
        Assert.Equal(3, spot.AccommodationTypeId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void AccommodationTypeId_InvalidValue_ThrowsArgumentException(int invalidTypeId)
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101", CampsiteId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => spot.AccommodationTypeId = invalidTypeId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(45.5)]
    [InlineData(-45.5)]
    [InlineData(90)]
    [InlineData(-90)]
    public void Latitude_ValidValue_SetsCorrectly(double validLatitude)
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101", CampsiteId = 1, AccommodationTypeId = 1 };

        // Act
        spot.Latitude = validLatitude;

        // Assert
        Assert.Equal(validLatitude, spot.Latitude);
    }

    [Theory]
    [InlineData(91)]
    [InlineData(-91)]
    [InlineData(100)]
    [InlineData(-100)]
    public void Latitude_InvalidValue_ThrowsArgumentException(double invalidLatitude)
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101", CampsiteId = 1, AccommodationTypeId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => spot.Latitude = invalidLatitude);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(90.5)]
    [InlineData(-90.5)]
    [InlineData(180)]
    [InlineData(-180)]
    public void Longitude_ValidValue_SetsCorrectly(double validLongitude)
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101", CampsiteId = 1, AccommodationTypeId = 1 };

        // Act
        spot.Longitude = validLongitude;

        // Assert
        Assert.Equal(validLongitude, spot.Longitude);
    }

    [Theory]
    [InlineData(181)]
    [InlineData(-181)]
    [InlineData(200)]
    [InlineData(-200)]
    public void Longitude_InvalidValue_ThrowsArgumentException(double invalidLongitude)
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101", CampsiteId = 1, AccommodationTypeId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => spot.Longitude = invalidLongitude);
    }

    [Theory]
    [InlineData("Available")]
    [InlineData("Occupied")]
    [InlineData("Reserved")]
    [InlineData("Maintenance")]
    public void Status_ValidValue_SetsCorrectly(string validStatus)
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101", CampsiteId = 1, AccommodationTypeId = 1 };

        // Act
        spot.Status = validStatus;

        // Assert
        Assert.Equal(validStatus, spot.Status);
    }

    [Theory]
    [InlineData("Invalid")]
    [InlineData("Booked")]
    [InlineData("Closed")]
    public void Status_InvalidValue_ThrowsArgumentException(string invalidStatus)
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101", CampsiteId = 1, AccommodationTypeId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => spot.Status = invalidStatus);
    }

    [Fact]
    public void Status_DefaultValue_IsAvailable()
    {
        // Arrange & Act
        var spot = new AccommodationSpot { SpotId = "A-101", CampsiteId = 1, AccommodationTypeId = 1 };

        // Assert
        Assert.Equal("Available", spot.Status);
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    public void PriceModifier_ValidValue_SetsCorrectly(decimal validModifier)
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101", CampsiteId = 1, AccommodationTypeId = 1 };

        // Act
        spot.PriceModifier = validModifier;

        // Assert
        Assert.Equal(validModifier, spot.PriceModifier);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.5)]
    public void PriceModifier_InvalidValue_ThrowsArgumentException(decimal invalidModifier)
    {
        // Arrange
        var spot = new AccommodationSpot { SpotId = "A-101", CampsiteId = 1, AccommodationTypeId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => spot.PriceModifier = invalidModifier);
    }

    [Fact]
    public void MarkAsAvailable_SetsStatusToAvailable()
    {
        // Arrange
        var spot = new AccommodationSpot
        {
            SpotId = "A-101",
            CampsiteId = 1,
            AccommodationTypeId = 1,
            Status = "Occupied"
        };

        // Act
        spot.MarkAsAvailable();

        // Assert
        Assert.Equal("Available", spot.Status);
    }

    [Fact]
    public void MarkAsOccupied_SetsStatusToOccupied()
    {
        // Arrange
        var spot = new AccommodationSpot
        {
            SpotId = "A-101",
            CampsiteId = 1,
            AccommodationTypeId = 1
        };

        // Act
        spot.MarkAsOccupied();

        // Assert
        Assert.Equal("Occupied", spot.Status);
    }

    [Fact]
    public void MarkAsReserved_SetsStatusToReserved()
    {
        // Arrange
        var spot = new AccommodationSpot
        {
            SpotId = "A-101",
            CampsiteId = 1,
            AccommodationTypeId = 1
        };

        // Act
        spot.MarkAsReserved();

        // Assert
        Assert.Equal("Reserved", spot.Status);
    }

    [Fact]
    public void MarkAsMaintenance_SetsStatusToMaintenance()
    {
        // Arrange
        var spot = new AccommodationSpot
        {
            SpotId = "A-101",
            CampsiteId = 1,
            AccommodationTypeId = 1
        };

        // Act
        spot.MarkAsMaintenance();

        // Assert
        Assert.Equal("Maintenance", spot.Status);
    }

    [Fact]
    public void IsAvailableForBooking_WhenAvailable_ReturnsTrue()
    {
        // Arrange
        var spot = new AccommodationSpot
        {
            SpotId = "A-101",
            CampsiteId = 1,
            AccommodationTypeId = 1,
            Status = "Available"
        };

        // Act
        var result = spot.IsAvailableForBooking();

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("Occupied")]
    [InlineData("Reserved")]
    [InlineData("Maintenance")]
    public void IsAvailableForBooking_WhenNotAvailable_ReturnsFalse(string status)
    {
        // Arrange
        var spot = new AccommodationSpot
        {
            SpotId = "A-101",
            CampsiteId = 1,
            AccommodationTypeId = 1,
            Status = status
        };

        // Act
        var result = spot.IsAvailableForBooking();

        // Assert
        Assert.False(result);
    }
}

