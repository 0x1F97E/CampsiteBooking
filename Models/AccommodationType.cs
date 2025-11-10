namespace CampsiteBooking.Models;

/// <summary>
/// AccommodationType entity representing a type of accommodation at a campsite
/// </summary>
public class AccommodationType
{
    public int AccommodationTypeId { get; set; }
    
    private int _campsiteId;
    public int CampsiteId
    {
        get => _campsiteId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("CampsiteId must be greater than 0", nameof(CampsiteId));
            _campsiteId = value;
        }
    }
    
    public string Type { get; set; } = string.Empty; // Cabin, Tent Site, RV Spot, Glamping
    public string Description { get; set; } = string.Empty;
    
    private int _maxCapacity;
    public int MaxCapacity
    {
        get => _maxCapacity;
        set
        {
            if (value <= 0)
                throw new ArgumentException("MaxCapacity must be greater than 0", nameof(MaxCapacity));
            _maxCapacity = value;
        }
    }
    
    private decimal _basePrice;
    public decimal BasePrice
    {
        get => _basePrice;
        set
        {
            if (value <= 0)
                throw new ArgumentException("BasePrice must be positive", nameof(BasePrice));
            _basePrice = value;
        }
    }
    
    public string ImageUrl { get; set; } = string.Empty;
    
    private int _availableUnits;
    public int AvailableUnits
    {
        get => _availableUnits;
        set
        {
            if (value < 0)
                throw new ArgumentException("AvailableUnits cannot be negative", nameof(AvailableUnits));
            _availableUnits = value;
        }
    }
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Activates the accommodation type
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }
    
    /// <summary>
    /// Deactivates the accommodation type
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }
    
    /// <summary>
    /// Reserves units
    /// </summary>
    public void ReserveUnits(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be positive", nameof(count));
        
        if (count > AvailableUnits)
            throw new InvalidOperationException("Not enough available units");
        
        AvailableUnits -= count;
    }
    
    /// <summary>
    /// Releases units
    /// </summary>
    public void ReleaseUnits(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be positive", nameof(count));
        
        AvailableUnits += count;
    }
}

