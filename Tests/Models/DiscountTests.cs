using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class DiscountTests
{
    [Fact]
    public void Code_ValidValue_SetsCorrectlyInUppercase()
    {
        // Arrange
        var discount = new Discount();

        // Act
        discount.Code = "summer2025";

        // Assert
        Assert.Equal("SUMMER2025", discount.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Code_EmptyValue_ThrowsArgumentException(string invalidCode)
    {
        // Arrange
        var discount = new Discount();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => discount.Code = invalidCode);
    }

    [Theory]
    [InlineData("Percentage")]
    [InlineData("Fixed")]
    public void Type_ValidValue_SetsCorrectly(string validType)
    {
        // Arrange
        var discount = new Discount { Code = "TEST" };

        // Act
        discount.Type = validType;

        // Assert
        Assert.Equal(validType, discount.Type);
    }

    [Fact]
    public void Type_InvalidValue_ThrowsArgumentException()
    {
        // Arrange
        var discount = new Discount { Code = "TEST" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => discount.Type = "Invalid");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void Value_ValidPercentage_SetsCorrectly(decimal validValue)
    {
        // Arrange
        var discount = new Discount { Code = "TEST", Type = "Percentage" };

        // Act
        discount.Value = validValue;

        // Assert
        Assert.Equal(validValue, discount.Value);
    }

    [Fact]
    public void Value_PercentageOver100_ThrowsArgumentException()
    {
        // Arrange
        var discount = new Discount { Code = "TEST", Type = "Percentage" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => discount.Value = 101);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Value_InvalidValue_ThrowsArgumentException(decimal invalidValue)
    {
        // Arrange
        var discount = new Discount { Code = "TEST", Type = "Percentage" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => discount.Value = invalidValue);
    }

    [Fact]
    public void ValidUntil_AfterValidFrom_SetsCorrectly()
    {
        // Arrange
        var discount = new Discount 
        { 
            Code = "TEST",
            ValidFrom = new DateTime(2025, 1, 1)
        };

        // Act
        discount.ValidUntil = new DateTime(2025, 12, 31);

        // Assert
        Assert.Equal(new DateTime(2025, 12, 31), discount.ValidUntil);
    }

    [Fact]
    public void ValidUntil_BeforeValidFrom_ThrowsArgumentException()
    {
        // Arrange
        var discount = new Discount 
        { 
            Code = "TEST",
            ValidFrom = new DateTime(2025, 6, 1)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => discount.ValidUntil = new DateTime(2025, 5, 31));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(100)]
    public void MaxUses_ValidValue_SetsCorrectly(int validMaxUses)
    {
        // Arrange
        var discount = new Discount { Code = "TEST" };

        // Act
        discount.MaxUses = validMaxUses;

        // Assert
        Assert.Equal(validMaxUses, discount.MaxUses);
    }

    [Fact]
    public void MaxUses_NegativeValue_ThrowsArgumentException()
    {
        // Arrange
        var discount = new Discount { Code = "TEST" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => discount.MaxUses = -1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(500)]
    public void MinimumBookingAmount_ValidValue_SetsCorrectly(decimal validAmount)
    {
        // Arrange
        var discount = new Discount { Code = "TEST" };

        // Act
        discount.MinimumBookingAmount = validAmount;

        // Assert
        Assert.Equal(validAmount, discount.MinimumBookingAmount);
    }

    [Fact]
    public void MinimumBookingAmount_NegativeValue_ThrowsArgumentException()
    {
        // Arrange
        var discount = new Discount { Code = "TEST" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => discount.MinimumBookingAmount = -1);
    }

    [Fact]
    public void IsValid_ActiveAndWithinDateRange_ReturnsTrue()
    {
        // Arrange
        var discount = new Discount
        {
            Code = "TEST",
            IsActive = true,
            ValidFrom = DateTime.UtcNow.AddDays(-7),
            ValidUntil = DateTime.UtcNow.AddDays(7),
            MaxUses = 0
        };

        // Act
        var result = discount.IsValid(DateTime.UtcNow);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_Inactive_ReturnsFalse()
    {
        // Arrange
        var discount = new Discount
        {
            Code = "TEST",
            IsActive = false,
            ValidFrom = DateTime.UtcNow.AddDays(-7),
            ValidUntil = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var result = discount.IsValid(DateTime.UtcNow);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_MaxUsesReached_ReturnsFalse()
    {
        // Arrange
        var discount = new Discount
        {
            Code = "TEST",
            IsActive = true,
            ValidFrom = DateTime.UtcNow.AddDays(-7),
            ValidUntil = DateTime.UtcNow.AddDays(7),
            MaxUses = 5,
            UsedCount = 5
        };

        // Act
        var result = discount.IsValid(DateTime.UtcNow);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void CalculateDiscount_PercentageType_ReturnsCorrectAmount()
    {
        // Arrange
        var discount = new Discount
        {
            Code = "TEST",
            Type = "Percentage",
            Value = 20,
            MinimumBookingAmount = 0
        };

        // Act
        var result = discount.CalculateDiscount(100m);

        // Assert
        Assert.Equal(20m, result);
    }

    [Fact]
    public void CalculateDiscount_FixedType_ReturnsCorrectAmount()
    {
        // Arrange
        var discount = new Discount
        {
            Code = "TEST",
            Type = "Fixed",
            Value = 50,
            MinimumBookingAmount = 0
        };

        // Act
        var result = discount.CalculateDiscount(200m);

        // Assert
        Assert.Equal(50m, result);
    }

    [Fact]
    public void CalculateDiscount_FixedExceedsBookingAmount_ReturnsBookingAmount()
    {
        // Arrange
        var discount = new Discount
        {
            Code = "TEST",
            Type = "Fixed",
            Value = 150,
            MinimumBookingAmount = 0
        };

        // Act
        var result = discount.CalculateDiscount(100m);

        // Assert
        Assert.Equal(100m, result);
    }

    [Fact]
    public void CalculateDiscount_BelowMinimum_ThrowsInvalidOperationException()
    {
        // Arrange
        var discount = new Discount
        {
            Code = "TEST",
            Type = "Percentage",
            Value = 20,
            MinimumBookingAmount = 100
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => discount.CalculateDiscount(50m));
    }

    [Fact]
    public void IncrementUsage_ValidUsage_IncrementsCount()
    {
        // Arrange
        var discount = new Discount
        {
            Code = "TEST",
            MaxUses = 10,
            UsedCount = 5
        };

        // Act
        discount.IncrementUsage();

        // Assert
        Assert.Equal(6, discount.UsedCount);
    }

    [Fact]
    public void IncrementUsage_MaxUsesReached_ThrowsInvalidOperationException()
    {
        // Arrange
        var discount = new Discount
        {
            Code = "TEST",
            MaxUses = 5,
            UsedCount = 5
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => discount.IncrementUsage());
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        // Arrange
        var discount = new Discount { Code = "TEST", IsActive = false };

        // Act
        discount.Activate();

        // Assert
        Assert.True(discount.IsActive);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        // Arrange
        var discount = new Discount { Code = "TEST", IsActive = true };

        // Act
        discount.Deactivate();

        // Assert
        Assert.False(discount.IsActive);
    }
}

