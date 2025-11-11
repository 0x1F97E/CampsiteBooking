using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_ValidAmount_CreatesMoneyObject()
    {
        // Act
        var money = Money.Create(100.50m, "DKK");

        // Assert
        Assert.Equal(100.50m, money.Amount);
        Assert.Equal("DKK", money.Currency);
    }

    [Fact]
    public void Create_DefaultCurrency_UsesDKK()
    {
        // Act
        var money = Money.Create(100m);

        // Assert
        Assert.Equal("DKK", money.Currency);
    }

    [Fact]
    public void Create_NegativeAmount_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Money.Create(-10m, "DKK"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyCurrency_ThrowsArgumentException(string invalidCurrency)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Money.Create(100m, invalidCurrency));
    }

    [Theory]
    [InlineData("DK")]
    [InlineData("DKKK")]
    public void Create_InvalidCurrencyLength_ThrowsArgumentException(string invalidCurrency)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Money.Create(100m, invalidCurrency));
    }

    [Fact]
    public void Zero_CreatesZeroAmount()
    {
        // Act
        var money = Money.Zero("EUR");

        // Assert
        Assert.Equal(0m, money.Amount);
        Assert.Equal("EUR", money.Currency);
    }

    [Fact]
    public void Add_SameCurrency_ReturnsSum()
    {
        // Arrange
        var money1 = Money.Create(100m, "DKK");
        var money2 = Money.Create(50m, "DKK");

        // Act
        var result = money1.Add(money2);

        // Assert
        Assert.Equal(150m, result.Amount);
        Assert.Equal("DKK", result.Currency);
    }

    [Fact]
    public void Add_DifferentCurrency_ThrowsInvalidOperationException()
    {
        // Arrange
        var money1 = Money.Create(100m, "DKK");
        var money2 = Money.Create(50m, "EUR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => money1.Add(money2));
    }

    [Fact]
    public void Subtract_SameCurrency_ReturnsDifference()
    {
        // Arrange
        var money1 = Money.Create(100m, "DKK");
        var money2 = Money.Create(30m, "DKK");

        // Act
        var result = money1.Subtract(money2);

        // Assert
        Assert.Equal(70m, result.Amount);
        Assert.Equal("DKK", result.Currency);
    }

    [Fact]
    public void Subtract_ResultNegative_ThrowsInvalidOperationException()
    {
        // Arrange
        var money1 = Money.Create(50m, "DKK");
        var money2 = Money.Create(100m, "DKK");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => money1.Subtract(money2));
    }

    [Fact]
    public void Multiply_ValidMultiplier_ReturnsProduct()
    {
        // Arrange
        var money = Money.Create(100m, "DKK");

        // Act
        var result = money.Multiply(1.5m);

        // Assert
        Assert.Equal(150m, result.Amount);
        Assert.Equal("DKK", result.Currency);
    }

    [Fact]
    public void Multiply_NegativeMultiplier_ThrowsArgumentException()
    {
        // Arrange
        var money = Money.Create(100m, "DKK");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => money.Multiply(-1.5m));
    }

    [Fact]
    public void IsGreaterThan_LargerAmount_ReturnsTrue()
    {
        // Arrange
        var money1 = Money.Create(100m, "DKK");
        var money2 = Money.Create(50m, "DKK");

        // Act
        var result = money1.IsGreaterThan(money2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsLessThan_SmallerAmount_ReturnsTrue()
    {
        // Arrange
        var money1 = Money.Create(50m, "DKK");
        var money2 = Money.Create(100m, "DKK");

        // Act
        var result = money1.IsLessThan(money2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_SameAmountAndCurrency_ReturnsTrue()
    {
        // Arrange
        var money1 = Money.Create(100m, "DKK");
        var money2 = Money.Create(100m, "DKK");

        // Act & Assert
        Assert.Equal(money1, money2);
        Assert.True(money1 == money2);
    }

    [Fact]
    public void Equals_DifferentAmount_ReturnsFalse()
    {
        // Arrange
        var money1 = Money.Create(100m, "DKK");
        var money2 = Money.Create(50m, "DKK");

        // Act & Assert
        Assert.NotEqual(money1, money2);
        Assert.True(money1 != money2);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        var money = Money.Create(1234.56m, "DKK");

        // Act
        var result = money.ToString();

        // Assert
        Assert.Contains("1,234.56", result);
        Assert.Contains("DKK", result);
    }
}

