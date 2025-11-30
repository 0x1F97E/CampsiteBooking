using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class CampsiteTests
{
    private Campsite CreateValidCampsite(
        string name = "Sunset Campsite",
        string streetAddress = "Campingvej 1",
        string city = "Copenhagen",
        string postalCode = "2100",
        double latitude = 56.0,
        double longitude = 10.0,
        int establishedYear = 2000,
        string attractiveness = "Medium")
    {
        return Campsite.Create(name, streetAddress, city, postalCode, latitude, longitude, establishedYear, attractiveness: attractiveness);
    }

    [Fact]
    public void Campsite_CanBeCreated_WithValidData()
    {
        var campsite = CreateValidCampsite();
        Assert.NotNull(campsite);
        Assert.Equal("Sunset Campsite", campsite.Name);
        Assert.True(campsite.IsActive);
    }

    [Fact]
    public void Campsite_Create_ThrowsException_WhenNameIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidCampsite(name: ""));
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Campsite_Create_ThrowsException_WhenLatitudeIsInvalid(double invalidLatitude)
    {
        Assert.Throws<DomainException>(() => CreateValidCampsite(latitude: invalidLatitude));
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Campsite_Create_ThrowsException_WhenLongitudeIsInvalid(double invalidLongitude)
    {
        Assert.Throws<DomainException>(() => CreateValidCampsite(longitude: invalidLongitude));
    }

    [Fact]
    public void Campsite_Create_ThrowsException_WhenEstablishedYearIsInFuture()
    {
        Assert.Throws<DomainException>(() => CreateValidCampsite(establishedYear: DateTime.UtcNow.Year + 1));
    }

    [Fact]
    public void Campsite_Create_ThrowsException_WhenAttractivenessIsInvalid()
    {
        Assert.Throws<DomainException>(() => CreateValidCampsite(attractiveness: "Invalid"));
    }

    [Theory]
    [InlineData("Low")]
    [InlineData("Medium")]
    [InlineData("High")]
    [InlineData("Very High")]
    public void Campsite_Create_AcceptsValidAttractiveness(string attractiveness)
    {
        var campsite = CreateValidCampsite(attractiveness: attractiveness);
        Assert.Equal(attractiveness, campsite.Attractiveness);
    }

    [Fact]
    public void Campsite_Activate_ChangesIsActiveToTrue()
    {
        var campsite = CreateValidCampsite();
        campsite.Deactivate();
        campsite.Activate();
        Assert.True(campsite.IsActive);
    }

    [Fact]
    public void Campsite_Activate_ThrowsException_WhenAlreadyActive()
    {
        var campsite = CreateValidCampsite();
        Assert.Throws<DomainException>(() => campsite.Activate());
    }

    [Fact]
    public void Campsite_Deactivate_ChangesIsActiveToFalse()
    {
        var campsite = CreateValidCampsite();
        campsite.Deactivate();
        Assert.False(campsite.IsActive);
    }

    [Fact]
    public void Campsite_Deactivate_ThrowsException_WhenAlreadyInactive()
    {
        var campsite = CreateValidCampsite();
        campsite.Deactivate();
        Assert.Throws<DomainException>(() => campsite.Deactivate());
    }

    [Fact]
    public void Campsite_UpdateInformation_UpdatesFields()
    {
        var campsite = CreateValidCampsite();
        var email = Email.Create("new@campsite.com");
        campsite.UpdateInformation("New Name", "New Street 123", "Aarhus", "8000", "New description", "High", "12345678", email, "http://new.com");
        Assert.Equal("New Name", campsite.Name);
        Assert.Equal("Aarhus", campsite.City);
    }

    [Fact]
    public void Campsite_UpdateLocation_UpdatesCoordinates()
    {
        var campsite = CreateValidCampsite();
        campsite.UpdateLocation(57.0, 11.0);
        Assert.Equal(57.0, campsite.Latitude);
        Assert.Equal(11.0, campsite.Longitude);
    }

}

