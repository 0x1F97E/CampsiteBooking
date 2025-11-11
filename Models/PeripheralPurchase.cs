using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

public class PeripheralPurchase : Entity<PeripheralPurchaseId>
{
    private BookingId _bookingId = null!;
    private string _itemName = string.Empty;
    private string _description = string.Empty;
    private int _quantity = 1;
    private Money _unitPrice = null!;
    private Money _totalPrice = null!;
    private DateTime _purchaseDate;
    private string _status = "Pending";
    
    public BookingId BookingId => _bookingId;
    public string ItemName => _itemName;
    public string Description => _description;
    public int Quantity => _quantity;
    public Money UnitPrice => _unitPrice;
    public Money TotalPrice => _totalPrice;
    public DateTime PurchaseDate => _purchaseDate;
    public string Status => _status;
    
    public int PeripheralPurchaseId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.PeripheralPurchaseId.Create(value) : ValueObjects.PeripheralPurchaseId.CreateNew();
    }
    
    public static PeripheralPurchase Create(BookingId bookingId, string itemName, string description, int quantity, Money unitPrice)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Item name cannot be empty");
        
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than 0");
        
        var totalPrice = Money.Create(unitPrice.Amount * quantity, unitPrice.Currency);
        
        return new PeripheralPurchase
        {
            Id = ValueObjects.PeripheralPurchaseId.CreateNew(),
            _bookingId = bookingId,
            _itemName = itemName.Trim(),
            _description = description?.Trim() ?? string.Empty,
            _quantity = quantity,
            _unitPrice = unitPrice,
            _totalPrice = totalPrice,
            _purchaseDate = DateTime.UtcNow,
            _status = "Pending"
        };
    }
    
    private PeripheralPurchase() { }
    
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new DomainException("Quantity must be greater than 0");
        
        _quantity = newQuantity;
        _totalPrice = Money.Create(_unitPrice.Amount * _quantity, _unitPrice.Currency);
    }
    
    public void Confirm()
    {
        if (_status != "Pending")
            throw new DomainException("Only pending purchases can be confirmed");
        _status = "Confirmed";
    }
    
    public void MarkAsDelivered()
    {
        if (_status != "Confirmed")
            throw new DomainException("Only confirmed purchases can be marked as delivered");
        _status = "Delivered";
    }
    
    public void Cancel()
    {
        if (_status == "Delivered")
            throw new DomainException("Cannot cancel delivered purchases");
        _status = "Cancelled";
    }
}
