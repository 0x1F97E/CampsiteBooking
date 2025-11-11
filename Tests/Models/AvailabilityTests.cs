using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class AvailabilityTests
{
    private Availability CreateValidAvailability(int campsiteId = 1, int accommodationTypeId = 1, DateTime? date = null, int totalUnits = 10)
    {
        return Availability.Create(CampsiteId.Create(campsiteId), AccommodationTypeId.Create(accommodationTypeId), date ?? DateTime.UtcNow.AddDays(1), totalUnits);
    }

    [Fact]
    public void Availability_CanBeCreated_WithValidData()
    {
        var availability = CreateValidAvailability();
        Assert.NotNull(availability);
        Assert.Equal(10, availability.AvailableUnits);
        Assert.Equal(0, availability.ReservedUnits);
    }

    [Fact]
    public void Availability_Create_ThrowsException_WhenDateIsInPast()
    {
        Assert.Throws<DomainException>(() => CreateValidAvailability(date: DateTime.UtcNow.AddDays(-1)));
    }

    [Fact]
    public void Availability_Create_ThrowsException_WhenTotalUnitsIsNegative()
    {
        Assert.Throws<DomainException>(() => CreateValidAvailability(totalUnits: -1));
    }

    [Fact]
    public void Availability_ReserveUnits_DecreasesAvailableAndIncreasesReserved()
    {
        var availability = CreateValidAvailability(totalUnits: 10);
        availability.ReserveUnits(3);
        Assert.Equal(7, availability.AvailableUnits);
        Assert.Equal(3, availability.ReservedUnits);
    }

    [Fact]
    public void Availability_ReserveUnits_ThrowsException_WhenCountIsZero()
    {
        var availability = CreateValidAvailability();
        Assert.Throws<DomainException>(() => availability.ReserveUnits(0));
    }

    [Fact]
    public void Availability_ReserveUnits_ThrowsException_WhenNotEnoughUnits()
    {
        var availability = CreateValidAvailability(totalUnits: 5);
        Assert.Throws<DomainException>(() => availability.ReserveUnits(10));
    }

    [Fact]
    public void Availability_ReleaseUnits_IncreasesAvailableAndDecreasesReserved()
    {
        var availability = CreateValidAvailability(totalUnits: 10);
        availability.ReserveUnits(5);
        availability.ReleaseUnits(2);
        Assert.Equal(7, availability.AvailableUnits);
        Assert.Equal(3, availability.ReservedUnits);
    }

    [Fact]
    public void Availability_ReleaseUnits_ThrowsException_WhenCountExceedsReserved()
    {
        var availability = CreateValidAvailability(totalUnits: 10);
        availability.ReserveUnits(3);
        Assert.Throws<DomainException>(() => availability.ReleaseUnits(5));
    }

    [Fact]
    public void Availability_GetTotalCapacity_ReturnsCorrectSum()
    {
        var availability = CreateValidAvailability(totalUnits: 10);
        availability.ReserveUnits(4);
        Assert.Equal(10, availability.GetTotalCapacity());
    }

    [Fact]
    public void Availability_HasAvailability_ReturnsTrueWhenEnoughUnits()
    {
        var availability = CreateValidAvailability(totalUnits: 10);
        Assert.True(availability.HasAvailability(5));
    }

    [Fact]
    public void Availability_HasAvailability_ReturnsFalseWhenNotEnoughUnits()
    {
        var availability = CreateValidAvailability(totalUnits: 10);
        availability.ReserveUnits(8);
        Assert.False(availability.HasAvailability(5));
    }
}
