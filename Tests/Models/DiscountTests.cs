using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class DiscountTests
{
    private Discount CreateValidDiscount(string code = "SUMMER20", string type = "Percentage", decimal value = 20m)
    {
        return Discount.Create(code, "Summer discount", type, value, DateTime.UtcNow.Date, DateTime.UtcNow.AddDays(30).Date);
    }

    [Fact]
    public void Discount_CanBeCreated_WithValidData()
    {
        var discount = CreateValidDiscount();
        Assert.NotNull(discount);
        Assert.Equal("SUMMER20", discount.Code);
        Assert.Equal("Percentage", discount.Type);
        Assert.Equal(20m, discount.Value);
        Assert.True(discount.IsActive);
    }

    [Fact]
    public void Discount_Create_ThrowsException_WhenCodeIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidDiscount(code: ""));
    }

    [Fact]
    public void Discount_Create_ThrowsException_WhenTypeIsInvalid()
    {
        Assert.Throws<DomainException>(() => CreateValidDiscount(type: "Invalid"));
    }

    [Fact]
    public void Discount_Create_ThrowsException_WhenValueIsZero()
    {
        Assert.Throws<DomainException>(() => CreateValidDiscount(value: 0));
    }

    [Fact]
    public void Discount_Create_ThrowsException_WhenPercentageExceeds100()
    {
        Assert.Throws<DomainException>(() => CreateValidDiscount(type: "Percentage", value: 150m));
    }

    [Fact]
    public void Discount_IsValid_ReturnsTrueForValidDiscount()
    {
        var discount = CreateValidDiscount();
        Assert.True(discount.IsValid(DateTime.UtcNow.AddDays(10)));
    }

    [Fact]
    public void Discount_IsValid_ReturnsFalseForExpiredDiscount()
    {
        var discount = CreateValidDiscount();
        Assert.False(discount.IsValid(DateTime.UtcNow.AddDays(40)));
    }

    [Fact]
    public void Discount_CalculateDiscount_ReturnsCorrectAmountForPercentage()
    {
        var discount = CreateValidDiscount(type: "Percentage", value: 20m);
        Assert.Equal(20m, discount.CalculateDiscount(100m));
    }

    [Fact]
    public void Discount_CalculateDiscount_ReturnsCorrectAmountForFixed()
    {
        var discount = CreateValidDiscount(type: "Fixed", value: 50m);
        Assert.Equal(50m, discount.CalculateDiscount(200m));
    }

    [Fact]
    public void Discount_CalculateDiscount_DoesNotExceedBookingAmountForFixed()
    {
        var discount = CreateValidDiscount(type: "Fixed", value: 150m);
        Assert.Equal(100m, discount.CalculateDiscount(100m));
    }

    [Fact]
    public void Discount_IncrementUsage_IncreasesUsedCount()
    {
        var discount = CreateValidDiscount();
        discount.IncrementUsage();
        Assert.Equal(1, discount.UsedCount);
    }

    [Fact]
    public void Discount_IncrementUsage_ThrowsException_WhenMaxUsesReached()
    {
        var discount = Discount.Create("TEST", "Test", "Percentage", 10m, DateTime.UtcNow, DateTime.UtcNow.AddDays(10), maxUses: 2);
        discount.IncrementUsage();
        discount.IncrementUsage();
        Assert.Throws<DomainException>(() => discount.IncrementUsage());
    }

    [Fact]
    public void Discount_Activate_SetsIsActiveToTrue()
    {
        var discount = CreateValidDiscount();
        discount.Deactivate();
        discount.Activate();
        Assert.True(discount.IsActive);
    }

    [Fact]
    public void Discount_Deactivate_SetsIsActiveToFalse()
    {
        var discount = CreateValidDiscount();
        discount.Deactivate();
        Assert.False(discount.IsActive);
    }

    [Fact]
    public void Discount_Code_IsStoredInUpperCase()
    {
        var discount = CreateValidDiscount(code: "summer20");
        Assert.Equal("SUMMER20", discount.Code);
    }
}
