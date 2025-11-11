namespace CampsiteBooking.Models;

public class Amenity
{
    public int AmenityId { get; set; }

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

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be empty", nameof(Name));
            _name = value;
        }
    }

    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string Category { get; set; } = "General"; // General, Facilities, Activities, Services
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Business methods
    public void MarkAsAvailable()
    {
        IsAvailable = true;
        UpdatedDate = DateTime.UtcNow;
    }

    public void MarkAsUnavailable()
    {
        IsAvailable = false;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateDetails(string name, string description, string iconUrl, string category)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        Description = description ?? string.Empty;
        IconUrl = iconUrl ?? string.Empty;
        Category = category ?? "General";
        UpdatedDate = DateTime.UtcNow;
    }
}

