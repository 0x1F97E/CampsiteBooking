using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// AccommodationSpot entity representing a specific bookable spot at a campsite
/// Part of the Campsite aggregate
/// </summary>
public class AccommodationSpot : Entity<AccommodationSpotId>
{
    // ============================================================================
    // PRIVATE FIELDS
    // ============================================================================
    
    private string _spotIdentifier = string.Empty; // e.g., "A1", "B2"
    private CampsiteId _campsiteId = null!;
    private string _campsiteName = string.Empty;
    private AccommodationTypeId _accommodationTypeId = null!;
    private double _latitude;
    private double _longitude;
    private string _type = string.Empty;
    private SpotStatus _status = SpotStatus.Available;
    private decimal _priceModifier = 1.0m;
    private DateTime _createdDate;
    
    // ============================================================================
    // PUBLIC PROPERTIES (Read-only)
    // ============================================================================
    
    public string SpotIdentifier => _spotIdentifier;
    public CampsiteId CampsiteId => _campsiteId;
    public string CampsiteName => _campsiteName;
    public AccommodationTypeId AccommodationTypeId => _accommodationTypeId;
    public double Latitude => _latitude;
    public double Longitude => _longitude;
    public string Type => _type;
    public SpotStatus Status => _status;
    public decimal PriceModifier => _priceModifier;
    public DateTime CreatedDate => _createdDate;
    
    // ============================================================================
    // LEGACY PROPERTIES (for EF Core backward compatibility)
    // ============================================================================
    
    public string SpotId
    {
        get => _spotIdentifier;
        set => _spotIdentifier = value;
    }
    
    public string Status_Legacy
    {
        get => _status.ToString();
        set => _status = Enum.Parse<SpotStatus>(value);
    }
    
    // ============================================================================
    // FACTORY METHOD
    // ============================================================================
    
    public static AccommodationSpot Create(
        string spotIdentifier,
        CampsiteId campsiteId,
        string campsiteName,
        AccommodationTypeId accommodationTypeId,
        string type,
        double latitude,
        double longitude,
        decimal priceModifier = 1.0m)
    {
        // Validate business rules
        if (string.IsNullOrWhiteSpace(spotIdentifier))
            throw new DomainException("Spot identifier cannot be empty");
        
        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Spot type cannot be empty");
        
        var validTypes = new[] { "Tent", "Caravan", "Cabin", "Premium" };
        if (!validTypes.Contains(type))
            throw new DomainException("Type must be Tent, Caravan, Cabin, or Premium");
        
        if (latitude < -90 || latitude > 90)
            throw new DomainException("Latitude must be between -90 and 90");
        
        if (longitude < -180 || longitude > 180)
            throw new DomainException("Longitude must be between -180 and 180");
        
        if (priceModifier <= 0)
            throw new DomainException("Price modifier must be positive");
        
        var spot = new AccommodationSpot
        {
            Id = ValueObjects.AccommodationSpotId.CreateNew(),
            _spotIdentifier = spotIdentifier.Trim(),
            _campsiteId = campsiteId,
            _campsiteName = campsiteName?.Trim() ?? string.Empty,
            _accommodationTypeId = accommodationTypeId,
            _type = type,
            _latitude = latitude,
            _longitude = longitude,
            _status = SpotStatus.Available,
            _priceModifier = priceModifier,
            _createdDate = DateTime.UtcNow
        };
        
        return spot;
    }
    
    // ============================================================================
    // CONSTRUCTORS
    // ============================================================================
    
    /// <summary>
    /// Protected constructor for EF Core
    /// </summary>
    private AccommodationSpot()
    {
    }
    
    // ============================================================================
    // DOMAIN BEHAVIOR
    // ============================================================================
    
    public void MarkAsAvailable()
    {
        _status = SpotStatus.Available;
    }
    
    public void MarkAsOccupied()
    {
        if (_status == SpotStatus.Maintenance)
            throw new DomainException("Cannot occupy a spot under maintenance");
        
        _status = SpotStatus.Occupied;
    }
    
    public void MarkAsReserved()
    {
        if (_status == SpotStatus.Maintenance)
            throw new DomainException("Cannot reserve a spot under maintenance");
        
        _status = SpotStatus.Reserved;
    }
    
    public void MarkAsMaintenance()
    {
        _status = SpotStatus.Maintenance;
    }
    
    public bool IsAvailableForBooking()
    {
        return _status == SpotStatus.Available;
    }
    
    public void UpdatePriceModifier(decimal priceModifier)
    {
        if (priceModifier <= 0)
            throw new DomainException("Price modifier must be positive");
        
        _priceModifier = priceModifier;
    }
}

/// <summary>
/// Spot status value object
/// </summary>
public enum SpotStatus
{
    Available,
    Occupied,
    Reserved,
    Maintenance
}

