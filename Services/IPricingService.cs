using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Services;

/// <summary>
/// Service interface for calculating prices with seasonal multipliers
/// </summary>
public interface IPricingService
{
    /// <summary>
    /// Calculates the total accommodation price for a date range, applying seasonal multipliers
    /// </summary>
    /// <param name="campsiteId">The campsite ID</param>
    /// <param name="accommodationTypeId">The accommodation type ID</param>
    /// <param name="basePricePerNight">The base price per night for the accommodation</param>
    /// <param name="checkInDate">Check-in date</param>
    /// <param name="checkOutDate">Check-out date</param>
    /// <param name="spotPriceModifier">Optional spot-specific price modifier (default: 1.0)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Pricing breakdown with total and per-night details</returns>
    Task<PricingBreakdown> CalculatePriceAsync(
        int campsiteId,
        int accommodationTypeId,
        decimal basePricePerNight,
        DateTime checkInDate,
        DateTime checkOutDate,
        decimal spotPriceModifier = 1.0m,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a detailed pricing breakdown with seasonal multipliers applied
/// </summary>
public class PricingBreakdown
{
    /// <summary>
    /// Total price for all nights with seasonal multipliers applied
    /// </summary>
    public decimal TotalPrice { get; set; }
    
    /// <summary>
    /// Base price per night (without any modifiers)
    /// </summary>
    public decimal BasePricePerNight { get; set; }
    
    /// <summary>
    /// Number of nights in the booking
    /// </summary>
    public int NumberOfNights { get; set; }
    
    /// <summary>
    /// Average seasonal multiplier applied across all nights
    /// </summary>
    public decimal AverageMultiplier { get; set; }
    
    /// <summary>
    /// Spot-specific price modifier
    /// </summary>
    public decimal SpotPriceModifier { get; set; }
    
    /// <summary>
    /// Detailed breakdown for each night showing the seasonal multiplier applied
    /// </summary>
    public List<NightPricing> NightlyBreakdown { get; set; } = new();
    
    /// <summary>
    /// Whether multiple seasons apply to this booking
    /// </summary>
    public bool SpansMultipleSeasons => NightlyBreakdown
        .Select(n => n.SeasonName)
        .Distinct()
        .Count() > 1;
}

/// <summary>
/// Pricing details for a single night
/// </summary>
public class NightPricing
{
    /// <summary>
    /// The date of this night
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// The name of the season that applies to this night
    /// </summary>
    public string SeasonName { get; set; } = "Regular";
    
    /// <summary>
    /// The seasonal multiplier applied to this night
    /// </summary>
    public decimal SeasonalMultiplier { get; set; } = 1.0m;
    
    /// <summary>
    /// The calculated price for this night (base * spot modifier * seasonal multiplier)
    /// </summary>
    public decimal Price { get; set; }
}

