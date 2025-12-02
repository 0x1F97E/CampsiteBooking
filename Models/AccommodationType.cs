using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// AccommodationType entity representing a type of accommodation at a campsite
/// Part of the Campsite aggregate
/// </summary>
public class AccommodationType : Entity<AccommodationTypeId>
{
    // ============================================================================
    // PRIVATE FIELDS
    // ============================================================================
    
    private CampsiteId _campsiteId = null!;
    private string _type = string.Empty;
    private string _description = string.Empty;
    private int _maxCapacity;
    private Money _basePrice = null!;
    private string _imageUrl = string.Empty;
    private int _availableUnits;
    private bool _isActive;
    private DateTime _createdDate;
    private string _amenities = string.Empty; // Comma-separated list of amenities
    private string _unitNamingPrefix = string.Empty;
    private string _unitNamingPattern = "Numbers"; // "Letters", "Numbers", "PrefixNumbers"
    private decimal? _areaSquareMeters; // Optional area in square meters
    
    // ============================================================================
    // PUBLIC PROPERTIES (Read-only)
    // ============================================================================
    
    public CampsiteId CampsiteId => _campsiteId;
    public string Type => _type;
    public string Description => _description;
    public int MaxCapacity => _maxCapacity;
    public Money BasePrice => _basePrice;
    public string ImageUrl => _imageUrl;
    public int AvailableUnits => _availableUnits;
    public bool IsActive => _isActive;
    public DateTime CreatedDate => _createdDate;
    public string Amenities => _amenities;
    public string UnitNamingPrefix => _unitNamingPrefix;
    public string UnitNamingPattern => _unitNamingPattern;
    public decimal? AreaSquareMeters => _areaSquareMeters;
    
    // ============================================================================
    // LEGACY PROPERTIES (for EF Core backward compatibility)
    // ============================================================================
    
    public int AccommodationTypeId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.AccommodationTypeId.Create(value) : ValueObjects.AccommodationTypeId.CreateNew();
    }
    
    public decimal BasePrice_Legacy
    {
        get => _basePrice?.Amount ?? 0;
        set => _basePrice = Money.Create(value, "DKK");
    }
    
    // ============================================================================
    // FACTORY METHOD
    // ============================================================================
    
    public static AccommodationType Create(
        CampsiteId campsiteId,
        string type,
        int maxCapacity,
        Money basePrice,
        int availableUnits,
        string description = "",
        string imageUrl = "")
    {
        // Validate business rules
        if (string.IsNullOrWhiteSpace(type))
            throw new DomainException("Accommodation type cannot be empty");

        if (maxCapacity <= 0)
            throw new DomainException("Max capacity must be greater than 0");

        if (availableUnits < 0)
            throw new DomainException("Available units cannot be negative");

        var accommodationType = new AccommodationType
        {
            Id = ValueObjects.AccommodationTypeId.CreateNew(),
            _campsiteId = campsiteId,
            _type = type.Trim(),
            _description = description?.Trim() ?? string.Empty,
            _maxCapacity = maxCapacity,
            _basePrice = basePrice,
            _imageUrl = imageUrl?.Trim() ?? string.Empty,
            _availableUnits = availableUnits,
            _isActive = true,
            _createdDate = DateTime.UtcNow
        };

        return accommodationType;
    }
    
    // ============================================================================
    // CONSTRUCTORS
    // ============================================================================
    
    /// <summary>
    /// Protected constructor for EF Core
    /// </summary>
    private AccommodationType()
    {
    }
    
    // ============================================================================
    // DOMAIN BEHAVIOR
    // ============================================================================
    
    public void Activate()
    {
        if (_isActive)
            throw new DomainException("Accommodation type is already active");
        
        _isActive = true;
    }
    
    public void Deactivate()
    {
        if (!_isActive)
            throw new DomainException("Accommodation type is already inactive");
        
        _isActive = false;
    }
    
    public void ReserveUnits(int count)
    {
        if (count <= 0)
            throw new DomainException("Count must be positive");
        
        if (count > _availableUnits)
            throw new DomainException("Not enough available units");
        
        _availableUnits -= count;
    }
    
    public void ReleaseUnits(int count)
    {
        if (count <= 0)
            throw new DomainException("Count must be positive");
        
        _availableUnits += count;
    }
    
    public void UpdateInformation(string description, string imageUrl)
    {
        _description = description?.Trim() ?? string.Empty;
        _imageUrl = imageUrl?.Trim() ?? string.Empty;
    }

    public void UpdateAmenities(List<string> amenities)
    {
        if (amenities == null)
        {
            _amenities = string.Empty;
            return;
        }

        // Store as comma-separated string
        _amenities = string.Join(",", amenities.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Trim()));
    }

    public List<string> GetAmenitiesList()
    {
        if (string.IsNullOrWhiteSpace(_amenities))
            return new List<string>();

        return _amenities.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(a => a.Trim())
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .ToList();
    }

    public void UpdateUnitNaming(string prefix, string pattern)
    {
        _unitNamingPrefix = prefix?.Trim() ?? string.Empty;

        // Validate pattern
        if (!string.IsNullOrWhiteSpace(pattern) &&
            (pattern == "Letters" || pattern == "Numbers" || pattern == "PrefixNumbers"))
        {
            _unitNamingPattern = pattern;
        }
        else
        {
            _unitNamingPattern = "Numbers"; // Default
        }
    }

    /// <summary>
    /// Updates the area in square meters for this accommodation type.
    /// </summary>
    /// <param name="areaSquareMeters">The area in square meters, or null to clear the value.</param>
    /// <exception cref="DomainException">Thrown when the area is negative.</exception>
    public void UpdateAreaSquareMeters(decimal? areaSquareMeters)
    {
        if (areaSquareMeters.HasValue && areaSquareMeters.Value < 0)
        {
            throw new DomainException("Area in square meters cannot be negative");
        }

        _areaSquareMeters = areaSquareMeters;
    }

    /// <summary>
    /// Updates the base price for this accommodation type.
    /// </summary>
    /// <param name="basePrice">The new base price.</param>
    /// <exception cref="DomainException">Thrown when the price is null or negative.</exception>
    public void UpdateBasePrice(Money basePrice)
    {
        if (basePrice == null)
        {
            throw new DomainException("Base price cannot be null");
        }

        if (basePrice.Amount < 0)
        {
            throw new DomainException("Base price cannot be negative");
        }

        _basePrice = basePrice;
    }

    /// <summary>
    /// Updates the maximum capacity for this accommodation type.
    /// </summary>
    /// <param name="maxCapacity">The new maximum capacity.</param>
    /// <exception cref="DomainException">Thrown when the capacity is less than 1.</exception>
    public void UpdateMaxCapacity(int maxCapacity)
    {
        if (maxCapacity < 1)
        {
            throw new DomainException("Max capacity must be at least 1");
        }

        _maxCapacity = maxCapacity;
    }
}

