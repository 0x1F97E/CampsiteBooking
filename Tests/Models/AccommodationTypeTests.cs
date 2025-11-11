using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class AccommodationTypeTests
{
    private AccommodationType CreateValidAccommodationType(
        int campsiteId = 1,
        string type = "Cabin",
        int maxCapacity = 4,
        decimal basePrice = 500.0m,
        int availableUnits = 10)
    {
        return AccommodationType.Create(
            CampsiteId.Create(campsiteId),
            type,
            maxCapacity,
            Money.Create(basePrice, "DKK"),
            availableUnits
        );
    }

    [Fact]
    public void AccommodationType_CanBeCreated_WithValidData()
    {
        var accommodationType = CreateValidAccommodationType();
        Assert.NotNull(accommodationType);
        Assert.Equal("Cabin", accommodationType.Type);
        Assert.Equal(4, accommodationType.MaxCapacity);
        Assert.True(accommodationType.IsActive);
    }

    [Fact]
    public void AccommodationType_Create_ThrowsException_WhenTypeIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidAccommodationType(type: ""));
    }

    [Fact]
    public void AccommodationType_Create_ThrowsException_WhenTypeIsInvalid()
    {
        Assert.Throws<DomainException>(() => CreateValidAccommodationType(type: "Invalid"));
    }

    [Theory]
    [InlineData("Cabin")]
    [InlineData("Tent Site")]
    [InlineData("RV Spot")]
    [InlineData("Glamping")]
    public void AccommodationType_Create_AcceptsValidTypes(string type)
    {
        var accommodationType = CreateValidAccommodationType(type: type);
        Assert.Equal(type, accommodationType.Type);
    }

    [Fact]
    public void AccommodationType_Create_ThrowsException_WhenMaxCapacityIsZero()
    {
        Assert.Throws<DomainException>(() => CreateValidAccommodationType(maxCapacity: 0));
    }

    [Fact]
    public void AccommodationType_Create_ThrowsException_WhenAvailableUnitsIsNegative()
    {
        Assert.Throws<DomainException>(() => CreateValidAccommodationType(availableUnits: -1));
    }

    [Fact]
    public void AccommodationType_Activate_ChangesIsActiveToTrue()
    {
        var accommodationType = CreateValidAccommodationType();
        accommodationType.Deactivate();
        accommodationType.Activate();
        Assert.True(accommodationType.IsActive);
    }

    [Fact]
    public void AccommodationType_Deactivate_ChangesIsActiveToFalse()
    {
        var accommodationType = CreateValidAccommodationType();
        accommodationType.Deactivate();
        Assert.False(accommodationType.IsActive);
    }

    [Fact]
    public void AccommodationType_ReserveUnits_DecreasesAvailableUnits()
    {
        var accommodationType = CreateValidAccommodationType(availableUnits: 10);
        accommodationType.ReserveUnits(3);
        Assert.Equal(7, accommodationType.AvailableUnits);
    }

    [Fact]
    public void AccommodationType_ReserveUnits_ThrowsException_WhenNotEnoughUnits()
    {
        var accommodationType = CreateValidAccommodationType(availableUnits: 5);
        Assert.Throws<DomainException>(() => accommodationType.ReserveUnits(10));
    }

    [Fact]
    public void AccommodationType_ReleaseUnits_IncreasesAvailableUnits()
    {
        var accommodationType = CreateValidAccommodationType(availableUnits: 10);
        accommodationType.ReleaseUnits(5);
        Assert.Equal(15, accommodationType.AvailableUnits);
    }

    [Fact]
    public void AccommodationType_UpdateInformation_UpdatesFields()
    {
        var accommodationType = CreateValidAccommodationType();
        accommodationType.UpdateInformation("New description", "http://image.jpg");
        Assert.Equal("New description", accommodationType.Description);
        Assert.Equal("http://image.jpg", accommodationType.ImageUrl);
    }
}

