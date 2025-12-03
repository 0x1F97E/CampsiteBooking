using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// Discount entity representing promotional discount codes
/// Can be a standalone aggregate root or part of a Marketing aggregate
/// </summary>
public class Discount : Entity<DiscountId>
{
    private string _code = string.Empty;
    private string _description = string.Empty;
    private string _type = "Percentage";
    private decimal _value;
    private DateTime _validFrom;
    private DateTime _validUntil;
    private int _usedCount;
    private int _maxUses;
    private decimal _minimumBookingAmount;
    private readonly List<int> _applicableCampsites = new();
    private readonly List<string> _applicableAccommodationTypes = new();
    private bool _isActive;
    private DateTime _createdDate;
    
    public string Code => _code;
    public string Description => _description;
    public string Type => _type;
    public decimal Value => _value;
    public DateTime ValidFrom => _validFrom;
    public DateTime ValidUntil => _validUntil;
    public int UsedCount => _usedCount;
    public int MaxUses => _maxUses;
    public decimal MinimumBookingAmount => _minimumBookingAmount;
    public IReadOnlyList<int> ApplicableCampsites => _applicableCampsites.AsReadOnly();
    public IReadOnlyList<string> ApplicableAccommodationTypes => _applicableAccommodationTypes.AsReadOnly();
    public bool IsActive => _isActive;
    public DateTime CreatedDate => _createdDate;
    
    public int DiscountId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.DiscountId.Create(value) : ValueObjects.DiscountId.CreateNew();
    }
    
    public static Discount Create(
        string code,
        string description,
        string type,
        decimal value,
        DateTime validFrom,
        DateTime validUntil,
        int maxUses = 0,
        decimal minimumBookingAmount = 0)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Code cannot be empty");
        
        var validTypes = new[] { "Percentage", "Fixed" };
        if (!validTypes.Contains(type))
            throw new DomainException("Type must be Percentage or Fixed");
        
        if (value <= 0)
            throw new DomainException("Value must be greater than 0");
        
        if (type == "Percentage" && value > 100)
            throw new DomainException("Percentage value cannot exceed 100");
        
        if (validUntil.Date < validFrom.Date)
            throw new DomainException("Valid until must be on or after valid from");
        
        if (maxUses < 0)
            throw new DomainException("Max uses cannot be negative (use 0 for unlimited)");
        
        if (minimumBookingAmount < 0)
            throw new DomainException("Minimum booking amount cannot be negative");
        
        return new Discount
        {
            Id = ValueObjects.DiscountId.CreateNew(),
            _code = code.ToUpper().Trim(),
            _description = description?.Trim() ?? string.Empty,
            _type = type,
            _value = value,
            _validFrom = validFrom.Date,
            _validUntil = validUntil.Date,
            _usedCount = 0,
            _maxUses = maxUses,
            _minimumBookingAmount = minimumBookingAmount,
            _isActive = true,
            _createdDate = DateTime.UtcNow
        };
    }
    
    private Discount() { }
    
    public bool IsValid(DateTime bookingDate)
    {
        var checkDate = bookingDate.Date;
        return _isActive 
            && checkDate >= _validFrom.Date 
            && checkDate <= _validUntil.Date
            && (_maxUses == 0 || _usedCount < _maxUses);
    }
    
    public decimal CalculateDiscount(decimal bookingAmount)
    {
        if (bookingAmount < _minimumBookingAmount)
            throw new DomainException($"Booking amount must be at least {_minimumBookingAmount}");
        
        if (_type == "Percentage")
            return bookingAmount * (_value / 100);
        else
            return Math.Min(_value, bookingAmount);
    }
    
    public void IncrementUsage()
    {
        if (_maxUses > 0 && _usedCount >= _maxUses)
            throw new DomainException("Discount has reached maximum usage limit");

        _usedCount++;

        // Auto-deactivate when max uses is reached
        if (_maxUses > 0 && _usedCount >= _maxUses)
        {
            _isActive = false;
        }
    }
    
    public void Activate() => _isActive = true;
    public void Deactivate() => _isActive = false;
    
    public void AddApplicableCampsite(int campsiteId)
    {
        if (!_applicableCampsites.Contains(campsiteId))
            _applicableCampsites.Add(campsiteId);
    }
    
    public void AddApplicableAccommodationType(string accommodationType)
    {
        if (!_applicableAccommodationTypes.Contains(accommodationType))
            _applicableAccommodationTypes.Add(accommodationType);
    }
}

