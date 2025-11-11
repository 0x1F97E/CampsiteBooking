using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class PhotoTests
{
    private Photo CreateValidPhoto(string url = "https://example.com/photo.jpg", int displayOrder = 0)
    {
        return Photo.Create(CampsiteId.Create(1), url, "Caption", "Alt text", displayOrder);
    }

    [Fact]
    public void Photo_CanBeCreated_WithValidData()
    {
        var photo = CreateValidPhoto();
        Assert.NotNull(photo);
        Assert.Equal("https://example.com/photo.jpg", photo.Url);
        Assert.Equal(0, photo.DisplayOrder);
        Assert.False(photo.IsPrimary);
    }

    [Fact]
    public void Photo_Create_ThrowsException_WhenUrlIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidPhoto(url: ""));
    }

    [Fact]
    public void Photo_Create_ThrowsException_WhenDisplayOrderIsNegative()
    {
        Assert.Throws<DomainException>(() => CreateValidPhoto(displayOrder: -1));
    }

    [Fact]
    public void Photo_SetAsPrimary_SetsIsPrimaryToTrue()
    {
        var photo = CreateValidPhoto();
        photo.SetAsPrimary();
        Assert.True(photo.IsPrimary);
    }

    [Fact]
    public void Photo_UnsetAsPrimary_SetsIsPrimaryToFalse()
    {
        var photo = CreateValidPhoto();
        photo.SetAsPrimary();
        photo.UnsetAsPrimary();
        Assert.False(photo.IsPrimary);
    }

    [Fact]
    public void Photo_UpdateDisplayOrder_UpdatesOrder()
    {
        var photo = CreateValidPhoto(displayOrder: 0);
        photo.UpdateDisplayOrder(5);
        Assert.Equal(5, photo.DisplayOrder);
    }

    [Fact]
    public void Photo_UpdateDisplayOrder_ThrowsException_WhenNegative()
    {
        var photo = CreateValidPhoto();
        Assert.Throws<DomainException>(() => photo.UpdateDisplayOrder(-1));
    }

    [Fact]
    public void Photo_UpdateCaption_UpdatesCaptionAndAltText()
    {
        var photo = CreateValidPhoto();
        photo.UpdateCaption("New Caption", "New Alt Text");
        Assert.Equal("New Caption", photo.Caption);
        Assert.Equal("New Alt Text", photo.AltText);
    }
}
