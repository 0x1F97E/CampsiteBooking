using CampsiteBooking.Data;
using CampsiteBooking.Models;
using CampsiteBooking.Models.ValueObjects;
using CampsiteBooking.Models.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace CampsiteBooking.Components.Pages.Admin;

public class PricingManagementBase : ComponentBase
{
    [Inject] protected IDialogService DialogService { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;
    [Inject] protected IDbContextFactory<CampsiteBookingDbContext> DbContextFactory { get; set; } = default!;

    protected int _selectedCampsiteId = 1;
    protected List<CampsitePricingDto> _campsites = new();
    protected List<SeasonalMultiplierDto> _seasonalMultipliers = new();
    protected List<DiscountDto> _discounts = new();
    protected bool _loading = true;

    // Preview calculator
    protected string _previewAccommodationType = "Cabin";
    protected string _previewSeason = "High Season";
    protected int _previewNights = 3;
    protected decimal _previewBase = 0;
    protected decimal _previewMultiplier = 1.5m;
    protected decimal _previewTotal = 0;

    protected override async Task OnInitializedAsync()
    {
        await LoadPricingData();
        await LoadSeasonalPricing();
        await LoadDiscounts();
        CalculatePreview();
    }

    protected async Task LoadPricingData()
    {
        _loading = true;
        try
        {
            using var context = await DbContextFactory.CreateDbContextAsync();

            // Load all campsites
            var campsiteEntities = await context.Campsites.ToListAsync();

            Console.WriteLine($"📊 Loaded {campsiteEntities.Count} campsites from database");

            // Load all accommodation types
            var allAccommodationTypes = await context.AccommodationTypes.ToListAsync();

            Console.WriteLine($"📊 Loaded {allAccommodationTypes.Count} accommodation types from database");

            // Convert to DTOs
            _campsites = campsiteEntities.Select(c =>
            {
                // Filter accommodation types for this campsite
                var campsiteAccommodationTypes = allAccommodationTypes
                    .Where(a => a.CampsiteId?.Value == c.Id?.Value)
                    .ToList();

                return new CampsitePricingDto
                {
                    Id = c.Id?.Value ?? 0,
                    Name = c.Name ?? "Unknown",
                    Region = c.Region ?? "Unknown",
                    Attractiveness = c.Attractiveness ?? "Unknown",
                    Pricing = campsiteAccommodationTypes.Select(a => new BasePricingDto
                    {
                        Id = a.Id?.Value ?? 0,
                        Type = a.Type ?? "Unknown",
                        Price = a.BasePrice?.Amount ?? 0,
                        AvailableUnits = a.AvailableUnits,
                        MaxGuests = a.MaxCapacity,
                        IsActive = a.IsActive,
                        AccommodationPeripheralPurchases = new()
                    }).ToList(),
                    PeripheralPurchases = new()
                };
            }).ToList();

            Console.WriteLine($"✅ Loaded {_campsites.Count} campsites with pricing data");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading pricing data: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Inner exception: {ex.InnerException.Message}");
            }
            Snackbar.Add($"Error loading pricing data: {ex.Message}", Severity.Error);
            _campsites = new();
            _seasonalMultipliers = new();
            _discounts = new();
        }
        finally
        {
            _loading = false;
        }
    }

    protected async Task LoadSeasonalPricing()
    {
        try
        {
            using var context = await DbContextFactory.CreateDbContextAsync();
            var seasonalPricingEntities = await context.SeasonalPricings.ToListAsync();

            Console.WriteLine($"📊 Loaded {seasonalPricingEntities.Count} seasonal pricing records from database");

            // Group by season name and create DTOs
            _seasonalMultipliers = seasonalPricingEntities
                .Where(sp => sp.IsActive)
                .GroupBy(sp => sp.SeasonName)
                .Select(g =>
                {
                    var first = g.First();
                    var dateRange = $"{first.StartDate:MMM d} - {first.EndDate:MMM d}";

                    return new SeasonalMultiplierDto
                    {
                        Id = first.Id.Value,
                        Name = first.SeasonName ?? "Unknown Season",
                        DateRange = dateRange,
                        Multiplier = first.PriceMultiplier
                    };
                })
                .ToList();

            Console.WriteLine($"✅ Loaded {_seasonalMultipliers.Count} seasonal multipliers");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading seasonal pricing: {ex.Message}");
            _seasonalMultipliers = new();
        }
    }

    protected async Task LoadDiscounts()
    {
        try
        {
            using var context = await DbContextFactory.CreateDbContextAsync();
            var discountEntities = await context.Discounts.ToListAsync();

            Console.WriteLine($"📊 Loaded {discountEntities.Count} discounts from database");

            // Convert entities to DTOs with null-safe access
            _discounts = discountEntities.Select(d => new DiscountDto
            {
                Id = d.Id.Value,
                Code = d.Code ?? "",
                Description = d.Description ?? "",
                Type = d.Type ?? "Percentage",
                Value = d.Value,
                ValidFrom = d.ValidFrom,
                ValidUntil = d.ValidUntil,
                UsedCount = d.UsedCount,
                MaxUses = d.MaxUses,
                IsActive = d.IsActive
            }).ToList();

            Console.WriteLine($"✅ Loaded {_discounts.Count} discounts");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading discounts: {ex.Message}");
            _discounts = new();
        }
    }

    protected CampsitePricingDto? GetSelectedCampsite()
    {
        return _campsites.FirstOrDefault(c => c.Id == _selectedCampsiteId);
    }

    protected string GetSelectedCampsiteName()
    {
        return GetSelectedCampsite()?.Name ?? "Unknown";
    }

    protected IEnumerable<BasePricingDto> GetSelectedCampsitePricing()
    {
        var campsite = GetSelectedCampsite();
        if (campsite?.Pricing != null)
            return campsite.Pricing;
        return new List<BasePricingDto>();
    }

    protected void SelectCampsite(int campsiteId)
    {
        _selectedCampsiteId = campsiteId;
        CalculatePreview();
    }

    protected void CalculatePreview()
    {
        var pricing = GetSelectedCampsitePricing().FirstOrDefault(p => p.Type == _previewAccommodationType);
        _previewBase = pricing?.Price ?? 0;
        _previewTotal = _previewBase * _previewMultiplier * _previewNights;
    }

    protected void AddAccommodationType()
    {
        // TODO: Open dialog to add new accommodation type
        Snackbar.Add("Add accommodation type dialog coming soon", Severity.Info);
    }

    protected void EditAccommodationType(BasePricingDto accommodation)
    {
        // TODO: Open dialog to edit accommodation type pricing
        Snackbar.Add($"Edit accommodation type dialog coming soon", Severity.Info);
    }

    protected async Task DeleteAccommodationType(BasePricingDto accommodation)
    {
        var campsite = GetSelectedCampsite();
        if (campsite == null) return;

        try
        {
            using var context = await DbContextFactory.CreateDbContextAsync();

            var accommodationTypeId = AccommodationTypeId.Create(accommodation.Id);
            var dbAccommodationType = await context.AccommodationTypes
                .FirstOrDefaultAsync(a => a.Id == accommodationTypeId);

            if (dbAccommodationType == null)
            {
                Snackbar.Add("Accommodation type not found", Severity.Error);
                return;
            }

            // Remove from database
            context.AccommodationTypes.Remove(dbAccommodationType);
            await context.SaveChangesAsync();

            // Remove from in-memory list
            campsite.Pricing.Remove(accommodation);

            Console.WriteLine($"✅ Accommodation type '{accommodation.Type}' deleted successfully");
            Snackbar.Add($"Accommodation type '{accommodation.Type}' deleted successfully", Severity.Success);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting accommodation type: {ex.Message}");
            Snackbar.Add($"Error deleting accommodation type: {ex.Message}", Severity.Error);
        }
    }

    protected void AddSeasonalMultiplier()
    {
        // TODO: Open dialog to add new seasonal multiplier
        Snackbar.Add("Add seasonal multiplier dialog coming soon", Severity.Info);
    }

    protected void EditSeasonalMultiplier(SeasonalMultiplierDto multiplier)
    {
        // TODO: Open dialog to edit seasonal multiplier
        Snackbar.Add($"Edit seasonal multiplier dialog coming soon", Severity.Info);
    }

    protected async Task DeleteSeasonalMultiplier(SeasonalMultiplierDto multiplier)
    {
        try
        {
            using var context = await DbContextFactory.CreateDbContextAsync();

            var seasonalPricingId = SeasonalPricingId.Create(multiplier.Id);
            var dbSeasonalPricing = await context.SeasonalPricings
                .FirstOrDefaultAsync(s => s.Id == seasonalPricingId);

            if (dbSeasonalPricing == null)
            {
                Snackbar.Add("Seasonal pricing not found", Severity.Error);
                return;
            }

            // Remove from database
            context.SeasonalPricings.Remove(dbSeasonalPricing);
            await context.SaveChangesAsync();

            // Remove from in-memory list
            _seasonalMultipliers.Remove(multiplier);

            Console.WriteLine($"✅ Seasonal multiplier '{multiplier.Name}' deleted successfully");
            Snackbar.Add($"Seasonal multiplier '{multiplier.Name}' deleted successfully", Severity.Success);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting seasonal multiplier: {ex.Message}");
            Snackbar.Add($"Error deleting seasonal multiplier: {ex.Message}", Severity.Error);
        }
    }

    protected void AddDiscount()
    {
        // TODO: Open dialog to add new discount
        Snackbar.Add("Add discount dialog coming soon", Severity.Info);
    }

    protected void EditDiscount(DiscountDto discount)
    {
        // TODO: Open dialog to edit discount
        Snackbar.Add($"Edit discount dialog coming soon", Severity.Info);
    }

    protected async Task DeleteDiscount(DiscountDto discount)
    {
        try
        {
            using var context = await DbContextFactory.CreateDbContextAsync();

            var discountId = DiscountId.Create(discount.Id);
            var dbDiscount = await context.Discounts
                .FirstOrDefaultAsync(d => d.Id == discountId);

            if (dbDiscount == null)
            {
                Snackbar.Add("Discount not found", Severity.Error);
                return;
            }

            // Remove from database
            context.Discounts.Remove(dbDiscount);
            await context.SaveChangesAsync();

            // Remove from in-memory list
            _discounts.Remove(discount);

            Console.WriteLine($"✅ Discount '{discount.Code}' deleted successfully");
            Snackbar.Add($"Discount '{discount.Code}' deleted successfully", Severity.Success);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting discount: {ex.Message}");
            Snackbar.Add($"Error deleting discount: {ex.Message}", Severity.Error);
        }
    }

    protected void AddAccommodationPeripheralPurchase(BasePricingDto accommodation)
    {
        Snackbar.Add($"Add peripheral purchase to {accommodation.Type}", Severity.Info);
    }

    protected void EditAccommodationPeripheralPurchase(BasePricingDto accommodation, PeripheralPurchaseDto purchase)
    {
        Snackbar.Add($"Edit {purchase.Name}", Severity.Info);
    }

    protected void DeleteAccommodationPeripheralPurchase(BasePricingDto accommodation, PeripheralPurchaseDto purchase)
    {
        accommodation.AccommodationPeripheralPurchases.Remove(purchase);
        Snackbar.Add($"Peripheral purchase '{purchase.Name}' deleted successfully", Severity.Success);
        StateHasChanged();
    }

    protected IEnumerable<PeripheralPurchaseDto> GetSelectedCampsitePeripheralPurchases()
    {
        var campsite = GetSelectedCampsite();
        if (campsite?.PeripheralPurchases != null)
            return campsite.PeripheralPurchases;
        return new List<PeripheralPurchaseDto>();
    }

    protected void AddPeripheralPurchase()
    {
        Snackbar.Add("Add new peripheral purchase", Severity.Info);
    }

    protected void EditPeripheralPurchase(PeripheralPurchaseDto purchase)
    {
        Snackbar.Add($"Edit {purchase.Name}", Severity.Info);
    }

    protected void DeletePeripheralPurchase(PeripheralPurchaseDto purchase)
    {
        var campsite = GetSelectedCampsite();
        if (campsite != null)
        {
            campsite.PeripheralPurchases.Remove(purchase);
            Snackbar.Add($"Peripheral purchase '{purchase.Name}' deleted successfully", Severity.Success);
            StateHasChanged();
        }
    }

    protected string GetAccommodationTypeIcon(string type)
    {
        return type.ToLower() switch
        {
            var t when t.Contains("cabin") => Icons.Material.Filled.Cabin,
            var t when t.Contains("tent") => Icons.Material.Filled.Flatware,
            var t when t.Contains("glamping") => Icons.Material.Filled.Cottage,
            var t when t.Contains("rv") || t.Contains("camper") => Icons.Material.Filled.RvHookup,
            _ => Icons.Material.Filled.House
        };
    }

    protected string GetPeripheralPurchaseIcon(string name)
    {
        return name.ToLower() switch
        {
            var n when n.Contains("fishing") => Icons.Material.Filled.Phishing,
            var n when n.Contains("yoga") => Icons.Material.Filled.SelfImprovement,
            var n when n.Contains("brunch") || n.Contains("restaurant") => Icons.Material.Filled.Restaurant,
            var n when n.Contains("kayak") => Icons.Material.Filled.Kayaking,
            var n when n.Contains("volleyball") => Icons.Material.Filled.SportsVolleyball,
            var n when n.Contains("lighthouse") || n.Contains("tour") => Icons.Material.Filled.Tour,
            var n when n.Contains("bike") || n.Contains("rental") => Icons.Material.Filled.DirectionsBike,
            var n when n.Contains("hiking") => Icons.Material.Filled.Hiking,
            var n when n.Contains("campfire") || n.Contains("firewood") => Icons.Material.Filled.LocalFireDepartment,
            _ => Icons.Material.Filled.LocalActivity
        };
    }

    protected string GetAccommodationIcon(string type) => type switch
    {
        "Cabin" => Icons.Material.Filled.Cabin,
        "Tent Site" => Icons.Material.Filled.Landscape,
        "Glamping" => Icons.Material.Filled.NightShelter,
        "RV Spot" => Icons.Material.Filled.RvHookup,
        _ => Icons.Material.Filled.Home
    };

    protected Color GetSeasonColor(string season) => season switch
    {
        "High Season" => Color.Error,
        "Medium Season" => Color.Warning,
        "Low Season" => Color.Info,
        _ => Color.Default
    };

    protected Color GetAttractivenessColor(string? attractiveness = null)
    {
        var attr = attractiveness ?? GetSelectedCampsite()?.Attractiveness ?? "";
        return attr switch
        {
            "Very High" => Color.Error,
            "High" => Color.Warning,
            "Medium" => Color.Info,
            _ => Color.Default
        };
    }

    protected int GetTotalUnits()
    {
        return GetSelectedCampsitePricing().Sum(p => p.AvailableUnits);
    }

    protected int GetTotalCapacity()
    {
        return GetSelectedCampsitePricing().Sum(p => p.AvailableUnits * p.MaxGuests);
    }

    protected int GetAccommodationTypeCount()
    {
        return GetSelectedCampsitePricing().Count();
    }

    protected decimal GetAveragePrice()
    {
        var pricing = GetSelectedCampsitePricing();
        return pricing.Any() ? pricing.Average(p => p.Price) : 0;
    }

    protected void EditSeasonalPricing()
    {
        Snackbar.Add("Edit seasonal multipliers (applies to all campsites)", Severity.Info);
        // TODO: Open dialog to edit seasonal pricing
    }
}

