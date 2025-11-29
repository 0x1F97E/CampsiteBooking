using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

public class Amenity : Entity<AmenityId>
{
    private CampsiteId _campsiteId = null!;
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _iconUrl = string.Empty;
    private string _category = "General";
    private bool _isAvailable;
    private DateTime _createdDate;
    private DateTime _updatedDate;

    public CampsiteId CampsiteId { get => _campsiteId; private set => _campsiteId = value; }
    public string Name { get => _name; private set => _name = value; }
    public string Description { get => _description; private set => _description = value; }
    public string IconUrl { get => _iconUrl; private set => _iconUrl = value; }
    public string Category { get => _category; private set => _category = value; }
    public bool IsAvailable { get => _isAvailable; private set => _isAvailable = value; }
    public DateTime CreatedDate { get => _createdDate; private set => _createdDate = value; }
    public DateTime UpdatedDate { get => _updatedDate; private set => _updatedDate = value; }
    
    public int AmenityId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.AmenityId.Create(value) : ValueObjects.AmenityId.CreateNew();
    }
    
    public static Amenity Create(CampsiteId campsiteId, string name, string description, string iconUrl, string category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");
        
        var validCategories = new[] { "General", "Facilities", "Activities", "Services" };
        if (!validCategories.Contains(category))
            throw new DomainException("Category must be General, Facilities, Activities, or Services");
        
        return new Amenity
        {
            Id = ValueObjects.AmenityId.CreateNew(),
            _campsiteId = campsiteId,
            _name = name.Trim(),
            _description = description?.Trim() ?? string.Empty,
            _iconUrl = iconUrl?.Trim() ?? string.Empty,
            _category = category,
            _isAvailable = true,
            _createdDate = DateTime.UtcNow,
            _updatedDate = DateTime.UtcNow
        };
    }
    
    private Amenity() { }
    
    public void MarkAsAvailable()
    {
        _isAvailable = true;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void MarkAsUnavailable()
    {
        _isAvailable = false;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void UpdateDetails(string name, string description, string iconUrl, string category)
    {
        if (!string.IsNullOrWhiteSpace(name))
            _name = name.Trim();
        
        _description = description?.Trim() ?? string.Empty;
        _iconUrl = iconUrl?.Trim() ?? string.Empty;
        
        var validCategories = new[] { "General", "Facilities", "Activities", "Services" };
        if (validCategories.Contains(category))
            _category = category;
        
        _updatedDate = DateTime.UtcNow;
    }
}
