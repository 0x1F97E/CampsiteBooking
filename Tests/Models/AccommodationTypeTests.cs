using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class AccommodationTypeTests
{
    [Fact]
    public void CampsiteId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var accommodationType = new AccommodationType();

        // Act
        accommodationType.CampsiteId = 5;

        // Assert
        Assert.Equal(5, accommodationType.CampsiteId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CampsiteId_InvalidValue_ThrowsArgumentException(int invalidCampsiteId)
    {
        // Arrange
        var accommodationType = new AccommodationType();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => accommodationType.CampsiteId = invalidCampsiteId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void MaxCapacity_ValidValue_SetsCorrectly(int validCapacity)
    {
        // Arrange
        var accommodationType = new AccommodationType { CampsiteId = 1 };

        // Act
        accommodationType.MaxCapacity = validCapacity;

        // Assert
        Assert.Equal(validCapacity, accommodationType.MaxCapacity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void MaxCapacity_InvalidValue_ThrowsArgumentException(int invalidCapacity)
    {
        // Arrange
        var accommodationType = new AccommodationType { CampsiteId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => accommodationType.MaxCapacity = invalidCapacity);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(250.50)]
    [InlineData(1000)]
    public void BasePrice_ValidValue_SetsCorrectly(decimal validPrice)
    {
        // Arrange
        var accommodationType = new AccommodationType { CampsiteId = 1 };

        // Act
        accommodationType.BasePrice = validPrice;

        // Assert
        Assert.Equal(validPrice, accommodationType.BasePrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void BasePrice_InvalidValue_ThrowsArgumentException(decimal invalidPrice)
    {
        // Arrange
        var accommodationType = new AccommodationType { CampsiteId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => accommodationType.BasePrice = invalidPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(100)]
    public void AvailableUnits_ValidValue_SetsCorrectly(int validUnits)
    {
        // Arrange
        var accommodationType = new AccommodationType { CampsiteId = 1 };

        // Act
        accommodationType.AvailableUnits = validUnits;

        // Assert
        Assert.Equal(validUnits, accommodationType.AvailableUnits);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void AvailableUnits_NegativeValue_ThrowsArgumentException(int invalidUnits)
    {
        // Arrange
        var accommodationType = new AccommodationType { CampsiteId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => accommodationType.AvailableUnits = invalidUnits);
    }

    [Fact]
    public void IsActive_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var accommodationType = new AccommodationType { CampsiteId = 1 };

        // Assert
        Assert.True(accommodationType.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        // Arrange
        var accommodationType = new AccommodationType 
        { 
            CampsiteId = 1,
            IsActive = false 
        };

        // Act
        accommodationType.Activate();

        // Assert
        Assert.True(accommodationType.IsActive);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var accommodationType = new AccommodationType
        {
            CampsiteId = 1,
            IsActive = true
        };

        // Act
        accommodationType.Deactivate();

        // Assert
        Assert.False(accommodationType.IsActive);
    }

    [Fact]
    public void ReserveUnits_ValidCount_DecreasesAvailableUnits()
    {
        // Arrange
        var accommodationType = new AccommodationType
        {
            CampsiteId = 1,
            AvailableUnits = 10
        };

        // Act
        accommodationType.ReserveUnits(3);

        // Assert
        Assert.Equal(7, accommodationType.AvailableUnits);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void ReserveUnits_InvalidCount_ThrowsArgumentException(int invalidCount)
    {
        // Arrange
        var accommodationType = new AccommodationType
        {
            CampsiteId = 1,
            AvailableUnits = 10
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => accommodationType.ReserveUnits(invalidCount));
    }

    [Fact]
    public void ReserveUnits_CountExceedsAvailable_ThrowsInvalidOperationException()
    {
        // Arrange
        var accommodationType = new AccommodationType
        {
            CampsiteId = 1,
            AvailableUnits = 5
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => accommodationType.ReserveUnits(10));
        Assert.Contains("Not enough available units", exception.Message);
    }

    [Fact]
    public void ReleaseUnits_ValidCount_IncreasesAvailableUnits()
    {
        // Arrange
        var accommodationType = new AccommodationType
        {
            CampsiteId = 1,
            AvailableUnits = 5
        };

        // Act
        accommodationType.ReleaseUnits(3);

        // Assert
        Assert.Equal(8, accommodationType.AvailableUnits);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void ReleaseUnits_InvalidCount_ThrowsArgumentException(int invalidCount)
    {
        // Arrange
        var accommodationType = new AccommodationType
        {
            CampsiteId = 1,
            AvailableUnits = 10
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => accommodationType.ReleaseUnits(invalidCount));
    }
}

