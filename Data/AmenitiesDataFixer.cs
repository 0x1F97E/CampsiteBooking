using Microsoft.EntityFrameworkCore;

namespace CampsiteBooking.Data;

/// <summary>
/// Utility class to fix amenities data that was corrupted due to EF Core change tracking issue
/// </summary>
public static class AmenitiesDataFixer
{
    /// <summary>
    /// Fixes the amenities data in the database by updating the _amenities field directly
    /// This is needed because old data was saved before the EF Core change tracking fix was applied
    /// </summary>
    public static async Task FixAmenitiesDataAsync(CampsiteBookingDbContext context)
    {
        Console.WriteLine("🔧 Starting amenities data fix...");
        
        try
        {
            // Load all accommodation types
            var allAccommodationTypes = await context.AccommodationTypes.ToListAsync();
            
            Console.WriteLine($"📊 Found {allAccommodationTypes.Count} accommodation types to check");
            
            foreach (var accType in allAccommodationTypes)
            {
                var currentAmenities = accType.GetAmenitiesList();
                
                Console.WriteLine($"\n🏠 {accType.Type} (ID: {accType.Id?.Value}, CampsiteId: {accType.CampsiteId?.Value})");
                Console.WriteLine($"   Current amenities: [{string.Join(", ", currentAmenities)}]");
                
                // Re-save the amenities using the domain method
                // This will trigger the EF Core change tracking fix
                accType.UpdateAmenities(currentAmenities);
                
                // CRITICAL: Mark the _amenities field as modified
                context.Entry(accType).Property("_amenities").IsModified = true;
            }
            
            // Save all changes
            var changedCount = await context.SaveChangesAsync();
            
            Console.WriteLine($"\n✅ Amenities data fix completed! Updated {changedCount} records.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error fixing amenities data: {ex.Message}");
            Console.WriteLine($"   Stack trace: {ex.StackTrace}");
            throw;
        }
    }
    
    /// <summary>
    /// Clears all amenities for all accommodation types
    /// Useful if you want to start fresh and set amenities via the admin UI
    /// </summary>
    public static async Task ClearAllAmenitiesAsync(CampsiteBookingDbContext context)
    {
        Console.WriteLine("🧹 Clearing all amenities...");
        
        try
        {
            var allAccommodationTypes = await context.AccommodationTypes.ToListAsync();
            
            foreach (var accType in allAccommodationTypes)
            {
                Console.WriteLine($"   Clearing amenities for {accType.Type} (ID: {accType.Id?.Value})");
                
                accType.UpdateAmenities(new List<string>());
                context.Entry(accType).Property("_amenities").IsModified = true;
            }
            
            await context.SaveChangesAsync();
            
            Console.WriteLine($"✅ Cleared amenities for {allAccommodationTypes.Count} accommodation types");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error clearing amenities: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Resets amenities to the seeder defaults
    /// </summary>
    public static async Task ResetToDefaultAmenitiesAsync(CampsiteBookingDbContext context)
    {
        Console.WriteLine("🔄 Resetting amenities to defaults...");
        
        try
        {
            var allAccommodationTypes = await context.AccommodationTypes.ToListAsync();
            
            foreach (var accType in allAccommodationTypes)
            {
                var defaultAmenities = GetDefaultAmenities(accType.Type ?? "", accType.CampsiteId?.Value ?? 0);
                
                Console.WriteLine($"   Resetting {accType.Type} (ID: {accType.Id?.Value}) to: [{string.Join(", ", defaultAmenities)}]");
                
                accType.UpdateAmenities(defaultAmenities);
                context.Entry(accType).Property("_amenities").IsModified = true;
            }
            
            await context.SaveChangesAsync();
            
            Console.WriteLine($"✅ Reset amenities for {allAccommodationTypes.Count} accommodation types");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error resetting amenities: {ex.Message}");
            throw;
        }
    }
    
    private static List<string> GetDefaultAmenities(string type, int campsiteId)
    {
        return type switch
        {
            "Cabin" => new List<string> { "WiFi", "Electric-Hookup", "Water-Hookup", "Heating", "Kitchen", "Bathroom" },
            "Tent Site" => new List<string> { "Electric-Hookup", "Fire-Pit", "Picnic-Table" },
            "Glamping" => new List<string> { "WiFi", "Electric-Hookup", "Heating", "Bathroom", "Kitchen", "Air-Conditioning", "Linens-Provided" },
            "RV Spot" => new List<string> { "Electric-Hookup", "Water-Hookup", "Sewer-Hookup", "WiFi" },
            _ => new List<string>()
        };
    }
}

