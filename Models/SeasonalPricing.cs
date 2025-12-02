using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// SeasonalPricing entity representing seasonal price adjustments
/// Part of the Campsite aggregate
/// </summary>
public class SeasonalPricing : Entity<SeasonalPricingId>
{
    private CampsiteId _campsiteId = null!;
    private AccommodationTypeId _accommodationTypeId = null!;
    private string _seasonName = string.Empty;
    private DateTime _startDate;
    private DateTime _endDate;
    private decimal _priceMultiplier = 1.0m;
    private bool _isActive;
    private DateTime _createdDate;
    private DateTime _updatedDate;
    
    public CampsiteId CampsiteId => _campsiteId;
    public AccommodationTypeId AccommodationTypeId => _accommodationTypeId;
    public string SeasonName => _seasonName;
    public DateTime StartDate => _startDate;
    public DateTime EndDate => _endDate;
    public decimal PriceMultiplier => _priceMultiplier;
    public bool IsActive => _isActive;
    public DateTime CreatedDate => _createdDate;
    public DateTime UpdatedDate => _updatedDate;
    
    public int SeasonalPricingId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.SeasonalPricingId.Create(value) : ValueObjects.SeasonalPricingId.CreateNew();
    }
    
    public static SeasonalPricing Create(
        CampsiteId campsiteId,
        AccommodationTypeId accommodationTypeId,
        string seasonName,
        DateTime startDate,
        DateTime endDate,
        decimal priceMultiplier)
    {
        if (string.IsNullOrWhiteSpace(seasonName))
            throw new DomainException("Season name cannot be empty");
        
        if (endDate.Date <= startDate.Date)
            throw new DomainException("End date must be after start date");
        
        if (priceMultiplier <= 0)
            throw new DomainException("Price multiplier must be greater than 0");
        
        return new SeasonalPricing
        {
            Id = ValueObjects.SeasonalPricingId.CreateNew(),
            _campsiteId = campsiteId,
            _accommodationTypeId = accommodationTypeId,
            _seasonName = seasonName.Trim(),
            _startDate = startDate.Date,
            _endDate = endDate.Date,
            _priceMultiplier = priceMultiplier,
            _isActive = true,
            _createdDate = DateTime.UtcNow,
            _updatedDate = DateTime.UtcNow
        };
    }
    
    private SeasonalPricing() { }
    
    public bool IsDateInSeason(DateTime date)
    {
        var checkDate = date.Date;
        return checkDate >= _startDate.Date && checkDate <= _endDate.Date;
    }
    
    public decimal CalculatePrice(decimal basePrice)
    {
        if (basePrice <= 0)
            throw new DomainException("Base price must be positive");
        
        return basePrice * _priceMultiplier;
    }
    
    public void Activate()
    {
        _isActive = true;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        _isActive = false;
        _updatedDate = DateTime.UtcNow;
    }
    
    public int GetSeasonDuration() => (_endDate.Date - _startDate.Date).Days + 1;
    
    public void UpdatePriceMultiplier(decimal newMultiplier)
    {
        if (newMultiplier <= 0)
            throw new DomainException("Price multiplier must be greater than 0");

        _priceMultiplier = newMultiplier;
        _updatedDate = DateTime.UtcNow;
    }

    public void UpdateDates(DateTime newStartDate, DateTime newEndDate)
    {
        if (newEndDate.Date <= newStartDate.Date)
            throw new DomainException("End date must be after start date");

        _startDate = newStartDate.Date;
        _endDate = newEndDate.Date;
        _updatedDate = DateTime.UtcNow;
    }
}

