namespace CampsiteBooking.Models;

public class InactivePeriod
{
    public int Id { get; set; }
    public int CampsiteId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public InactivePeriodType Type { get; set; } = InactivePeriodType.EntireCampsite;
    public List<string> AffectedSpotIds { get; set; } = new(); // Empty if entire campsite, otherwise specific spot IDs
}

public enum InactivePeriodType
{
    EntireCampsite,
    SpecificSpots
}

