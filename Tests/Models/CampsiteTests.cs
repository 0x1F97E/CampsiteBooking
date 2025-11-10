using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class CampsiteTests
{
    [Fact]
    public void Name_ValidValue_SetsCorrectly()
    {
        // Arrange
        var campsite = new Campsite();

        // Act
        campsite.Name = "Sunset Campsite";

        // Assert
        Assert.Equal("Sunset Campsite", campsite.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Name_EmptyValue_ThrowsArgumentException(string invalidName)
    {
        // Arrange
        var campsite = new Campsite();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => campsite.Name = invalidName);
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
        var campsite = new Campsite { Name = "Test Campsite", TotalArea = 100 };

        // Act
        campsite.Latitude = validLatitude;

        // Assert
        Assert.Equal(validLatitude, campsite.Latitude);
    }

    [Theory]
    [InlineData(91)]
    [InlineData(-91)]
    [InlineData(100)]
    [InlineData(-100)]
    public void Latitude_InvalidValue_ThrowsArgumentException(double invalidLatitude)
    {
        // Arrange
        var campsite = new Campsite { Name = "Test Campsite", TotalArea = 100 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => campsite.Latitude = invalidLatitude);
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
        var campsite = new Campsite { Name = "Test Campsite", TotalArea = 100 };

        // Act
        campsite.Longitude = validLongitude;

        // Assert
        Assert.Equal(validLongitude, campsite.Longitude);
    }

    [Theory]
    [InlineData(181)]
    [InlineData(-181)]
    [InlineData(200)]
    [InlineData(-200)]
    public void Longitude_InvalidValue_ThrowsArgumentException(double invalidLongitude)
    {
        // Arrange
        var campsite = new Campsite { Name = "Test Campsite", TotalArea = 100 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => campsite.Longitude = invalidLongitude);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100.5)]
    [InlineData(1000)]
    public void TotalArea_ValidValue_SetsCorrectly(decimal validArea)
    {
        // Arrange
        var campsite = new Campsite { Name = "Test Campsite" };

        // Act
        campsite.TotalArea = validArea;

        // Assert
        Assert.Equal(validArea, campsite.TotalArea);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void TotalArea_InvalidValue_ThrowsArgumentException(decimal invalidArea)
    {
        // Arrange
        var campsite = new Campsite { Name = "Test Campsite" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => campsite.TotalArea = invalidArea);
    }

    [Fact]
    public void EstablishedYear_ValidYear_SetsCorrectly()
    {
        // Arrange
        var campsite = new Campsite { Name = "Test Campsite", TotalArea = 100 };
        var validYear = DateTime.UtcNow.Year - 10;

        // Act
        campsite.EstablishedYear = validYear;

        // Assert
        Assert.Equal(validYear, campsite.EstablishedYear);
    }

    [Fact]
    public void EstablishedYear_FutureYear_ThrowsArgumentException()
    {
        // Arrange
        var campsite = new Campsite { Name = "Test Campsite", TotalArea = 100 };
        var futureYear = DateTime.UtcNow.Year + 1;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => campsite.EstablishedYear = futureYear);
    }

    [Fact]
    public void IsActive_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var campsite = new Campsite { Name = "Test Campsite", TotalArea = 100 };

        // Assert
        Assert.True(campsite.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        // Arrange
        var campsite = new Campsite
        {
            Name = "Test Campsite",
            TotalArea = 100,
            IsActive = false
        };

        // Act
        campsite.Activate();

        // Assert
        Assert.True(campsite.IsActive);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var campsite = new Campsite
        {
            Name = "Test Campsite",
            TotalArea = 100,
            IsActive = true
        };

        // Act
        campsite.Deactivate();

        // Assert
        Assert.False(campsite.IsActive);
    }

    [Fact]
    public void Update_UpdatesUpdatedDate()
    {
        // Arrange
        var campsite = new Campsite
        {
            Name = "Test Campsite",
            TotalArea = 100
        };
        var originalUpdatedDate = campsite.UpdatedDate;

        // Wait a tiny bit to ensure time difference
        System.Threading.Thread.Sleep(10);

        // Act
        campsite.Update();

        // Assert
        Assert.True(campsite.UpdatedDate > originalUpdatedDate);
    }
}

