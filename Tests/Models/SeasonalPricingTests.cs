using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class SeasonalPricingTests
{
    private SeasonalPricing CreateValidSeasonalPricing(int campsiteId = 1, int accommodationTypeId = 1, string seasonName = "Summer", decimal priceMultiplier = 1.5m)
    {
        return SeasonalPricing.Create(CampsiteId.Create(campsiteId), AccommodationTypeId.Create(accommodationTypeId), seasonName, DateTime.UtcNow.Date, DateTime.UtcNow.AddDays(30).Date, priceMultiplier);
    }

    [Fact]
    public void SeasonalPricing_CanBeCreated_WithValidData()
    {
        var pricing = CreateValidSeasonalPricing();
        Assert.NotNull(pricing);
        Assert.Equal("Summer", pricing.SeasonName);
        Assert.Equal(1.5m, pricing.PriceMultiplier);
        Assert.True(pricing.IsActive);
    }

    [Fact]
    public void SeasonalPricing_Create_ThrowsException_WhenSeasonNameIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidSeasonalPricing(seasonName: ""));
    }

    [Fact]
    public void SeasonalPricing_Create_ThrowsException_WhenEndDateBeforeStartDate()
    {
        Assert.Throws<DomainException>(() => SeasonalPricing.Create(CampsiteId.Create(1), AccommodationTypeId.Create(1), "Winter", DateTime.UtcNow.AddDays(10), DateTime.UtcNow, 1.2m));
    }

    [Fact]
    public void SeasonalPricing_Create_ThrowsException_WhenPriceMultiplierIsZero()
    {
        Assert.Throws<DomainException>(() => CreateValidSeasonalPricing(priceMultiplier: 0));
    }

    [Fact]
    public void SeasonalPricing_IsDateInSeason_ReturnsTrueForDateInRange()
    {
        var pricing = CreateValidSeasonalPricing();
        Assert.True(pricing.IsDateInSeason(DateTime.UtcNow.AddDays(15)));
    }

    [Fact]
    public void SeasonalPricing_IsDateInSeason_ReturnsFalseForDateOutsideRange()
    {
        var pricing = CreateValidSeasonalPricing();
        Assert.False(pricing.IsDateInSeason(DateTime.UtcNow.AddDays(40)));
    }

    [Fact]
    public void SeasonalPricing_CalculatePrice_ReturnsCorrectPrice()
    {
        var pricing = CreateValidSeasonalPricing(priceMultiplier: 1.5m);
        Assert.Equal(150m, pricing.CalculatePrice(100m));
    }

    [Fact]
    public void SeasonalPricing_CalculatePrice_ThrowsException_WhenBasePriceIsZero()
    {
        var pricing = CreateValidSeasonalPricing();
        Assert.Throws<DomainException>(() => pricing.CalculatePrice(0));
    }

    [Fact]
    public void SeasonalPricing_Activate_SetsIsActiveToTrue()
    {
        var pricing = CreateValidSeasonalPricing();
        pricing.Deactivate();
        pricing.Activate();
        Assert.True(pricing.IsActive);
    }

    [Fact]
    public void SeasonalPricing_Deactivate_SetsIsActiveToFalse()
    {
        var pricing = CreateValidSeasonalPricing();
        pricing.Deactivate();
        Assert.False(pricing.IsActive);
    }

    [Fact]
    public void SeasonalPricing_GetSeasonDuration_ReturnsCorrectDays()
    {
        var pricing = CreateValidSeasonalPricing();
        Assert.Equal(31, pricing.GetSeasonDuration());
    }

    [Fact]
    public void SeasonalPricing_UpdatePriceMultiplier_UpdatesValue()
    {
        var pricing = CreateValidSeasonalPricing(priceMultiplier: 1.5m);
        pricing.UpdatePriceMultiplier(2.0m);
        Assert.Equal(2.0m, pricing.PriceMultiplier);
    }

    [Fact]
    public void SeasonalPricing_UpdatePriceMultiplier_ThrowsException_WhenValueIsZero()
    {
        var pricing = CreateValidSeasonalPricing();
        Assert.Throws<DomainException>(() => pricing.UpdatePriceMultiplier(0));
    }
}
