using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// Global lookup table for standard amenities that can be assigned to accommodation types.
/// This is a master list of available amenities across the entire system.
/// </summary>
public class AmenityLookup : Entity<AmenityLookupId>
{
    private string _name = string.Empty;
    private string _displayName = string.Empty;
    private string _category = "General";
    private string _iconClass = string.Empty;
    private bool _isActive = true;
    private int _sortOrder = 0;
    private DateTime _createdDate;
    private DateTime _updatedDate;

    public string Name { get => _name; private set => _name = value; }
    public string DisplayName { get => _displayName; private set => _displayName = value; }
    public string Category { get => _category; private set => _category = value; }
    public string IconClass { get => _iconClass; private set => _iconClass = value; }
    public bool IsActive { get => _isActive; private set => _isActive = value; }
    public int SortOrder { get => _sortOrder; private set => _sortOrder = value; }
    public DateTime CreatedDate { get => _createdDate; private set => _createdDate = value; }
    public DateTime UpdatedDate { get => _updatedDate; private set => _updatedDate = value; }
    
    public int AmenityLookupId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.AmenityLookupId.Create(value) : ValueObjects.AmenityLookupId.CreateNew();
    }
    
    public static AmenityLookup Create(string name, string displayName, string category = "General", string iconClass = "", int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Amenity name cannot be empty");
        
        if (string.IsNullOrWhiteSpace(displayName))
            throw new DomainException("Amenity display name cannot be empty");
        
        var validCategories = new[] { "General", "Utilities", "Comfort", "Facilities", "Accessibility", "Appliances", "Security" };
        if (!validCategories.Contains(category))
            throw new DomainException($"Category must be one of: {string.Join(", ", validCategories)}");
        
        return new AmenityLookup
        {
            Id = ValueObjects.AmenityLookupId.CreateNew(),
            _name = name.Trim(),
            _displayName = displayName.Trim(),
            _category = category,
            _iconClass = iconClass?.Trim() ?? string.Empty,
            _sortOrder = sortOrder,
            _isActive = true,
            _createdDate = DateTime.UtcNow,
            _updatedDate = DateTime.UtcNow
        };
    }
    
    private AmenityLookup() { }
    
    public void Activate()
    {
        _isActive = true;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        _isActive = false;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void UpdateDetails(string displayName, string category, string iconClass, int sortOrder)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
            _displayName = displayName.Trim();
        
        var validCategories = new[] { "General", "Utilities", "Comfort", "Facilities", "Accessibility", "Appliances", "Security" };
        if (validCategories.Contains(category))
            _category = category;
        
        _iconClass = iconClass?.Trim() ?? string.Empty;
        _sortOrder = sortOrder;
        _updatedDate = DateTime.UtcNow;
    }
}

