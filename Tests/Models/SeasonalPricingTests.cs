using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class SeasonalPricingTests
{
    [Fact]
    public void CampsiteId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var pricing = new SeasonalPricing();

        // Act
        pricing.CampsiteId = 5;

        // Assert
        Assert.Equal(5, pricing.CampsiteId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CampsiteId_InvalidValue_ThrowsArgumentException(int invalidId)
    {
        // Arrange
        var pricing = new SeasonalPricing();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => pricing.CampsiteId = invalidId);
    }

    [Fact]
    public void AccommodationTypeId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var pricing = new SeasonalPricing { CampsiteId = 1 };

        // Act
        pricing.AccommodationTypeId = 3;

        // Assert
        Assert.Equal(3, pricing.AccommodationTypeId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AccommodationTypeId_InvalidValue_ThrowsArgumentException(int invalidId)
    {
        // Arrange
        var pricing = new SeasonalPricing { CampsiteId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => pricing.AccommodationTypeId = invalidId);
    }

    [Fact]
    public void SeasonName_ValidValue_SetsCorrectly()
    {
        // Arrange
        var pricing = new SeasonalPricing { CampsiteId = 1, AccommodationTypeId = 1 };

        // Act
        pricing.SeasonName = "Summer";

        // Assert
        Assert.Equal("Summer", pricing.SeasonName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SeasonName_EmptyValue_ThrowsArgumentException(string invalidName)
    {
        // Arrange
        var pricing = new SeasonalPricing { CampsiteId = 1, AccommodationTypeId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => pricing.SeasonName = invalidName);
    }

    [Fact]
    public void EndDate_AfterStartDate_SetsCorrectly()
    {
        // Arrange
        var pricing = new SeasonalPricing 
        { 
            CampsiteId = 1, 
            AccommodationTypeId = 1,
            SeasonName = "Summer",
            StartDate = new DateTime(2025, 6, 1)
        };

        // Act
        pricing.EndDate = new DateTime(2025, 8, 31);

        // Assert
        Assert.Equal(new DateTime(2025, 8, 31), pricing.EndDate);
    }

    [Fact]
    public void EndDate_BeforeOrEqualStartDate_ThrowsArgumentException()
    {
        // Arrange
        var pricing = new SeasonalPricing 
        { 
            CampsiteId = 1, 
            AccommodationTypeId = 1,
            SeasonName = "Summer",
            StartDate = new DateTime(2025, 6, 1)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => pricing.EndDate = new DateTime(2025, 5, 31));
    }

    [Theory]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(1.5)]
    [InlineData(2.0)]
    public void PriceMultiplier_ValidValue_SetsCorrectly(decimal validMultiplier)
    {
        // Arrange
        var pricing = new SeasonalPricing 
        { 
            CampsiteId = 1, 
            AccommodationTypeId = 1,
            SeasonName = "Summer"
        };

        // Act
        pricing.PriceMultiplier = validMultiplier;

        // Assert
        Assert.Equal(validMultiplier, pricing.PriceMultiplier);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-0.5)]
    public void PriceMultiplier_InvalidValue_ThrowsArgumentException(decimal invalidMultiplier)
    {
        // Arrange
        var pricing = new SeasonalPricing 
        { 
            CampsiteId = 1, 
            AccommodationTypeId = 1,
            SeasonName = "Summer"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => pricing.PriceMultiplier = invalidMultiplier);
    }

    [Fact]
    public void IsActive_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var pricing = new SeasonalPricing { CampsiteId = 1, AccommodationTypeId = 1, SeasonName = "Summer" };

        // Assert
        Assert.True(pricing.IsActive);
    }

    [Theory]
    [InlineData("2025-07-15", true)]
    [InlineData("2025-06-01", true)]
    [InlineData("2025-08-31", true)]
    [InlineData("2025-05-31", false)]
    [InlineData("2025-09-01", false)]
    public void IsDateInSeason_ReturnsCorrectResult(string dateStr, bool expected)
    {
        // Arrange
        var pricing = new SeasonalPricing
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            SeasonName = "Summer",
            StartDate = new DateTime(2025, 6, 1),
            EndDate = new DateTime(2025, 8, 31)
        };
        var checkDate = DateTime.Parse(dateStr);

        // Act
        var result = pricing.IsDateInSeason(checkDate);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculatePrice_ValidBasePrice_ReturnsCorrectPrice()
    {
        // Arrange
        var pricing = new SeasonalPricing
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            SeasonName = "Summer",
            PriceMultiplier = 1.5m
        };

        // Act
        var result = pricing.CalculatePrice(100m);

        // Assert
        Assert.Equal(150m, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void CalculatePrice_InvalidBasePrice_ThrowsArgumentException(decimal invalidPrice)
    {
        // Arrange
        var pricing = new SeasonalPricing
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            SeasonName = "Summer",
            PriceMultiplier = 1.5m
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => pricing.CalculatePrice(invalidPrice));
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        // Arrange
        var pricing = new SeasonalPricing
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            SeasonName = "Summer",
            IsActive = false
        };

        // Act
        pricing.Activate();

        // Assert
        Assert.True(pricing.IsActive);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var pricing = new SeasonalPricing
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            SeasonName = "Summer",
            IsActive = true
        };

        // Act
        pricing.Deactivate();

        // Assert
        Assert.False(pricing.IsActive);
    }

    [Fact]
    public void GetSeasonDuration_ReturnsCorrectDays()
    {
        // Arrange
        var pricing = new SeasonalPricing
        {
            CampsiteId = 1,
            AccommodationTypeId = 1,
            SeasonName = "Summer",
            StartDate = new DateTime(2025, 6, 1),
            EndDate = new DateTime(2025, 8, 31)
        };

        // Act
        var duration = pricing.GetSeasonDuration();

        // Assert
        Assert.Equal(92, duration); // June (30) + July (31) + August (31) = 92 days
    }
}

