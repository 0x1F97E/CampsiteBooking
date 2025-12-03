namespace CampsiteBooking.Components.Pages.Admin;

public class CampsitePricingDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Attractiveness { get; set; } = string.Empty;
    public List<BasePricingDto> Pricing { get; set; } = new();
    public List<PeripheralPurchaseDto> PeripheralPurchases { get; set; } = new();
}

public class BasePricingDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int AvailableUnits { get; set; }
    public int MaxGuests { get; set; }
    public bool IsActive { get; set; } = true;
    public List<PeripheralPurchaseDto> AccommodationPeripheralPurchases { get; set; } = new();
}

public class SeasonalMultiplierDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DateRange { get; set; } = string.Empty;
    public decimal Multiplier { get; set; }
}

public class DiscountDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "Percentage";
    public decimal Value { get; set; }
    public DateTime ValidFrom { get; set; } = DateTime.Today;
    public DateTime ValidUntil { get; set; } = DateTime.Today.AddMonths(1);
    public int UsedCount { get; set; }
    public int MaxUses { get; set; }
    public decimal MinimumBookingAmount { get; set; }
    public bool IsActive { get; set; } = true;
}

public class PeripheralPurchaseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = "Activity";
    public bool IsActive { get; set; } = true;
}

