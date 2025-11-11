using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class AmenityTests
{
    [Fact]
    public void CampsiteId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var amenity = new Amenity();

        // Act
        amenity.CampsiteId = 5;

        // Assert
        Assert.Equal(5, amenity.CampsiteId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CampsiteId_InvalidValue_ThrowsArgumentException(int invalidId)
    {
        // Arrange
        var amenity = new Amenity();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => amenity.CampsiteId = invalidId);
    }

    [Fact]
    public void Name_ValidValue_SetsCorrectly()
    {
        // Arrange
        var amenity = new Amenity { CampsiteId = 1 };

        // Act
        amenity.Name = "Swimming Pool";

        // Assert
        Assert.Equal("Swimming Pool", amenity.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Name_EmptyValue_ThrowsArgumentException(string invalidName)
    {
        // Arrange
        var amenity = new Amenity { CampsiteId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => amenity.Name = invalidName);
    }

    [Fact]
    public void IsAvailable_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var amenity = new Amenity { CampsiteId = 1, Name = "Pool" };

        // Assert
        Assert.True(amenity.IsAvailable);
    }

    [Fact]
    public void MarkAsAvailable_SetsIsAvailableToTrue()
    {
        // Arrange
        var amenity = new Amenity 
        { 
            CampsiteId = 1, 
            Name = "Pool",
            IsAvailable = false
        };

        // Act
        amenity.MarkAsAvailable();

        // Assert
        Assert.True(amenity.IsAvailable);
    }

    [Fact]
    public void MarkAsUnavailable_SetsIsAvailableToFalse()
    {
        // Arrange
        var amenity = new Amenity 
        { 
            CampsiteId = 1, 
            Name = "Pool",
            IsAvailable = true
        };

        // Act
        amenity.MarkAsUnavailable();

        // Assert
        Assert.False(amenity.IsAvailable);
    }

    [Fact]
    public void UpdateDetails_ValidData_UpdatesAllFields()
    {
        // Arrange
        var amenity = new Amenity 
        { 
            CampsiteId = 1, 
            Name = "Pool"
        };

        // Act
        amenity.UpdateDetails("Swimming Pool", "Olympic size pool", "pool-icon.png", "Facilities");

        // Assert
        Assert.Equal("Swimming Pool", amenity.Name);
        Assert.Equal("Olympic size pool", amenity.Description);
        Assert.Equal("pool-icon.png", amenity.IconUrl);
        Assert.Equal("Facilities", amenity.Category);
    }

    [Fact]
    public void UpdateDetails_NullValues_HandlesGracefully()
    {
        // Arrange
        var amenity = new Amenity 
        { 
            CampsiteId = 1, 
            Name = "Pool"
        };

        // Act
        amenity.UpdateDetails("Swimming Pool", null, null, null);

        // Assert
        Assert.Equal("Swimming Pool", amenity.Name);
        Assert.Equal(string.Empty, amenity.Description);
        Assert.Equal(string.Empty, amenity.IconUrl);
        Assert.Equal("General", amenity.Category);
    }
}

