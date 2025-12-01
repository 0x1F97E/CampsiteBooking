using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// Campsite aggregate root representing a physical campsite location
/// </summary>
public class Campsite : AggregateRoot<CampsiteId>
{
    // ============================================================================
    // PRIVATE FIELDS
    // ============================================================================

    private string _name = string.Empty;
    private string _streetAddress = string.Empty;
    private string _city = string.Empty;
    private string _postalCode = string.Empty;
    private string _description = string.Empty;
    private double _latitude;
    private double _longitude;
    private string _attractiveness = "Medium";
    private string _phoneNumber = string.Empty;
    private Email? _email;
    private string _websiteUrl = string.Empty;
    private int _establishedYear;
    private bool _isActive;
    private DateTime? _seasonOpeningDate;
    private DateTime _createdDate;
    private DateTime _updatedDate;

    // ============================================================================
    // PUBLIC PROPERTIES (Read-only)
    // ============================================================================

    public string Name => _name;
    public string StreetAddress => _streetAddress;
    public string City => _city;
    public string PostalCode => _postalCode;
    public string Description => _description;
    public double Latitude => _latitude;
    public double Longitude => _longitude;
    public string Attractiveness => _attractiveness;
    public string PhoneNumber => _phoneNumber;
    public Email? Email => _email;
    public string WebsiteUrl => _websiteUrl;
    public int EstablishedYear => _establishedYear;
    public bool IsActive => _isActive;
    public DateTime? SeasonOpeningDate => _seasonOpeningDate;
    public DateTime CreatedDate => _createdDate;
    public DateTime UpdatedDate => _updatedDate;
    
    // ============================================================================
    // LEGACY PROPERTIES (for EF Core backward compatibility)
    // ============================================================================
    
    public int CampsiteId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.CampsiteId.Create(value) : ValueObjects.CampsiteId.CreateNew();
    }
    
    // ============================================================================
    // FACTORY METHOD
    // ============================================================================
    
    public static Campsite Create(
        string name,
        string streetAddress,
        string city,
        string postalCode,
        double latitude,
        double longitude,
        int establishedYear,
        string description = "",
        string attractiveness = "Medium",
        string phoneNumber = "",
        Email? email = null,
        string websiteUrl = "")
    {
        // Validate business rules
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Campsite name cannot be empty");

        if (name.Length > 200)
            throw new DomainException("Campsite name cannot exceed 200 characters");

        if (latitude < -90 || latitude > 90)
            throw new DomainException("Latitude must be between -90 and 90");

        if (longitude < -180 || longitude > 180)
            throw new DomainException("Longitude must be between -180 and 180");

        if (establishedYear > DateTime.UtcNow.Year)
            throw new DomainException("Established year cannot be in the future");

        var validAttractivenessLevels = new[] { "Low", "Medium", "High", "Very High" };
        if (!validAttractivenessLevels.Contains(attractiveness))
            throw new DomainException("Attractiveness must be Low, Medium, High, or Very High");

        var campsite = new Campsite
        {
            Id = ValueObjects.CampsiteId.CreateNew(),
            _name = name.Trim(),
            _streetAddress = streetAddress?.Trim() ?? string.Empty,
            _city = city?.Trim() ?? string.Empty,
            _postalCode = postalCode?.Trim() ?? string.Empty,
            _description = description?.Trim() ?? string.Empty,
            _latitude = latitude,
            _longitude = longitude,
            _attractiveness = attractiveness,
            _phoneNumber = phoneNumber?.Trim() ?? string.Empty,
            _email = email,
            _websiteUrl = websiteUrl?.Trim() ?? string.Empty,
            _establishedYear = establishedYear,
            _isActive = true,
            _createdDate = DateTime.UtcNow,
            _updatedDate = DateTime.UtcNow
        };

        return campsite;
    }
    
    // ============================================================================
    // CONSTRUCTORS
    // ============================================================================
    
    /// <summary>
    /// Protected constructor for EF Core
    /// </summary>
    private Campsite()
    {
    }
    
    // ============================================================================
    // DOMAIN BEHAVIOR
    // ============================================================================
    
    public void Activate()
    {
        if (_isActive)
            throw new DomainException("Campsite is already active");
        
        _isActive = true;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        if (!_isActive)
            throw new DomainException("Campsite is already inactive");

        _isActive = false;
        _updatedDate = DateTime.UtcNow;
    }

    public void SetActiveStatus(bool isActive, DateTime? seasonOpeningDate = null)
    {
        _isActive = isActive;
        _seasonOpeningDate = isActive ? null : seasonOpeningDate;
        _updatedDate = DateTime.UtcNow;
    }

    public void UpdateSeasonOpeningDate(DateTime? openingDate)
    {
        _seasonOpeningDate = openingDate;
        _updatedDate = DateTime.UtcNow;
    }

    public void UpdateInformation(
        string name,
        string streetAddress,
        string city,
        string postalCode,
        string description,
        string attractiveness,
        string phoneNumber,
        Email? email,
        string websiteUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Campsite name cannot be empty");

        if (name.Length > 200)
            throw new DomainException("Campsite name cannot exceed 200 characters");

        var validAttractivenessLevels = new[] { "Low", "Medium", "High", "Very High" };
        if (!validAttractivenessLevels.Contains(attractiveness))
            throw new DomainException("Attractiveness must be Low, Medium, High, or Very High");

        _name = name.Trim();
        _streetAddress = streetAddress?.Trim() ?? string.Empty;
        _city = city?.Trim() ?? string.Empty;
        _postalCode = postalCode?.Trim() ?? string.Empty;
        _description = description?.Trim() ?? string.Empty;
        _attractiveness = attractiveness;
        _phoneNumber = phoneNumber?.Trim() ?? string.Empty;
        _email = email;
        _websiteUrl = websiteUrl?.Trim() ?? string.Empty;
        _updatedDate = DateTime.UtcNow;
    }

    public void UpdateLocation(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new DomainException("Latitude must be between -90 and 90");

        if (longitude < -180 || longitude > 180)
            throw new DomainException("Longitude must be between -180 and 180");

        _latitude = latitude;
        _longitude = longitude;
        _updatedDate = DateTime.UtcNow;
    }
}

