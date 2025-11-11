using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class PeripheralPurchaseTests
{
    [Fact]
    public void BookingId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var purchase = new PeripheralPurchase();

        // Act
        purchase.BookingId = 5;

        // Assert
        Assert.Equal(5, purchase.BookingId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void BookingId_InvalidValue_ThrowsArgumentException(int invalidId)
    {
        // Arrange
        var purchase = new PeripheralPurchase();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => purchase.BookingId = invalidId);
    }

    [Fact]
    public void ItemName_ValidValue_SetsCorrectly()
    {
        // Arrange
        var purchase = new PeripheralPurchase { BookingId = 1 };

        // Act
        purchase.ItemName = "Firewood Bundle";

        // Assert
        Assert.Equal("Firewood Bundle", purchase.ItemName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ItemName_EmptyValue_ThrowsArgumentException(string invalidName)
    {
        // Arrange
        var purchase = new PeripheralPurchase { BookingId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => purchase.ItemName = invalidName);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Quantity_ValidValue_SetsCorrectlyAndUpdatesTotalPrice(int validQuantity)
    {
        // Arrange
        var purchase = new PeripheralPurchase 
        { 
            BookingId = 1,
            ItemName = "Firewood",
            UnitPrice = 10m
        };

        // Act
        purchase.Quantity = validQuantity;

        // Assert
        Assert.Equal(validQuantity, purchase.Quantity);
        Assert.Equal(validQuantity * 10m, purchase.TotalPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Quantity_InvalidValue_ThrowsArgumentException(int invalidQuantity)
    {
        // Arrange
        var purchase = new PeripheralPurchase 
        { 
            BookingId = 1,
            ItemName = "Firewood"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => purchase.Quantity = invalidQuantity);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(25.50)]
    [InlineData(100)]
    public void UnitPrice_ValidValue_SetsCorrectlyAndUpdatesTotalPrice(decimal validPrice)
    {
        // Arrange
        var purchase = new PeripheralPurchase 
        { 
            BookingId = 1,
            ItemName = "Firewood",
            Quantity = 2
        };

        // Act
        purchase.UnitPrice = validPrice;

        // Assert
        Assert.Equal(validPrice, purchase.UnitPrice);
        Assert.Equal(2 * validPrice, purchase.TotalPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void UnitPrice_InvalidValue_ThrowsArgumentException(decimal invalidPrice)
    {
        // Arrange
        var purchase = new PeripheralPurchase 
        { 
            BookingId = 1,
            ItemName = "Firewood"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => purchase.UnitPrice = invalidPrice);
    }

    [Fact]
    public void TotalPrice_CalculatedCorrectly()
    {
        // Arrange & Act
        var purchase = new PeripheralPurchase 
        { 
            BookingId = 1,
            ItemName = "Firewood",
            Quantity = 3,
            UnitPrice = 15.50m
        };

        // Assert
        Assert.Equal(46.50m, purchase.TotalPrice);
    }

    [Fact]
    public void UpdateQuantity_ValidQuantity_UpdatesQuantityAndTotalPrice()
    {
        // Arrange
        var purchase = new PeripheralPurchase 
        { 
            BookingId = 1,
            ItemName = "Firewood",
            Quantity = 2,
            UnitPrice = 10m
        };

        // Act
        purchase.UpdateQuantity(5);

        // Assert
        Assert.Equal(5, purchase.Quantity);
        Assert.Equal(50m, purchase.TotalPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateQuantity_InvalidQuantity_ThrowsArgumentException(int invalidQuantity)
    {
        // Arrange
        var purchase = new PeripheralPurchase
        {
            BookingId = 1,
            ItemName = "Firewood",
            Quantity = 2,
            UnitPrice = 10m
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => purchase.UpdateQuantity(invalidQuantity));
    }

    [Fact]
    public void Confirm_PendingPurchase_ChangesStatusToConfirmed()
    {
        // Arrange
        var purchase = new PeripheralPurchase
        {
            BookingId = 1,
            ItemName = "Firewood",
            UnitPrice = 10m,
            Status = "Pending"
        };

        // Act
        purchase.Confirm();

        // Assert
        Assert.Equal("Confirmed", purchase.Status);
    }

    [Fact]
    public void Confirm_NonPendingPurchase_ThrowsInvalidOperationException()
    {
        // Arrange
        var purchase = new PeripheralPurchase
        {
            BookingId = 1,
            ItemName = "Firewood",
            UnitPrice = 10m,
            Status = "Confirmed"
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => purchase.Confirm());
    }

    [Fact]
    public void MarkAsDelivered_ConfirmedPurchase_ChangesStatusToDelivered()
    {
        // Arrange
        var purchase = new PeripheralPurchase
        {
            BookingId = 1,
            ItemName = "Firewood",
            UnitPrice = 10m,
            Status = "Confirmed"
        };

        // Act
        purchase.MarkAsDelivered();

        // Assert
        Assert.Equal("Delivered", purchase.Status);
    }

    [Fact]
    public void MarkAsDelivered_NonConfirmedPurchase_ThrowsInvalidOperationException()
    {
        // Arrange
        var purchase = new PeripheralPurchase
        {
            BookingId = 1,
            ItemName = "Firewood",
            UnitPrice = 10m,
            Status = "Pending"
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => purchase.MarkAsDelivered());
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("Confirmed")]
    public void Cancel_NonDeliveredPurchase_ChangesStatusToCancelled(string status)
    {
        // Arrange
        var purchase = new PeripheralPurchase
        {
            BookingId = 1,
            ItemName = "Firewood",
            UnitPrice = 10m,
            Status = status
        };

        // Act
        purchase.Cancel();

        // Assert
        Assert.Equal("Cancelled", purchase.Status);
    }

    [Fact]
    public void Cancel_DeliveredPurchase_ThrowsInvalidOperationException()
    {
        // Arrange
        var purchase = new PeripheralPurchase
        {
            BookingId = 1,
            ItemName = "Firewood",
            UnitPrice = 10m,
            Status = "Delivered"
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => purchase.Cancel());
    }
}

