namespace CampsiteBooking.Models;

public class CampingSpot
{
    /// <summary>
    /// Database ID of the AccommodationSpot entity (used for booking)
    /// </summary>
    public int DatabaseId { get; set; }

    public string SpotId { get; set; } = string.Empty;
    public int CampsiteId { get; set; }
    public string CampsiteName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Type { get; set; } = string.Empty; // "Tent", "Caravan", "Cabin", "Premium"
    public List<string> Amenities { get; set; } = new();
    public string Status { get; set; } = "Available"; // "Available", "Occupied", "Reserved"
    public decimal PriceModifier { get; set; } = 1.0m; // 1.0 = normal, 1.5 = premium
}

