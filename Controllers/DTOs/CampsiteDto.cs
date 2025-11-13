namespace CampsiteBooking.Controllers;

/// <summary>
/// Data Transfer Object for Campsite responses.
/// </summary>
public class CampsiteDto
{
    public int CampsiteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsActive { get; set; }
}

