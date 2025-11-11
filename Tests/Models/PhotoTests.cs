using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class PhotoTests
{
    [Fact]
    public void CampsiteId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var photo = new Photo();

        // Act
        photo.CampsiteId = 5;

        // Assert
        Assert.Equal(5, photo.CampsiteId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CampsiteId_InvalidValue_ThrowsArgumentException(int invalidId)
    {
        // Arrange
        var photo = new Photo();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => photo.CampsiteId = invalidId);
    }

    [Fact]
    public void Url_ValidValue_SetsCorrectly()
    {
        // Arrange
        var photo = new Photo { CampsiteId = 1 };

        // Act
        photo.Url = "https://example.com/photo.jpg";

        // Assert
        Assert.Equal("https://example.com/photo.jpg", photo.Url);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Url_EmptyValue_ThrowsArgumentException(string invalidUrl)
    {
        // Arrange
        var photo = new Photo { CampsiteId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => photo.Url = invalidUrl);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    public void DisplayOrder_ValidValue_SetsCorrectly(int validOrder)
    {
        // Arrange
        var photo = new Photo { CampsiteId = 1, Url = "test.jpg" };

        // Act
        photo.DisplayOrder = validOrder;

        // Assert
        Assert.Equal(validOrder, photo.DisplayOrder);
    }

    [Fact]
    public void DisplayOrder_NegativeValue_ThrowsArgumentException()
    {
        // Arrange
        var photo = new Photo { CampsiteId = 1, Url = "test.jpg" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => photo.DisplayOrder = -1);
    }

    [Fact]
    public void IsPrimary_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var photo = new Photo { CampsiteId = 1, Url = "test.jpg" };

        // Assert
        Assert.False(photo.IsPrimary);
    }

    [Fact]
    public void SetAsPrimary_SetsIsPrimaryToTrue()
    {
        // Arrange
        var photo = new Photo 
        { 
            CampsiteId = 1, 
            Url = "test.jpg",
            IsPrimary = false
        };

        // Act
        photo.SetAsPrimary();

        // Assert
        Assert.True(photo.IsPrimary);
    }

    [Fact]
    public void UnsetAsPrimary_SetsIsPrimaryToFalse()
    {
        // Arrange
        var photo = new Photo 
        { 
            CampsiteId = 1, 
            Url = "test.jpg",
            IsPrimary = true
        };

        // Act
        photo.UnsetAsPrimary();

        // Assert
        Assert.False(photo.IsPrimary);
    }

    [Fact]
    public void UpdateDisplayOrder_ValidOrder_UpdatesDisplayOrder()
    {
        // Arrange
        var photo = new Photo 
        { 
            CampsiteId = 1, 
            Url = "test.jpg",
            DisplayOrder = 0
        };

        // Act
        photo.UpdateDisplayOrder(5);

        // Assert
        Assert.Equal(5, photo.DisplayOrder);
    }

    [Fact]
    public void UpdateDisplayOrder_NegativeOrder_ThrowsArgumentException()
    {
        // Arrange
        var photo = new Photo 
        { 
            CampsiteId = 1, 
            Url = "test.jpg"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => photo.UpdateDisplayOrder(-1));
    }

    [Fact]
    public void UpdateCaption_ValidData_UpdatesCaptionAndAltText()
    {
        // Arrange
        var photo = new Photo 
        { 
            CampsiteId = 1, 
            Url = "test.jpg"
        };

        // Act
        photo.UpdateCaption("Beautiful sunset", "Sunset over campsite");

        // Assert
        Assert.Equal("Beautiful sunset", photo.Caption);
        Assert.Equal("Sunset over campsite", photo.AltText);
    }

    [Fact]
    public void UpdateCaption_NullValues_SetsEmptyStrings()
    {
        // Arrange
        var photo = new Photo 
        { 
            CampsiteId = 1, 
            Url = "test.jpg"
        };

        // Act
        photo.UpdateCaption(null, null);

        // Assert
        Assert.Equal(string.Empty, photo.Caption);
        Assert.Equal(string.Empty, photo.AltText);
    }
}

