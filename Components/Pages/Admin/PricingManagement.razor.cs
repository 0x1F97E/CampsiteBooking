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

            // Load all purchase options
            var allPurchaseOptions = await context.PurchaseOptions.ToListAsync();

            Console.WriteLine($"📊 Loaded {allPurchaseOptions.Count} purchase options from database");

            // Convert to DTOs
            _campsites = campsiteEntities.Select(c =>
            {
                // Filter accommodation types for this campsite
                var campsiteAccommodationTypes = allAccommodationTypes
                    .Where(a => a.CampsiteId?.Value == c.Id?.Value)
                    .ToList();

                // Filter campsite-level purchase options (no accommodation type)
                var campsitePurchaseOptions = allPurchaseOptions
                    .Where(p => p.CampsiteId?.Value == c.Id?.Value && p.AccommodationTypeId == null)
                    .Select(p => new PeripheralPurchaseDto
                    {
                        Id = p.PurchaseOptionId,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price?.Amount ?? 0,
                        Category = p.Category,
                        IsActive = p.IsActive
                    })
                    .ToList();

                return new CampsitePricingDto
                {
                    Id = c.Id?.Value ?? 0,
                    Name = c.Name ?? "Unknown",
                    City = c.City ?? "Unknown",
                    Attractiveness = c.Attractiveness ?? "Unknown",
                    Pricing = campsiteAccommodationTypes.Select(a =>
                    {
                        // Filter accommodation-level purchase options
                        var accommodationPurchaseOptions = allPurchaseOptions
                            .Where(p => p.AccommodationTypeId?.Value == a.Id?.Value)
                            .Select(p => new PeripheralPurchaseDto
                            {
                                Id = p.PurchaseOptionId,
                                Name = p.Name,
                                Description = p.Description,
                                Price = p.Price?.Amount ?? 0,
                                Category = p.Category,
                                IsActive = p.IsActive
                            })
                            .ToList();

                        return new BasePricingDto
                        {
                            Id = a.Id?.Value ?? 0,
                            Type = a.Type ?? "Unknown",
                            Price = a.BasePrice?.Amount ?? 0,
                            AvailableUnits = a.AvailableUnits,
                            MaxGuests = a.MaxCapacity,
                            IsActive = a.IsActive,
                            AccommodationPeripheralPurchases = accommodationPurchaseOptions
                        };
                    }).ToList(),
                    PeripheralPurchases = campsitePurchaseOptions
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

            // Debug: Log each entity's properties
            foreach (var sp in seasonalPricingEntities.Take(3))
            {
                Console.WriteLine($"   📋 Entity: Id={sp.Id?.Value}, SeasonName='{sp.SeasonName}', IsActive={sp.IsActive}, Multiplier={sp.PriceMultiplier}, StartDate={sp.StartDate}, EndDate={sp.EndDate}");
            }

            // Filter active ones
            var activeEntities = seasonalPricingEntities.Where(sp => sp.IsActive).ToList();
            Console.WriteLine($"   🔍 Active entities: {activeEntities.Count}");

            // Group by season name and create DTOs
            _seasonalMultipliers = activeEntities
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
            Console.WriteLine($"   Stack trace: {ex.StackTrace}");
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

    // Save accommodation type pricing (base price and max capacity)
    protected async Task SaveAccommodationPricing(BasePricingDto accommodation)
    {
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

            // Update pricing using domain methods
            dbAccommodationType.UpdateBasePrice(Money.Create(accommodation.Price, "DKK"));
            dbAccommodationType.UpdateMaxCapacity(accommodation.MaxGuests);

            // Mark private fields as modified for EF Core to track changes
            context.Entry(dbAccommodationType).Property("_basePrice").IsModified = true;
            context.Entry(dbAccommodationType).Property("_maxCapacity").IsModified = true;

            await context.SaveChangesAsync();

            Console.WriteLine($"✅ Saved pricing for '{accommodation.Type}': ${accommodation.Price}/night, {accommodation.MaxGuests} guests max");
            Snackbar.Add($"Pricing for '{accommodation.Type}' saved successfully!", Severity.Success);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error saving accommodation pricing: {ex.Message}");
            Snackbar.Add($"Error saving pricing: {ex.Message}", Severity.Error);
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

    protected async Task AddAccommodationPeripheralPurchase(BasePricingDto accommodation)
    {
        var campsite = GetSelectedCampsite();
        if (campsite == null) return;

        var parameters = new DialogParameters<AddPurchaseDialog>
        {
            { x => x.Purchase, new AddPurchaseDialog.PurchaseDialogDto { AccommodationTypeId = accommodation.Id } },
            { x => x.IsEdit, false },
            { x => x.AccommodationTypeName, accommodation.Type }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<AddPurchaseDialog>($"Add Purchase Option for {accommodation.Type}", parameters, options);
        var result = await dialog.Result;

        if (result != null && !result.Canceled && result.Data is AddPurchaseDialog.PurchaseDialogDto purchaseDto)
        {
            await SavePurchaseOption(campsite.Id, purchaseDto);

            // Add to in-memory list with the ID from the saved entity
            accommodation.AccommodationPeripheralPurchases.Add(new PeripheralPurchaseDto
            {
                Id = purchaseDto.Id,
                Name = purchaseDto.Name,
                Description = purchaseDto.Description,
                Price = purchaseDto.Price,
                Category = purchaseDto.Category,
                IsActive = purchaseDto.IsActive
            });

            Snackbar.Add($"Purchase option '{purchaseDto.Name}' added successfully", Severity.Success);
            StateHasChanged();
        }
    }

    protected async Task EditAccommodationPeripheralPurchase(BasePricingDto accommodation, PeripheralPurchaseDto purchase)
    {
        var parameters = new DialogParameters<AddPurchaseDialog>
        {
            { x => x.Purchase, new AddPurchaseDialog.PurchaseDialogDto
                {
                    Id = purchase.Id,
                    Name = purchase.Name,
                    Description = purchase.Description,
                    Price = purchase.Price,
                    Category = purchase.Category,
                    IsActive = purchase.IsActive,
                    AccommodationTypeId = accommodation.Id
                }
            },
            { x => x.IsEdit, true },
            { x => x.AccommodationTypeName, accommodation.Type }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<AddPurchaseDialog>($"Edit Purchase Option", parameters, options);
        var result = await dialog.Result;

        if (result != null && !result.Canceled && result.Data is AddPurchaseDialog.PurchaseDialogDto purchaseDto)
        {
            await UpdatePurchaseOption(purchaseDto);

            // Update in-memory
            purchase.Name = purchaseDto.Name;
            purchase.Description = purchaseDto.Description;
            purchase.Price = purchaseDto.Price;
            purchase.Category = purchaseDto.Category;
            purchase.IsActive = purchaseDto.IsActive;

            Snackbar.Add($"Purchase option '{purchaseDto.Name}' updated successfully", Severity.Success);
            StateHasChanged();
        }
    }

    protected async Task DeleteAccommodationPeripheralPurchase(BasePricingDto accommodation, PeripheralPurchaseDto purchase)
    {
        var confirm = await DialogService.ShowMessageBox(
            "Confirm Delete",
            $"Are you sure you want to delete the purchase option '{purchase.Name}'?",
            yesText: "Delete",
            cancelText: "Cancel");

        if (confirm == true)
        {
            await DeletePurchaseOptionFromDb(purchase.Id);
            accommodation.AccommodationPeripheralPurchases.Remove(purchase);
            Snackbar.Add($"Purchase option '{purchase.Name}' deleted successfully", Severity.Success);
            StateHasChanged();
        }
    }

    protected IEnumerable<PeripheralPurchaseDto> GetSelectedCampsitePeripheralPurchases()
    {
        var campsite = GetSelectedCampsite();
        if (campsite?.PeripheralPurchases != null)
            return campsite.PeripheralPurchases;
        return new List<PeripheralPurchaseDto>();
    }

    protected async Task AddPeripheralPurchase()
    {
        var campsite = GetSelectedCampsite();
        if (campsite == null) return;

        var parameters = new DialogParameters<AddPurchaseDialog>
        {
            { x => x.Purchase, new AddPurchaseDialog.PurchaseDialogDto() },
            { x => x.IsEdit, false }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<AddPurchaseDialog>($"Add Purchase Option for {campsite.Name}", parameters, options);
        var result = await dialog.Result;

        if (result != null && !result.Canceled && result.Data is AddPurchaseDialog.PurchaseDialogDto purchaseDto)
        {
            await SavePurchaseOption(campsite.Id, purchaseDto);

            // Add to in-memory list with the ID from the saved entity
            campsite.PeripheralPurchases.Add(new PeripheralPurchaseDto
            {
                Id = purchaseDto.Id,
                Name = purchaseDto.Name,
                Description = purchaseDto.Description,
                Price = purchaseDto.Price,
                Category = purchaseDto.Category,
                IsActive = purchaseDto.IsActive
            });

            Snackbar.Add($"Purchase option '{purchaseDto.Name}' added successfully", Severity.Success);
            StateHasChanged();
        }
    }

    protected async Task EditPeripheralPurchase(PeripheralPurchaseDto purchase)
    {
        var parameters = new DialogParameters<AddPurchaseDialog>
        {
            { x => x.Purchase, new AddPurchaseDialog.PurchaseDialogDto
                {
                    Id = purchase.Id,
                    Name = purchase.Name,
                    Description = purchase.Description,
                    Price = purchase.Price,
                    Category = purchase.Category,
                    IsActive = purchase.IsActive
                }
            },
            { x => x.IsEdit, true }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        var dialog = await DialogService.ShowAsync<AddPurchaseDialog>("Edit Purchase Option", parameters, options);
        var result = await dialog.Result;

        if (result != null && !result.Canceled && result.Data is AddPurchaseDialog.PurchaseDialogDto purchaseDto)
        {
            await UpdatePurchaseOption(purchaseDto);

            // Update in-memory
            purchase.Name = purchaseDto.Name;
            purchase.Description = purchaseDto.Description;
            purchase.Price = purchaseDto.Price;
            purchase.Category = purchaseDto.Category;
            purchase.IsActive = purchaseDto.IsActive;

            Snackbar.Add($"Purchase option '{purchaseDto.Name}' updated successfully", Severity.Success);
            StateHasChanged();
        }
    }

    protected async Task DeletePeripheralPurchase(PeripheralPurchaseDto purchase)
    {
        var campsite = GetSelectedCampsite();
        if (campsite == null) return;

        var confirm = await DialogService.ShowMessageBox(
            "Confirm Delete",
            $"Are you sure you want to delete the purchase option '{purchase.Name}'?",
            yesText: "Delete",
            cancelText: "Cancel");

        if (confirm == true)
        {
            await DeletePurchaseOptionFromDb(purchase.Id);
            campsite.PeripheralPurchases.Remove(purchase);
            Snackbar.Add($"Purchase option '{purchase.Name}' deleted successfully", Severity.Success);
            StateHasChanged();
        }
    }

    private async Task SavePurchaseOption(int campsiteId, AddPurchaseDialog.PurchaseDialogDto dto)
    {
        try
        {
            using var context = await DbContextFactory.CreateDbContextAsync();

            PurchaseOption purchaseOption;

            if (dto.AccommodationTypeId.HasValue)
            {
                purchaseOption = PurchaseOption.CreateForAccommodationType(
                    CampsiteId.Create(campsiteId),
                    AccommodationTypeId.Create(dto.AccommodationTypeId.Value),
                    dto.Name,
                    dto.Description,
                    Money.Create(dto.Price, "USD"),
                    dto.Category
                );
            }
            else
            {
                purchaseOption = PurchaseOption.CreateForCampsite(
                    CampsiteId.Create(campsiteId),
                    dto.Name,
                    dto.Description,
                    Money.Create(dto.Price, "USD"),
                    dto.Category
                );
            }

            if (!dto.IsActive)
            {
                purchaseOption.Deactivate();
            }

            await context.PurchaseOptions.AddAsync(purchaseOption);
            await context.SaveChangesAsync();

            dto.Id = purchaseOption.PurchaseOptionId;
            Console.WriteLine($"✅ Saved purchase option '{dto.Name}' with ID {dto.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error saving purchase option: {ex.Message}");
            Snackbar.Add($"Error saving purchase option: {ex.Message}", Severity.Error);
            throw;
        }
    }

    private async Task UpdatePurchaseOption(AddPurchaseDialog.PurchaseDialogDto dto)
    {
        try
        {
            using var context = await DbContextFactory.CreateDbContextAsync();

            var purchaseOptionId = PurchaseOptionId.Create(dto.Id);
            var dbPurchaseOption = await context.PurchaseOptions
                .FirstOrDefaultAsync(p => p.Id == purchaseOptionId);

            if (dbPurchaseOption == null)
            {
                Snackbar.Add("Purchase option not found", Severity.Error);
                return;
            }

            dbPurchaseOption.UpdateDetails(
                dto.Name,
                dto.Description,
                Money.Create(dto.Price, "USD"),
                dto.Category
            );

            dbPurchaseOption.SetActive(dto.IsActive);

            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Updated purchase option '{dto.Name}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error updating purchase option: {ex.Message}");
            Snackbar.Add($"Error updating purchase option: {ex.Message}", Severity.Error);
            throw;
        }
    }

    private async Task DeletePurchaseOptionFromDb(int purchaseOptionId)
    {
        try
        {
            using var context = await DbContextFactory.CreateDbContextAsync();

            var id = PurchaseOptionId.Create(purchaseOptionId);
            var dbPurchaseOption = await context.PurchaseOptions
                .FirstOrDefaultAsync(p => p.Id == id);

            if (dbPurchaseOption != null)
            {
                context.PurchaseOptions.Remove(dbPurchaseOption);
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Deleted purchase option with ID {purchaseOptionId}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error deleting purchase option: {ex.Message}");
            Snackbar.Add($"Error deleting purchase option: {ex.Message}", Severity.Error);
            throw;
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

    protected string GetCategoryIcon(string? category)
    {
        return category?.ToLowerInvariant() switch
        {
            "activity" => Icons.Material.Filled.DirectionsRun,
            "service" => Icons.Material.Filled.RoomService,
            "equipment" => Icons.Material.Filled.Build,
            "food & beverage" or "food" => Icons.Material.Filled.Restaurant,
            "other" => Icons.Material.Filled.MoreHoriz,
            _ => Icons.Material.Filled.ShoppingCart
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

    protected async Task EditSeasonalPricing()
    {
        try
        {
            using var context = await DbContextFactory.CreateDbContextAsync();
            var seasonalPricingEntities = await context.SeasonalPricings.ToListAsync();

            // Group by season name and create edit DTOs
            var seasonEditDtos = seasonalPricingEntities
                .Where(sp => sp.IsActive)
                .GroupBy(sp => sp.SeasonName)
                .Select(g =>
                {
                    var first = g.First();
                    return new EditSeasonalMultipliersDialog.SeasonEditDto
                    {
                        Id = first.Id.Value,
                        Name = first.SeasonName ?? "Unknown Season",
                        StartDate = first.StartDate,
                        EndDate = first.EndDate,
                        Multiplier = first.PriceMultiplier
                    };
                })
                .ToList();

            var parameters = new DialogParameters<EditSeasonalMultipliersDialog>
            {
                { x => x.Seasons, seasonEditDtos }
            };

            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true };
            var dialog = await DialogService.ShowAsync<EditSeasonalMultipliersDialog>("Edit Seasonal Multipliers", parameters, options);
            var result = await dialog.Result;

            if (result != null && !result.Canceled && result.Data is List<EditSeasonalMultipliersDialog.SeasonEditDto> updatedSeasons)
            {
                await SaveSeasonalMultipliers(updatedSeasons);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error editing seasonal pricing: {ex.Message}");
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
    }

    private async Task SaveSeasonalMultipliers(List<EditSeasonalMultipliersDialog.SeasonEditDto> updatedSeasons)
    {
        try
        {
            using var context = await DbContextFactory.CreateDbContextAsync();
            var allSeasonalPricing = await context.SeasonalPricings.ToListAsync();

            foreach (var updated in updatedSeasons)
            {
                // Find all records with this season name and update them
                var matchingRecords = allSeasonalPricing.Where(sp => sp.SeasonName == updated.Name).ToList();

                foreach (var record in matchingRecords)
                {
                    // Update the price multiplier
                    record.UpdatePriceMultiplier(updated.Multiplier);

                    // Update dates if changed
                    if (updated.StartDate.HasValue && updated.EndDate.HasValue)
                    {
                        record.UpdateDates(updated.StartDate.Value, updated.EndDate.Value);
                    }
                }
            }

            await context.SaveChangesAsync();
            Snackbar.Add("Seasonal multipliers updated successfully!", Severity.Success);

            // Reload the data to refresh the UI
            await LoadSeasonalPricing();
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error saving seasonal multipliers: {ex.Message}");
            Snackbar.Add($"Error saving changes: {ex.Message}", Severity.Error);
        }
    }
}

