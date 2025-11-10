namespace CampsiteBooking.Models;

/// <summary>
/// Payment entity representing a payment transaction
/// </summary>
public class Payment
{
    public int PaymentId { get; set; }
    
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
    
    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Amount must be positive", nameof(Amount));
            _amount = value;
        }
    }
    
    public string Currency { get; set; } = "DKK"; // DKK, EUR, USD
    public string Method { get; set; } = string.Empty; // CreditCard, DebitCard, MobilePay, BankTransfer
    
    private string _transactionId = string.Empty;
    public string TransactionId
    {
        get => _transactionId;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("TransactionId cannot be empty", nameof(TransactionId));
            _transactionId = value;
        }
    }
    
    public string Status { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public DateTime? RefundDate { get; set; }
    
    private decimal _refundAmount;
    public decimal RefundAmount
    {
        get => _refundAmount;
        set
        {
            if (value < 0)
                throw new ArgumentException("RefundAmount cannot be negative", nameof(RefundAmount));
            
            if (value > Amount)
                throw new ArgumentException("RefundAmount cannot exceed original Amount", nameof(RefundAmount));
            
            _refundAmount = value;
        }
    }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Marks the payment as completed
    /// </summary>
    public void MarkAsCompleted()
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Only pending payments can be completed");
        
        Status = "Completed";
        PaymentDate = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Marks the payment as failed
    /// </summary>
    public void MarkAsFailed()
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Only pending payments can be marked as failed");
        
        Status = "Failed";
    }
    
    /// <summary>
    /// Processes a refund
    /// </summary>
    public void ProcessRefund(decimal amount)
    {
        if (Status != "Completed")
            throw new InvalidOperationException("Only completed payments can be refunded");
        
        if (amount <= 0)
            throw new ArgumentException("Refund amount must be positive", nameof(amount));
        
        if (amount > Amount)
            throw new ArgumentException("Refund amount cannot exceed original payment amount", nameof(amount));
        
        RefundAmount = amount;
        RefundDate = DateTime.UtcNow;
        Status = "Refunded";
    }
    
    /// <summary>
    /// Validates currency code
    /// </summary>
    public bool IsValidCurrency()
    {
        var validCurrencies = new[] { "DKK", "EUR", "USD", "GBP", "SEK", "NOK" };
        return validCurrencies.Contains(Currency);
    }
}

