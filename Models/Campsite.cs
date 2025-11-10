namespace CampsiteBooking.Models;

/// <summary>
/// Campsite entity representing a physical campsite location
/// </summary>
public class Campsite
{
    public int CampsiteId { get; set; }
    
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
    
    public string Region { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
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
    
    public string Attractiveness { get; set; } = "Medium"; // Low, Medium, High, Very High
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    
    private int _establishedYear;
    public int EstablishedYear
    {
        get => _establishedYear;
        set
        {
            if (value > DateTime.UtcNow.Year)
                throw new ArgumentException("EstablishedYear cannot be in the future", nameof(EstablishedYear));
            _establishedYear = value;
        }
    }
    
    public bool IsActive { get; set; } = true;
    
    private decimal _totalArea;
    public decimal TotalArea
    {
        get => _totalArea;
        set
        {
            if (value <= 0)
                throw new ArgumentException("TotalArea must be positive", nameof(TotalArea));
            _totalArea = value;
        }
    }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Activates the campsite
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdatedDate = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Deactivates the campsite
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdatedDate = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Updates the campsite information
    /// </summary>
    public void Update()
    {
        UpdatedDate = DateTime.UtcNow;
    }
}

