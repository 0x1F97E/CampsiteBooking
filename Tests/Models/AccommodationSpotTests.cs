using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class AccommodationSpotTests
{
    private AccommodationSpot CreateValidAccommodationSpot(
        string spotIdentifier = "A1",
        int campsiteId = 1,
        string campsiteName = "Test Campsite",
        int accommodationTypeId = 1,
        string type = "Tent",
        double latitude = 56.0,
        double longitude = 10.0,
        decimal priceModifier = 1.0m)
    {
        return AccommodationSpot.Create(
            spotIdentifier,
            CampsiteId.Create(campsiteId),
            campsiteName,
            AccommodationTypeId.Create(accommodationTypeId),
            type,
            latitude,
            longitude,
            priceModifier
        );
    }

    [Fact]
    public void AccommodationSpot_CanBeCreated_WithValidData()
    {
        var spot = CreateValidAccommodationSpot();
        Assert.NotNull(spot);
        Assert.Equal("A1", spot.SpotIdentifier);
        Assert.Equal("Tent", spot.Type);
        Assert.Equal(SpotStatus.Available, spot.Status);
    }

    [Fact]
    public void AccommodationSpot_Create_ThrowsException_WhenSpotIdentifierIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidAccommodationSpot(spotIdentifier: ""));
    }

    [Fact]
    public void AccommodationSpot_Create_ThrowsException_WhenTypeIsInvalid()
    {
        Assert.Throws<DomainException>(() => CreateValidAccommodationSpot(type: "Invalid"));
    }

    [Theory]
    [InlineData("Tent")]
    [InlineData("Caravan")]
    [InlineData("Cabin")]
    [InlineData("Premium")]
    public void AccommodationSpot_Create_AcceptsValidTypes(string type)
    {
        var spot = CreateValidAccommodationSpot(type: type);
        Assert.Equal(type, spot.Type);
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void AccommodationSpot_Create_ThrowsException_WhenLatitudeIsInvalid(double invalidLatitude)
    {
        Assert.Throws<DomainException>(() => CreateValidAccommodationSpot(latitude: invalidLatitude));
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void AccommodationSpot_Create_ThrowsException_WhenLongitudeIsInvalid(double invalidLongitude)
    {
        Assert.Throws<DomainException>(() => CreateValidAccommodationSpot(longitude: invalidLongitude));
    }

    [Fact]
    public void AccommodationSpot_Create_ThrowsException_WhenPriceModifierIsZero()
    {
        Assert.Throws<DomainException>(() => CreateValidAccommodationSpot(priceModifier: 0));
    }

    [Fact]
    public void AccommodationSpot_MarkAsAvailable_ChangesStatusToAvailable()
    {
        var spot = CreateValidAccommodationSpot();
        spot.MarkAsOccupied();
        spot.MarkAsAvailable();
        Assert.Equal(SpotStatus.Available, spot.Status);
    }

    [Fact]
    public void AccommodationSpot_MarkAsOccupied_ChangesStatusToOccupied()
    {
        var spot = CreateValidAccommodationSpot();
        spot.MarkAsOccupied();
        Assert.Equal(SpotStatus.Occupied, spot.Status);
    }

    [Fact]
    public void AccommodationSpot_MarkAsReserved_ChangesStatusToReserved()
    {
        var spot = CreateValidAccommodationSpot();
        spot.MarkAsReserved();
        Assert.Equal(SpotStatus.Reserved, spot.Status);
    }

    [Fact]
    public void AccommodationSpot_MarkAsMaintenance_ChangesStatusToMaintenance()
    {
        var spot = CreateValidAccommodationSpot();
        spot.MarkAsMaintenance();
        Assert.Equal(SpotStatus.Maintenance, spot.Status);
    }

    [Fact]
    public void AccommodationSpot_MarkAsOccupied_ThrowsException_WhenUnderMaintenance()
    {
        var spot = CreateValidAccommodationSpot();
        spot.MarkAsMaintenance();
        Assert.Throws<DomainException>(() => spot.MarkAsOccupied());
    }

    [Fact]
    public void AccommodationSpot_MarkAsReserved_ThrowsException_WhenUnderMaintenance()
    {
        var spot = CreateValidAccommodationSpot();
        spot.MarkAsMaintenance();
        Assert.Throws<DomainException>(() => spot.MarkAsReserved());
    }

    [Fact]
    public void AccommodationSpot_IsAvailableForBooking_ReturnsTrueWhenAvailable()
    {
        var spot = CreateValidAccommodationSpot();
        Assert.True(spot.IsAvailableForBooking());
    }

    [Fact]
    public void AccommodationSpot_IsAvailableForBooking_ReturnsFalseWhenOccupied()
    {
        var spot = CreateValidAccommodationSpot();
        spot.MarkAsOccupied();
        Assert.False(spot.IsAvailableForBooking());
    }

    [Fact]
    public void AccommodationSpot_UpdatePriceModifier_UpdatesValue()
    {
        var spot = CreateValidAccommodationSpot();
        spot.UpdatePriceModifier(1.5m);
        Assert.Equal(1.5m, spot.PriceModifier);
    }

    [Fact]
    public void AccommodationSpot_UpdatePriceModifier_ThrowsException_WhenZero()
    {
        var spot = CreateValidAccommodationSpot();
        Assert.Throws<DomainException>(() => spot.UpdatePriceModifier(0));
    }
}

