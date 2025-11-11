namespace CampsiteBooking.Models;

public class PeripheralPurchase
{
    public int PeripheralPurchaseId { get; set; }

    private int _bookingId;
    public int BookingId
    {
        get => _bookingId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("BookingId must be greater than 0", nameof(BookingId));
            _bookingId = value;
        }
    }

    private string _itemName = string.Empty;
    public string ItemName
    {
        get => _itemName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("ItemName cannot be empty", nameof(ItemName));
            _itemName = value;
        }
    }

    public string Description { get; set; } = string.Empty;

    private int _quantity = 1;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Quantity must be greater than 0", nameof(Quantity));
            _quantity = value;
            UpdateTotalPrice();
        }
    }

    private decimal _unitPrice;
    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (value <= 0)
                throw new ArgumentException("UnitPrice must be greater than 0", nameof(UnitPrice));
            _unitPrice = value;
            UpdateTotalPrice();
        }
    }

    public decimal TotalPrice { get; private set; }

    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Delivered, Cancelled

    // Business methods
    private void UpdateTotalPrice()
    {
        TotalPrice = Quantity * UnitPrice;
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0", nameof(newQuantity));

        Quantity = newQuantity;
    }

    public void Confirm()
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Only pending purchases can be confirmed");

        Status = "Confirmed";
    }

    public void MarkAsDelivered()
    {
        if (Status != "Confirmed")
            throw new InvalidOperationException("Only confirmed purchases can be marked as delivered");

        Status = "Delivered";
    }

    public void Cancel()
    {
        if (Status == "Delivered")
            throw new InvalidOperationException("Cannot cancel delivered purchases");

        Status = "Cancelled";
    }
}

