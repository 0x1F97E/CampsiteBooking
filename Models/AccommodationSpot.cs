namespace CampsiteBooking.Models;

/// <summary>
/// AccommodationSpot entity representing a specific bookable spot at a campsite
/// </summary>
public class AccommodationSpot
{
    private string _spotId = string.Empty;
    public string SpotId
    {
        get => _spotId;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SpotId cannot be empty", nameof(SpotId));
            _spotId = value;
        }
    }
    
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
    
    public string CampsiteName { get; set; } = string.Empty;
    
    private int _accommodationTypeId;
    public int AccommodationTypeId
    {
        get => _accommodationTypeId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("AccommodationTypeId must be greater than 0", nameof(AccommodationTypeId));
            _accommodationTypeId = value;
        }
    }
    
    private double _latitude;
    public double Latitude
    {
        get => _latitude;
        set
        {
            if (value < -90 || value > 90)
                throw new ArgumentException("Latitude must be between -90 and 90", nameof(Latitude));
            _latitude = value;
        }
    }
    
    private double _longitude;
    public double Longitude
    {
        get => _longitude;
        set
        {
            if (value < -180 || value > 180)
                throw new ArgumentException("Longitude must be between -180 and 180", nameof(Longitude));
            _longitude = value;
        }
    }
    
    public string Type { get; set; } = string.Empty; // Tent, Caravan, Cabin, Premium
    
    private string _status = "Available";
    public string Status
    {
        get => _status;
        set
        {
            var validStatuses = new[] { "Available", "Occupied", "Reserved", "Maintenance" };
            if (!validStatuses.Contains(value))
                throw new ArgumentException($"Status must be one of: {string.Join(", ", validStatuses)}", nameof(Status));
            _status = value;
        }
    }
    
    private decimal _priceModifier = 1.0m;
    public decimal PriceModifier
    {
        get => _priceModifier;
        set
        {
            if (value <= 0)
                throw new ArgumentException("PriceModifier must be positive", nameof(PriceModifier));
            _priceModifier = value;
        }
    }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Marks the spot as available
    /// </summary>
    public void MarkAsAvailable()
    {
        Status = "Available";
    }
    
    /// <summary>
    /// Marks the spot as occupied
    /// </summary>
    public void MarkAsOccupied()
    {
        Status = "Occupied";
    }
    
    /// <summary>
    /// Marks the spot as reserved
    /// </summary>
    public void MarkAsReserved()
    {
        Status = "Reserved";
    }
    
    /// <summary>
    /// Marks the spot as under maintenance
    /// </summary>
    public void MarkAsMaintenance()
    {
        Status = "Maintenance";
    }
    
    /// <summary>
    /// Checks if the spot is available for booking
    /// </summary>
    public bool IsAvailableForBooking()
    {
        return Status == "Available";
    }
}

