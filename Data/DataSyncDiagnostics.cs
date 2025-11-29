using Microsoft.EntityFrameworkCore;

namespace CampsiteBooking.Data;

/// <summary>
/// Diagnostic utility to help identify data synchronization issues between user-facing and admin pages
/// </summary>
public static class DataSyncDiagnostics
{
    /// <summary>
    /// Runs comprehensive diagnostics on bookings and users data
    /// </summary>
    public static async Task RunDiagnosticsAsync(CampsiteBookingDbContext context)
    {
        Console.WriteLine("\n" + new string('=', 80));
        Console.WriteLine("🔍 DATA SYNCHRONIZATION DIAGNOSTICS");
        Console.WriteLine(new string('=', 80));
        
        await DiagnoseBookingsAsync(context);
        await DiagnoseUsersAsync(context);
        await DiagnoseGuestsAsync(context);
        await DiagnoseCampsitesAsync(context);
        await DiagnoseAccommodationTypesAsync(context);
        
        Console.WriteLine(new string('=', 80));
        Console.WriteLine("✅ DIAGNOSTICS COMPLETE");
        Console.WriteLine(new string('=', 80) + "\n");
    }
    
    private static async Task DiagnoseBookingsAsync(CampsiteBookingDbContext context)
    {
        Console.WriteLine("\n📊 BOOKINGS TABLE:");
        Console.WriteLine(new string('-', 80));
        
        try
        {
            var bookings = await context.Bookings.ToListAsync();
            Console.WriteLine($"   Total bookings in database: {bookings.Count}");
            
            if (bookings.Any())
            {
                Console.WriteLine($"   Latest booking ID: {bookings.Max(b => b.Id?.Value ?? 0)}");
                Console.WriteLine($"   Oldest booking ID: {bookings.Min(b => b.Id?.Value ?? 0)}");
                
                var statusGroups = bookings.GroupBy(b => b.Status.ToString()).ToList();
                Console.WriteLine($"   Bookings by status:");
                foreach (var group in statusGroups)
                {
                    Console.WriteLine($"      - {group.Key}: {group.Count()}");
                }
                
                Console.WriteLine($"\n   Recent bookings (last 5):");
                var recentBookings = bookings.OrderByDescending(b => b.Id?.Value ?? 0).Take(5);
                foreach (var booking in recentBookings)
                {
                    Console.WriteLine($"      - ID: {booking.Id?.Value}, Guest: {booking.GuestId?.Value}, " +
                                    $"Campsite: {booking.CampsiteId?.Value}, Status: {booking.Status}, " +
                                    $"Created: {booking.CreatedDate:yyyy-MM-dd HH:mm:ss}");
                }
            }
            else
            {
                Console.WriteLine($"   ⚠️  NO BOOKINGS FOUND IN DATABASE!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Error loading bookings: {ex.Message}");
        }
    }
    
    private static async Task DiagnoseUsersAsync(CampsiteBookingDbContext context)
    {
        Console.WriteLine("\n👥 USERS TABLE:");
        Console.WriteLine(new string('-', 80));
        
        try
        {
            var users = await context.Users.ToListAsync();
            Console.WriteLine($"   Total users in database: {users.Count}");
            
            if (users.Any())
            {
                Console.WriteLine($"   Latest user ID: {users.Max(u => u.Id?.Value ?? 0)}");
                Console.WriteLine($"   Oldest user ID: {users.Min(u => u.Id?.Value ?? 0)}");
                
                var roleGroups = users.GroupBy(u => u.GetType().Name).ToList();
                Console.WriteLine($"   Users by role:");
                foreach (var group in roleGroups)
                {
                    Console.WriteLine($"      - {group.Key}: {group.Count()}");
                }
                
                Console.WriteLine($"\n   Recent users (last 5):");
                var recentUsers = users.OrderByDescending(u => u.Id?.Value ?? 0).Take(5);
                foreach (var user in recentUsers)
                {
                    Console.WriteLine($"      - ID: {user.Id?.Value}, Name: {user.FirstName} {user.LastName}, " +
                                    $"Email: {user.Email?.Value}, Role: {user.GetType().Name}, " +
                                    $"Joined: {user.JoinedDate:yyyy-MM-dd HH:mm:ss}");
                }
            }
            else
            {
                Console.WriteLine($"   ⚠️  NO USERS FOUND IN DATABASE!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Error loading users: {ex.Message}");
        }
    }
    
    private static async Task DiagnoseGuestsAsync(CampsiteBookingDbContext context)
    {
        Console.WriteLine("\n🏕️ GUESTS TABLE:");
        Console.WriteLine(new string('-', 80));
        
        try
        {
            var guests = await context.Guests.ToListAsync();
            Console.WriteLine($"   Total guests in database: {guests.Count}");
            
            if (guests.Any())
            {
                Console.WriteLine($"   Latest guest ID: {guests.Max(g => g.Id?.Value ?? 0)}");
                Console.WriteLine($"   Recent guests (last 3):");
                var recentGuests = guests.OrderByDescending(g => g.Id?.Value ?? 0).Take(3);
                foreach (var guest in recentGuests)
                {
                    Console.WriteLine($"      - ID: {guest.Id?.Value}, Name: {guest.FirstName} {guest.LastName}, " +
                                    $"Email: {guest.Email?.Value}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Error loading guests: {ex.Message}");
        }
    }
    
    private static async Task DiagnoseCampsitesAsync(CampsiteBookingDbContext context)
    {
        Console.WriteLine("\n🏕️ CAMPSITES TABLE:");
        Console.WriteLine(new string('-', 80));
        
        try
        {
            var campsites = await context.Campsites.ToListAsync();
            Console.WriteLine($"   Total campsites in database: {campsites.Count}");
            
            if (campsites.Any())
            {
                foreach (var campsite in campsites.Take(5))
                {
                    Console.WriteLine($"      - ID: {campsite.Id?.Value}, Name: {campsite.Name}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Error loading campsites: {ex.Message}");
        }
    }
    
    private static async Task DiagnoseAccommodationTypesAsync(CampsiteBookingDbContext context)
    {
        Console.WriteLine("\n🏠 ACCOMMODATION TYPES TABLE:");
        Console.WriteLine(new string('-', 80));
        
        try
        {
            var accommodationTypes = await context.AccommodationTypes.ToListAsync();
            Console.WriteLine($"   Total accommodation types in database: {accommodationTypes.Count}");
            
            if (accommodationTypes.Any())
            {
                foreach (var accType in accommodationTypes.Take(5))
                {
                    Console.WriteLine($"      - ID: {accType.Id?.Value}, Type: {accType.Type}, " +
                                    $"Campsite: {accType.CampsiteId?.Value}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Error loading accommodation types: {ex.Message}");
        }
    }
}

