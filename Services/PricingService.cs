using CampsiteBooking.Data;
using CampsiteBooking.Models;
using CampsiteBooking.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CampsiteBooking.Services;

/// <summary>
/// Service for calculating prices with database-driven seasonal multipliers
/// </summary>
public class PricingService : IPricingService
{
    private readonly IDbContextFactory<CampsiteBookingDbContext> _dbContextFactory;
    private readonly ILogger<PricingService> _logger;

    public PricingService(
        IDbContextFactory<CampsiteBookingDbContext> dbContextFactory,
        ILogger<PricingService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<PricingBreakdown> CalculatePriceAsync(
        int campsiteId,
        int accommodationTypeId,
        decimal basePricePerNight,
        DateTime checkInDate,
        DateTime checkOutDate,
        decimal spotPriceModifier = 1.0m,
        CancellationToken cancellationToken = default)
    {
        if (checkOutDate <= checkInDate)
        {
            throw new ArgumentException("Check-out date must be after check-in date");
        }

        if (basePricePerNight <= 0)
        {
            throw new ArgumentException("Base price per night must be positive");
        }

        // Load active seasonal pricing from database for this campsite and accommodation type
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Use EF.Property to reference private backing fields since public properties are ignored in EF Core config
        var targetCampsiteId = CampsiteId.Create(campsiteId);
        var targetAccommodationTypeId = AccommodationTypeId.Create(accommodationTypeId);

        var seasonalPricings = await context.SeasonalPricings
            .Where(sp => EF.Property<bool>(sp, "_isActive"))
            .Where(sp => EF.Property<CampsiteId>(sp, "_campsiteId") == targetCampsiteId)
            .Where(sp => EF.Property<AccommodationTypeId>(sp, "_accommodationTypeId") == targetAccommodationTypeId)
            .ToListAsync(cancellationToken);

        _logger.LogInformation(
            "🔍 PricingService: Loaded {Count} seasonal pricing records for Campsite {CampsiteId}, AccommodationType {AccommodationTypeId}",
            seasonalPricings.Count, campsiteId, accommodationTypeId);

        foreach (var sp in seasonalPricings)
        {
            _logger.LogInformation(
                "   📋 Season: {SeasonName}, Multiplier: {Multiplier:F2}, Dates: {Start:d} - {End:d}",
                sp.SeasonName, sp.PriceMultiplier, sp.StartDate, sp.EndDate);
        }

        // Calculate price for each night
        var breakdown = new PricingBreakdown
        {
            BasePricePerNight = basePricePerNight,
            SpotPriceModifier = spotPriceModifier,
            NumberOfNights = (checkOutDate.Date - checkInDate.Date).Days,
            NightlyBreakdown = new List<NightPricing>()
        };

        decimal totalPrice = 0m;
        var currentDate = checkInDate.Date;

        while (currentDate < checkOutDate.Date)
        {
            // Find applicable seasonal pricing for this date
            var applicableSeason = seasonalPricings
                .FirstOrDefault(sp => sp.IsDateInSeason(currentDate));

            var seasonName = applicableSeason?.SeasonName ?? "Regular";
            var seasonalMultiplier = applicableSeason?.PriceMultiplier ?? 1.0m;

            // Calculate night price: base * spot modifier * seasonal multiplier
            var nightPrice = basePricePerNight * spotPriceModifier * seasonalMultiplier;

            breakdown.NightlyBreakdown.Add(new NightPricing
            {
                Date = currentDate,
                SeasonName = seasonName,
                SeasonalMultiplier = seasonalMultiplier,
                Price = nightPrice
            });

            totalPrice += nightPrice;
            currentDate = currentDate.AddDays(1);
        }

        breakdown.TotalPrice = totalPrice;
        breakdown.AverageMultiplier = breakdown.NumberOfNights > 0
            ? breakdown.NightlyBreakdown.Average(n => n.SeasonalMultiplier)
            : 1.0m;

        _logger.LogInformation(
            "Calculated pricing for {Nights} nights: Total={Total:C}, AvgMultiplier={Multiplier:F2}, SpansMultipleSeasons={SpansMultiple}",
            breakdown.NumberOfNights, breakdown.TotalPrice, breakdown.AverageMultiplier, breakdown.SpansMultipleSeasons);

        return breakdown;
    }
}

