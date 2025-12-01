namespace CampsiteBooking.Models.DTOs;

/// <summary>
/// DTO for creating and editing campsites in the admin interface.
/// </summary>
public class CampsiteFormDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Attractiveness { get; set; } = "Medium";
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? WebsiteUrl { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int EstablishedYear { get; set; } = DateTime.Now.Year;
    public bool IsActive { get; set; } = true;
    public DateTime? SeasonOpeningDate { get; set; }
}

