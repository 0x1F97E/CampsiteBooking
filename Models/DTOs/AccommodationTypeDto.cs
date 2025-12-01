namespace CampsiteBooking.Models.DTOs;

public class AccommodationTypeDto
{
    public int Id { get; set; }
    public int CampsiteId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public decimal PricePerNight { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty; // Custom icon for this accommodation type
    public List<string> Amenities { get; set; } = new();
    public List<AccommodationSpotDto> Spots { get; set; } = new();
    public List<string> GalleryImages { get; set; } = new();
    public bool IsActive { get; set; } = true;
    public int AvailableUnits { get; set; }
    public string UnitNamingPrefix { get; set; } = string.Empty;
    public string UnitNamingPattern { get; set; } = "Numbers"; // "Letters", "Numbers", "PrefixNumbers"
    public decimal? AreaSquareMeters { get; set; } // Optional area in square meters
}

public class AccommodationSpotDto
{
    public int Id { get; set; }
    public string SpotNumber { get; set; } = string.Empty; // e.g., "1", "2", "3"
    public string Name { get; set; } = string.Empty; // e.g., "A1", "B2", "Cabin-1"
    public bool IsAvailable { get; set; } = true;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsUnderMaintenance { get; set; } = false; // When true, spot is hidden from search results
}

