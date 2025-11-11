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
    
    public CampsiteId CampsiteId => _campsiteId;
    public string Name => _name;
    public string Description => _description;
    public string IconUrl => _iconUrl;
    public string Category => _category;
    public bool IsAvailable => _isAvailable;
    public DateTime CreatedDate => _createdDate;
    public DateTime UpdatedDate => _updatedDate;
    
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
