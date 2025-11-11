namespace CampsiteBooking.Models;

public class SeasonalPricing
{
    public int SeasonalPricingId { get; set; }

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

    private int _accommodationTypeId;
    public int AccommodationTypeId
    {
        get => _accommodationTypeId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("AccommodationTypeId must be greater than 0", nameof(AccommodationTypeId));
            _accommodationTypeId = value;
        }
    }

    private string _seasonName = string.Empty;
    public string SeasonName
    {
        get => _seasonName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SeasonName cannot be empty", nameof(SeasonName));
            _seasonName = value;
        }
    }

    private DateTime _startDate;
    public DateTime StartDate
    {
        get => _startDate;
        set => _startDate = value.Date; // Store only the date part
    }

    private DateTime _endDate;
    public DateTime EndDate
    {
        get => _endDate;
        set
        {
            if (value.Date <= StartDate.Date)
                throw new ArgumentException("EndDate must be after StartDate", nameof(EndDate));
            _endDate = value.Date; // Store only the date part
        }
    }

    private decimal _priceMultiplier = 1.0m;
    public decimal PriceMultiplier
    {
        get => _priceMultiplier;
        set
        {
            if (value <= 0)
                throw new ArgumentException("PriceMultiplier must be greater than 0", nameof(PriceMultiplier));
            _priceMultiplier = value;
        }
    }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Business methods
    public bool IsDateInSeason(DateTime date)
    {
        var checkDate = date.Date;
        return checkDate >= StartDate.Date && checkDate <= EndDate.Date;
    }

    public decimal CalculatePrice(decimal basePrice)
    {
        if (basePrice <= 0)
            throw new ArgumentException("Base price must be positive", nameof(basePrice));

        return basePrice * PriceMultiplier;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedDate = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedDate = DateTime.UtcNow;
    }

    public int GetSeasonDuration()
    {
        return (EndDate.Date - StartDate.Date).Days + 1;
    }
}

