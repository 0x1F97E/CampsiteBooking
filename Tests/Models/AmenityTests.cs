using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class AmenityTests
{
    private Amenity CreateValidAmenity(string name = "Swimming Pool", string category = "Facilities")
    {
        return Amenity.Create(CampsiteId.Create(1), name, "Description", "icon.png", category);
    }

    [Fact]
    public void Amenity_CanBeCreated_WithValidData()
    {
        var amenity = CreateValidAmenity();
        Assert.NotNull(amenity);
        Assert.Equal("Swimming Pool", amenity.Name);
        Assert.Equal("Facilities", amenity.Category);
        Assert.True(amenity.IsAvailable);
    }

    [Fact]
    public void Amenity_Create_ThrowsException_WhenNameIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidAmenity(name: ""));
    }

    [Fact]
    public void Amenity_Create_ThrowsException_WhenCategoryIsInvalid()
    {
        Assert.Throws<DomainException>(() => CreateValidAmenity(category: "Invalid"));
    }

    [Theory]
    [InlineData("General")]
    [InlineData("Facilities")]
    [InlineData("Activities")]
    [InlineData("Services")]
    public void Amenity_Create_AcceptsValidCategories(string category)
    {
        var amenity = CreateValidAmenity(category: category);
        Assert.Equal(category, amenity.Category);
    }

    [Fact]
    public void Amenity_MarkAsAvailable_SetsIsAvailableToTrue()
    {
        var amenity = CreateValidAmenity();
        amenity.MarkAsUnavailable();
        amenity.MarkAsAvailable();
        Assert.True(amenity.IsAvailable);
    }

    [Fact]
    public void Amenity_MarkAsUnavailable_SetsIsAvailableToFalse()
    {
        var amenity = CreateValidAmenity();
        amenity.MarkAsUnavailable();
        Assert.False(amenity.IsAvailable);
    }

    [Fact]
    public void Amenity_UpdateDetails_UpdatesProperties()
    {
        var amenity = CreateValidAmenity();
        amenity.UpdateDetails("New Name", "New Description", "new-icon.png", "Activities");
        Assert.Equal("New Name", amenity.Name);
        Assert.Equal("New Description", amenity.Description);
        Assert.Equal("Activities", amenity.Category);
    }
}
