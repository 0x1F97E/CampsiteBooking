namespace CampsiteBooking.Models;

public class Discount
{
    public int DiscountId { get; set; }

    private string _code = string.Empty;
    public string Code
    {
        get => _code;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Code cannot be empty", nameof(Code));
            _code = value.ToUpper(); // Store codes in uppercase for consistency
        }
    }

    public string Description { get; set; } = string.Empty;

    private string _type = "Percentage"; // Percentage or Fixed
    public string Type
    {
        get => _type;
        set
        {
            var validTypes = new[] { "Percentage", "Fixed" };
            if (!validTypes.Contains(value))
                throw new ArgumentException($"Type must be one of: {string.Join(", ", validTypes)}", nameof(Type));
            _type = value;
        }
    }

    private decimal _value;
    public decimal Value
    {
        get => _value;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Value must be greater than 0", nameof(Value));

            if (Type == "Percentage" && value > 100)
                throw new ArgumentException("Percentage value cannot exceed 100", nameof(Value));

            _value = value;
        }
    }

    private DateTime _validFrom;
    public DateTime ValidFrom
    {
        get => _validFrom;
        set => _validFrom = value.Date; // Store only the date part
    }

    private DateTime _validUntil;
    public DateTime ValidUntil
    {
        get => _validUntil;
        set
        {
            if (value.Date < ValidFrom.Date)
                throw new ArgumentException("ValidUntil must be on or after ValidFrom", nameof(ValidUntil));
            _validUntil = value.Date; // Store only the date part
        }
    }

    public int UsedCount { get; set; } = 0;

    private int _maxUses;
    public int MaxUses
    {
        get => _maxUses;
        set
        {
            if (value < 0)
                throw new ArgumentException("MaxUses cannot be negative (use 0 for unlimited)", nameof(MaxUses));
            _maxUses = value;
        }
    }

    private decimal _minimumBookingAmount;
    public decimal MinimumBookingAmount
    {
        get => _minimumBookingAmount;
        set
        {
            if (value < 0)
                throw new ArgumentException("MinimumBookingAmount cannot be negative", nameof(MinimumBookingAmount));
            _minimumBookingAmount = value;
        }
    }

    public List<int> ApplicableCampsites { get; set; } = new();
    public List<string> ApplicableAccommodationTypes { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Business methods
    public bool IsValid(DateTime bookingDate)
    {
        var checkDate = bookingDate.Date;
        return IsActive 
            && checkDate >= ValidFrom.Date 
            && checkDate <= ValidUntil.Date
            && (MaxUses == 0 || UsedCount < MaxUses);
    }

    public decimal CalculateDiscount(decimal bookingAmount)
    {
        if (bookingAmount < MinimumBookingAmount)
            throw new InvalidOperationException($"Booking amount must be at least {MinimumBookingAmount}");

        if (Type == "Percentage")
            return bookingAmount * (Value / 100);
        else // Fixed
            return Math.Min(Value, bookingAmount); // Don't discount more than the booking amount
    }

    public void IncrementUsage()
    {
        if (MaxUses > 0 && UsedCount >= MaxUses)
            throw new InvalidOperationException("Discount has reached maximum usage limit");

        UsedCount++;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}

