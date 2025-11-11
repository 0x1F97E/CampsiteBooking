using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class PeripheralPurchaseTests
{
    private PeripheralPurchase CreateValidPurchase(string itemName = "Firewood", int quantity = 2, decimal unitPrice = 50m)
    {
        return PeripheralPurchase.Create(BookingId.Create(1), itemName, "Description", quantity, Money.Create(unitPrice));
    }

    [Fact]
    public void PeripheralPurchase_CanBeCreated_WithValidData()
    {
        var purchase = CreateValidPurchase();
        Assert.NotNull(purchase);
        Assert.Equal("Firewood", purchase.ItemName);
        Assert.Equal(2, purchase.Quantity);
        Assert.Equal(100m, purchase.TotalPrice.Amount);
    }

    [Fact]
    public void PeripheralPurchase_Create_ThrowsException_WhenItemNameIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidPurchase(itemName: ""));
    }

    [Fact]
    public void PeripheralPurchase_Create_ThrowsException_WhenQuantityIsZero()
    {
        Assert.Throws<DomainException>(() => CreateValidPurchase(quantity: 0));
    }

    [Fact]
    public void PeripheralPurchase_UpdateQuantity_UpdatesTotalPrice()
    {
        var purchase = CreateValidPurchase(quantity: 2, unitPrice: 50m);
        purchase.UpdateQuantity(5);
        Assert.Equal(5, purchase.Quantity);
        Assert.Equal(250m, purchase.TotalPrice.Amount);
    }

    [Fact]
    public void PeripheralPurchase_Confirm_ChangesStatusToConfirmed()
    {
        var purchase = CreateValidPurchase();
        purchase.Confirm();
        Assert.Equal("Confirmed", purchase.Status);
    }

    [Fact]
    public void PeripheralPurchase_MarkAsDelivered_ChangesStatusToDelivered()
    {
        var purchase = CreateValidPurchase();
        purchase.Confirm();
        purchase.MarkAsDelivered();
        Assert.Equal("Delivered", purchase.Status);
    }

    [Fact]
    public void PeripheralPurchase_Cancel_ChangesStatusToCancelled()
    {
        var purchase = CreateValidPurchase();
        purchase.Cancel();
        Assert.Equal("Cancelled", purchase.Status);
    }

    [Fact]
    public void PeripheralPurchase_Cancel_ThrowsException_WhenDelivered()
    {
        var purchase = CreateValidPurchase();
        purchase.Confirm();
        purchase.MarkAsDelivered();
        Assert.Throws<DomainException>(() => purchase.Cancel());
    }
}
