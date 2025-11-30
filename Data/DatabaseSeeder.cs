using CampsiteBooking.Models;
using CampsiteBooking.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace CampsiteBooking.Data;

/// <summary>
/// Seeds initial data into the database
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(CampsiteBookingDbContext context)
    {
        // Note: Schema should be managed by EF Core Migrations in production
        // This seeder only populates initial data

        // IMPORTANT: Set up AUTO_INCREMENT for all tables with strongly-typed IDs
        // This is required because EF Core migrations don't set up AUTO_INCREMENT by default
        await SetupAutoIncrementForAllTables(context);

        // Seed Campsites first (before users, since Staff references CampsiteId)
        // Force complete reseed by truncating the table
        await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0");
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Campsites");
        await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1");
        await context.Database.ExecuteSqlRawAsync("ALTER TABLE Campsites AUTO_INCREMENT = 1");

        var campsite1 = Campsite.Create(
                name: "Copenhagen Beach Camp",
                streetAddress: "Strandvejen 152",
                city: "Charlottenlund",
                postalCode: "2920",
                latitude: 55.6761,
                longitude: 12.5683,
                establishedYear: 2010,
                description: "Beautiful beachfront camping with stunning views of the Øresund strait. Our campsite offers direct beach access, modern facilities, and a variety of accommodation options for families and solo travelers alike.",
                attractiveness: "High",
                phoneNumber: "+45 12 34 56 78",
                email: Email.Create("info@copenhagenbeach.dk"),
                websiteUrl: "https://copenhagenbeach.dk"
            );
            await context.Campsites.AddAsync(campsite1);
            await context.SaveChangesAsync();
            context.Entry(campsite1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var campsite2 = Campsite.Create(
                name: "Skagen North Point",
                streetAddress: "Fyrvej 45",
                city: "Skagen",
                postalCode: "9990",
                latitude: 57.7209,
                longitude: 10.5882,
                establishedYear: 2005,
                description: "Denmark's northernmost campsite with breathtaking views where two seas meet. Experience unique natural phenomena and pristine beaches.",
                attractiveness: "Very High",
                phoneNumber: "+45 23 45 67 89",
                email: Email.Create("info@skagennorth.dk"),
                websiteUrl: "https://skagennorth.dk"
            );
            await context.Campsites.AddAsync(campsite2);
            await context.SaveChangesAsync();
            context.Entry(campsite2).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var campsite3 = Campsite.Create(
                name: "Aarhus Forest Retreat",
                streetAddress: "Skovvej 78",
                city: "Aarhus",
                postalCode: "8000",
                latitude: 56.1629,
                longitude: 10.2039,
                establishedYear: 2012,
                description: "Peaceful forest camping surrounded by ancient Danish woodlands. Perfect for nature lovers seeking tranquility and outdoor adventures.",
                attractiveness: "High",
                phoneNumber: "+45 34 56 78 90",
                email: Email.Create("info@aarhusforest.dk"),
                websiteUrl: "https://aarhusforest.dk"
            );
            await context.Campsites.AddAsync(campsite3);
            await context.SaveChangesAsync();
            context.Entry(campsite3).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var campsite4 = Campsite.Create(
                name: "Odense Family Camp",
                streetAddress: "Campingvej 23",
                city: "Odense",
                postalCode: "5000",
                latitude: 55.4038,
                longitude: 10.4024,
                establishedYear: 2008,
                description: "Family-friendly campsite with playgrounds, activities, and spacious facilities. Close to Hans Christian Andersen's birthplace and other attractions.",
                attractiveness: "Medium",
                phoneNumber: "+45 45 67 89 01",
                email: Email.Create("info@odensefamily.dk"),
                websiteUrl: "https://odensefamily.dk"
            );
            await context.Campsites.AddAsync(campsite4);
            await context.SaveChangesAsync();
            context.Entry(campsite4).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var campsite5 = Campsite.Create(
                name: "Bornholm Island Camp",
                streetAddress: "Havnevej 12",
                city: "Rønne",
                postalCode: "3700",
                latitude: 55.1367,
                longitude: 14.9155,
                establishedYear: 2015,
                description: "Island paradise camping on the beautiful Baltic island of Bornholm. Enjoy rocky coastlines, sandy beaches, and charming fishing villages.",
                attractiveness: "Very High",
                phoneNumber: "+45 56 78 90 12",
                email: Email.Create("info@bornholmisland.dk"),
                websiteUrl: "https://bornholmisland.dk"
            );
            await context.Campsites.AddAsync(campsite5);
            await context.SaveChangesAsync();
            context.Entry(campsite5).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Make AccommodationTypes Id column auto-increment if it isn't already
        try
        {
            await context.Database.ExecuteSqlRawAsync("ALTER TABLE AccommodationTypes MODIFY COLUMN Id INT AUTO_INCREMENT");
        }
        catch
        {
            // Might already be auto-increment, ignore error
        }

        // Check if accommodation types already exist - if so, skip seeding to preserve admin changes
        var existingAccommodationTypes = await context.AccommodationTypes.AnyAsync();
        if (existingAccommodationTypes)
        {
            Console.WriteLine("⏭️  Accommodation types already exist, skipping seeding to preserve admin changes...");
        }
        else
        {
            // Get the actual campsite IDs from the database (they should be 1-5 after truncate and reseed)
            var campsiteIds = await context.Database.SqlQueryRaw<int>("SELECT Id as Value FROM Campsites ORDER BY Id").ToListAsync();
            if (campsiteIds.Count < 5)
            {
                Console.WriteLine($"❌ Expected 5 campsites, found {campsiteIds.Count}. Skipping accommodation types seeding.");
            }
            else
            {

        // Seed Accommodation Types for each campsite
        // Copenhagen Beach Camp (ID 1) - High attractiveness
        var cabin1 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[0]),
                "Cabin",
                6,
                Money.Create(150m, "DKK"),
                12,
                "Cozy wooden cabins with modern amenities, perfect for families",
                "https://images.unsplash.com/photo-1587061949409-02df41d5e562?w=800&q=80"
            );
            cabin1.UpdateAmenities(new List<string> { "WiFi", "Electric-Hookup", "Water-Hookup", "Heating", "Kitchen", "Bathroom" });
            await context.AccommodationTypes.AddAsync(cabin1);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(cabin1).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(cabin1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var tent1 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[0]),
                "Tent Site",
                4,
                Money.Create(60m, "DKK"),
                25,
                "Spacious tent sites with electricity hookup",
                "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80"
            );
            tent1.UpdateAmenities(new List<string> { "Electric-Hookup", "Fire-Pit", "Picnic-Table" });
            await context.AccommodationTypes.AddAsync(tent1);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(tent1).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(tent1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var glamping1 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[0]),
                "Glamping",
                4,
                Money.Create(220m, "DKK"),
                8,
                "Luxury glamping tents with premium furnishings",
                "https://images.unsplash.com/photo-1504280390367-361c6d9f38f4?w=800&q=80"
            );
            glamping1.UpdateAmenities(new List<string> { "WiFi", "Electric-Hookup", "Heating", "Bathroom", "Kitchen", "Air-Conditioning", "Linens-Provided" });
            await context.AccommodationTypes.AddAsync(glamping1);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(glamping1).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(glamping1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var rv1 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[0]),
                "RV Spot",
                6,
                Money.Create(100m, "DKK"),
                15,
                "Full hookup RV spots with water, electricity, and sewage",
                "https://images.unsplash.com/photo-1523987355523-c7b5b0dd90a7?w=800&q=80"
            );
            rv1.UpdateAmenities(new List<string> { "Electric-Hookup", "Water-Hookup", "Sewer-Hookup", "WiFi" });
            await context.AccommodationTypes.AddAsync(rv1);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(rv1).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(rv1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Skagen North Point (ID 2) - Very High attractiveness
            var cabin2 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[1]),
                "Cabin",
                6,
                Money.Create(145m, "DKK"),
                10,
                "Modern cabins with sea views",
                "https://images.unsplash.com/photo-1587061949409-02df41d5e562?w=800&q=80"
            );
            cabin2.UpdateAmenities(new List<string> { "WiFi", "Electric-Hookup", "Water-Hookup", "Heating", "Kitchen", "Bathroom" });
            await context.AccommodationTypes.AddAsync(cabin2);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(cabin2).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(cabin2).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var tent2 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[1]),
                "Tent Site",
                4,
                Money.Create(55m, "DKK"),
                30,
                "Tent sites near the beach",
                "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80"
            );
            tent2.UpdateAmenities(new List<string> { "Electric-Hookup", "Fire-Pit", "Picnic-Table" });
            await context.AccommodationTypes.AddAsync(tent2);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(tent2).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(tent2).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Aarhus Forest Retreat (ID 3) - High attractiveness
            var cabin3 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[2]),
                "Cabin",
                5,
                Money.Create(120m, "DKK"),
                15,
                "Forest cabins surrounded by nature",
                "https://images.unsplash.com/photo-1587061949409-02df41d5e562?w=800&q=80"
            );
            cabin3.UpdateAmenities(new List<string> { "WiFi", "Electric-Hookup", "Heating", "Kitchen", "Bathroom" });
            await context.AccommodationTypes.AddAsync(cabin3);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(cabin3).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(cabin3).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var tent3 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[2]),
                "Tent Site",
                4,
                Money.Create(45m, "DKK"),
                40,
                "Peaceful forest tent sites",
                "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80"
            );
            tent3.UpdateAmenities(new List<string> { "Fire-Pit", "Picnic-Table" });
            await context.AccommodationTypes.AddAsync(tent3);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(tent3).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(tent3).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Odense Family Camp (ID 4) - Medium attractiveness
            var cabin4 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[3]),
                "Cabin",
                5,
                Money.Create(100m, "DKK"),
                18,
                "Family-friendly cabins near playground",
                "https://images.unsplash.com/photo-1587061949409-02df41d5e562?w=800&q=80"
            );
            cabin4.UpdateAmenities(new List<string> { "WiFi", "Electric-Hookup", "Heating", "Kitchen", "Bathroom" });
            await context.AccommodationTypes.AddAsync(cabin4);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(cabin4).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(cabin4).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var tent4 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[3]),
                "Tent Site",
                4,
                Money.Create(40m, "DKK"),
                50,
                "Large tent area for families",
                "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80"
            );
            tent4.UpdateAmenities(new List<string> { "Electric-Hookup", "Fire-Pit", "Picnic-Table" });
            await context.AccommodationTypes.AddAsync(tent4);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(tent4).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(tent4).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Bornholm Island Camp (ID 5) - Very High attractiveness
            var cabin5 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[4]),
                "Cabin",
                6,
                Money.Create(140m, "DKK"),
                8,
                "Island cabins with stunning views",
                "https://images.unsplash.com/photo-1587061949409-02df41d5e562?w=800&q=80"
            );
            cabin5.UpdateAmenities(new List<string> { "WiFi", "Electric-Hookup", "Water-Hookup", "Heating", "Kitchen", "Bathroom" });
            await context.AccommodationTypes.AddAsync(cabin5);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(cabin5).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(cabin5).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var tent5 = AccommodationType.Create(
                CampsiteId.Create(campsiteIds[4]),
                "Tent Site",
                4,
                Money.Create(55m, "DKK"),
                20,
                "Tent sites with island charm",
                "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80"
            );
            tent5.UpdateAmenities(new List<string> { "Electric-Hookup", "Fire-Pit", "Picnic-Table" });
            await context.AccommodationTypes.AddAsync(tent5);
            // CRITICAL FIX: Mark _amenities as modified for EF Core change tracking
            context.Entry(tent5).Property("_amenities").IsModified = true;
            await context.SaveChangesAsync();
            context.Entry(tent5).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                Console.WriteLine("✅ Seeded accommodation types for all 5 campsites");
            } // End of accommodation types seeding
        } // End of if/else for accommodation types existence check

        // Seed Accommodation Spots (needed for bookings)
        await SeedAccommodationSpots(context);

        // Seed Seasonal Pricing
        await SeedSeasonalPricing(context);

        // Seed Discounts
        await SeedDiscounts(context);

        // Seed Events
        await SeedEvents(context);

        // Seed Newsletters
        await SeedNewsletters(context);

        // Seed Sample Bookings (must be last, after users and accommodation spots)
        await SeedBookings(context);

        // Seed Payments (after bookings)
        await SeedPayments(context);

        // Seed Reviews (after users and campsites)
        await SeedReviews(context);

        // Seed Amenities (after campsites)
        await SeedAmenities(context);

        // Seed Event Registrations (after events and users)
        await SeedEventRegistrations(context);

        // Seed Newsletter Subscriptions (after users)
        await SeedNewsletterSubscriptions(context);

        // Seed Peripheral Purchases (after bookings)
        await SeedPeripheralPurchases(context);

        // Seed Newsletter Analytics (after newsletters)
        await SeedNewsletterAnalytics(context);

        // Seed Availabilities (after accommodation types)
        await SeedAvailabilities(context);

        // Seed Maintenance Tasks (after campsites and accommodation spots)
        await SeedMaintenanceTasks(context);

        // Delete ALL users (they have NULL values in new columns and need to be recreated)
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Users");

        // Reset auto-increment counter
        await context.Database.ExecuteSqlRawAsync("ALTER TABLE Users AUTO_INCREMENT = 1");

        // Check if users exist using raw SQL (to avoid EF Core trying to load NULL values)
        var userCount = await context.Database.SqlQueryRaw<int>("SELECT COUNT(*) as Value FROM Users").FirstOrDefaultAsync();

        // Seed Users if none exist
        if (userCount == 0)
        {
            // Get the actual campsite IDs from the database for Staff member creation
            var campsiteIdsForUsers = await context.Database.SqlQueryRaw<int>("SELECT Id as Value FROM Campsites ORDER BY Id").ToListAsync();

            // Create password hasher (ASP.NET Core Identity)
            var passwordHasher = new PasswordHasher<User>();

            // Add users one by one to avoid ID conflicts (all have ID = 0 before persistence)
            var guest1 = Guest.Create(Email.Create("john.doe@example.com"), "John", "Doe", "+45 12 34 56 78", "Denmark");
            guest1.SetPasswordHash(passwordHasher.HashPassword(guest1, "Password123!")); // Demo password
            await context.Users.AddAsync(guest1);
            await context.SaveChangesAsync();
            context.Entry(guest1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var guest2 = Guest.Create(Email.Create("jane.smith@example.com"), "Jane", "Smith", "+45 23 45 67 89", "Denmark");
            guest2.SetPasswordHash(passwordHasher.HashPassword(guest2, "Password123!"));
            await context.Users.AddAsync(guest2);
            await context.SaveChangesAsync();
            context.Entry(guest2).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var guest3 = Guest.Create(Email.Create("mike.johnson@example.com"), "Mike", "Johnson", "+45 34 56 78 90", "Sweden");
            guest3.SetPasswordHash(passwordHasher.HashPassword(guest3, "Password123!"));
            await context.Users.AddAsync(guest3);
            await context.SaveChangesAsync();
            context.Entry(guest3).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var admin = Admin.Create(Email.Create("admin@campsitebooking.dk"), "Admin", "User", "+45 45 67 89 01", "Denmark");
            admin.SetPasswordHash(passwordHasher.HashPassword(admin, "Admin123!"));
            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
            context.Entry(admin).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var staff = Staff.Create(Email.Create("staff@campsitebooking.dk"), "Staff", "Member", "EMP001", CampsiteId.Create(campsiteIdsForUsers[0]), DateTime.UtcNow.AddYears(-2), "", "+45 56 78 90 12", "Denmark");
            staff.SetPasswordHash(passwordHasher.HashPassword(staff, "Staff123!"));
            await context.Users.AddAsync(staff);
            await context.SaveChangesAsync();
            context.Entry(staff).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            Console.WriteLine("✅ Seeded users with hashed passwords:");
            Console.WriteLine("   - john.doe@example.com / Password123!");
            Console.WriteLine("   - jane.smith@example.com / Password123!");
            Console.WriteLine("   - mike.johnson@example.com / Password123!");
            Console.WriteLine("   - admin@campsitebooking.dk / Admin123!");
            Console.WriteLine("   - staff@campsitebooking.dk / Staff123!");
        }

        // Check if photos already exist
        if (await context.Photos.AnyAsync())
        {
            Console.WriteLine("⏭️  Photos already seeded, skipping...");
            return;
        }

        Console.WriteLine("🔄 Seeding photos...");

        // Seed "All Campsites" Activity Photos (CampsiteId = 0)
        var campsiteIdZero = CampsiteId.CreateNew(); // CampsiteId with value 0 for "All Campsites"

        var activityPhotosData = new[]
        {
            new { Url = "https://images.unsplash.com/photo-1571068316344-75bc76f77890?w=800&q=80", Caption = "Cycling", AltText = "Explore Denmark's extensive cycling routes. Bike rentals available at most locations.", DisplayOrder = 1 },
            new { Url = "https://images.unsplash.com/photo-1551632811-561732d1e306?w=800&q=80", Caption = "Hiking", AltText = "Discover scenic hiking trails ranging from easy walks to challenging treks.", DisplayOrder = 2 },
            new { Url = "https://images.unsplash.com/photo-1476514525535-07fb3b4ae5f1?w=800&q=80", Caption = "Swimming", AltText = "Enjoy swimming in the sea, lakes, or on-site swimming pools at select locations.", DisplayOrder = 3 },
            new { Url = "https://images.unsplash.com/photo-1544551763-46a013bb70d5?w=800&q=80", Caption = "Water Sports", AltText = "Kayaking, sailing, and paddleboarding available at coastal and lakeside sites.", DisplayOrder = 4 },
            new { Url = "https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=800&q=80", Caption = "Fishing", AltText = "Fishing opportunities in nearby lakes, rivers, and coastal waters.", DisplayOrder = 5 },
            new { Url = "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?w=800&q=80", Caption = "Dining", AltText = "On-site restaurants and nearby dining options featuring local Danish cuisine.", DisplayOrder = 6 },
            new { Url = "https://images.unsplash.com/photo-1535131749006-b7f58c99034b?w=800&q=80", Caption = "Golfing", AltText = "Enjoy golfing at nearby courses with beautiful Danish landscapes and coastal views.", DisplayOrder = 7 },
            new { Url = "https://images.unsplash.com/photo-1548198055-8d6fcf8a4f0c?w=800&q=80", Caption = "Fireplace Gathering", AltText = "Cozy evenings gathered around the fireplace with family and friends.", DisplayOrder = 8 }
        };

        // Add "All Campsites" photos with manually assigned IDs
        int photoId = 1;
        foreach (var photoData in activityPhotosData)
        {
            var photo = Photo.Create(
                campsiteIdZero,
                photoData.Url,
                photoData.Caption,
                photoData.AltText,
                photoData.DisplayOrder,
                null, // accommodationTypeId
                "Information" // category - set to "Information" for the Information page
            );

            // Manually set the PhotoId property to assign a unique ID
            typeof(Photo).GetProperty("PhotoId")!.SetValue(photo, photoId);
            photoId++;

            await context.Photos.AddAsync(photo);
        }

        await context.SaveChangesAsync();
        Console.WriteLine($"✅ Seeded {activityPhotosData.Length} 'All Campsites' photos");

        // Seed campsite-specific photos
        var campsitePhotosData = new[]
        {
            // Copenhagen Beach Camp (ID 1)
            new { CampsiteId = 1, Url = "https://images.unsplash.com/photo-1510312305653-8ed496efae75?w=800&q=80", Caption = "Beach View", AltText = "Beautiful beach view at Copenhagen Beach Camp", DisplayOrder = 1 },
            new { CampsiteId = 1, Url = "https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=800&q=80", Caption = "Beachside Camping", AltText = "Camping spots right by the beach", DisplayOrder = 2 },

            // Skagen North Point (ID 2)
            new { CampsiteId = 2, Url = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80", Caption = "Northern Lights", AltText = "Northern landscape at Skagen", DisplayOrder = 1 },
            new { CampsiteId = 2, Url = "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80", Caption = "Coastal Sunset", AltText = "Stunning sunset views", DisplayOrder = 2 },

            // Aarhus Forest Retreat (ID 3)
            new { CampsiteId = 3, Url = "https://images.unsplash.com/photo-1511497584788-876760111969?w=800&q=80", Caption = "Forest Trail", AltText = "Peaceful forest trails", DisplayOrder = 1 },
            new { CampsiteId = 3, Url = "https://images.unsplash.com/photo-1542273917363-3b1817f69a2d?w=800&q=80", Caption = "Forest Camping", AltText = "Camping among the trees", DisplayOrder = 2 },

            // Odense Family Camp (ID 4)
            new { CampsiteId = 4, Url = "https://images.unsplash.com/photo-1523987355523-c7b5b0dd90a7?w=800&q=80", Caption = "Family Activities", AltText = "Fun activities for the whole family", DisplayOrder = 1 },
            new { CampsiteId = 4, Url = "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80", Caption = "Playground", AltText = "Children's playground area", DisplayOrder = 2 },

            // Bornholm Island Camp (ID 5)
            new { CampsiteId = 5, Url = "https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800&q=80", Caption = "Island Paradise", AltText = "Beautiful island views at Bornholm", DisplayOrder = 1 },
            new { CampsiteId = 5, Url = "https://images.unsplash.com/photo-1510312305653-8ed496efae75?w=800&q=80", Caption = "Rocky Coastline", AltText = "Stunning rocky coastline of Bornholm", DisplayOrder = 2 },
        };

        foreach (var photoData in campsitePhotosData)
        {
            var campsiteId = CampsiteId.Create(photoData.CampsiteId);
            var photo = Photo.Create(
                campsiteId,
                photoData.Url,
                photoData.Caption,
                photoData.AltText,
                photoData.DisplayOrder,
                null,
                "Information"
            );

            typeof(Photo).GetProperty("PhotoId")!.SetValue(photo, photoId);
            photoId++;

            await context.Photos.AddAsync(photo);
        }

        await context.SaveChangesAsync();
        Console.WriteLine($"✅ Seeded {campsitePhotosData.Length} campsite-specific photos");
        Console.WriteLine($"✅ Total photos seeded: {photoId - 1}");
    }

    private static async Task SeedSeasonalPricing(CampsiteBookingDbContext context)
    {
        // Delete existing seasonal pricing
        await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0");
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE SeasonalPricings");
        await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1");

        // High Season (Summer) - June 1 to August 31
            for (int campsiteId = 1; campsiteId <= 5; campsiteId++)
            {
                var allAccommodationTypes = await context.AccommodationTypes.ToListAsync();
                var accommodationTypes = allAccommodationTypes.Where(a => a.CampsiteId?.Value == campsiteId).ToList();

                foreach (var accommodationType in accommodationTypes)
                {
                    var highSeason = SeasonalPricing.Create(
                        CampsiteId.Create(campsiteId),
                        accommodationType.Id,
                        "High Season",
                        new DateTime(DateTime.Now.Year, 6, 1),
                        new DateTime(DateTime.Now.Year, 8, 31),
                        1.5m
                    );
                    await context.SeasonalPricings.AddAsync(highSeason);
                    await context.SaveChangesAsync();
                    context.Entry(highSeason).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
            }

            // Medium Season (Spring & Fall) - April 1 to May 31, September 1 to October 31
            for (int campsiteId = 1; campsiteId <= 5; campsiteId++)
            {
                var allAccommodationTypes = await context.AccommodationTypes.ToListAsync();
                var accommodationTypes = allAccommodationTypes.Where(a => a.CampsiteId?.Value == campsiteId).ToList();

                foreach (var accommodationType in accommodationTypes)
                {
                    var mediumSeasonSpring = SeasonalPricing.Create(
                        CampsiteId.Create(campsiteId),
                        accommodationType.Id,
                        "Medium Season",
                        new DateTime(DateTime.Now.Year, 4, 1),
                        new DateTime(DateTime.Now.Year, 5, 31),
                        1.2m
                    );
                    await context.SeasonalPricings.AddAsync(mediumSeasonSpring);
                    await context.SaveChangesAsync();
                    context.Entry(mediumSeasonSpring).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                    var mediumSeasonFall = SeasonalPricing.Create(
                        CampsiteId.Create(campsiteId),
                        accommodationType.Id,
                        "Medium Season",
                        new DateTime(DateTime.Now.Year, 9, 1),
                        new DateTime(DateTime.Now.Year, 10, 31),
                        1.2m
                    );
                    await context.SeasonalPricings.AddAsync(mediumSeasonFall);
                    await context.SaveChangesAsync();
                    context.Entry(mediumSeasonFall).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
            }

            // Low Season (Winter) - November 1 to March 31
            for (int campsiteId = 1; campsiteId <= 5; campsiteId++)
            {
                var allAccommodationTypes = await context.AccommodationTypes.ToListAsync();
                var accommodationTypes = allAccommodationTypes.Where(a => a.CampsiteId?.Value == campsiteId).ToList();

                foreach (var accommodationType in accommodationTypes)
                {
                    var lowSeasonWinter = SeasonalPricing.Create(
                        CampsiteId.Create(campsiteId),
                        accommodationType.Id,
                        "Low Season",
                        new DateTime(DateTime.Now.Year, 11, 1),
                        new DateTime(DateTime.Now.Year, 12, 31),
                        0.8m
                    );
                    await context.SeasonalPricings.AddAsync(lowSeasonWinter);
                    await context.SaveChangesAsync();
                    context.Entry(lowSeasonWinter).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                    var lowSeasonEarlyYear = SeasonalPricing.Create(
                        CampsiteId.Create(campsiteId),
                        accommodationType.Id,
                        "Low Season",
                        new DateTime(DateTime.Now.Year, 1, 1),
                        new DateTime(DateTime.Now.Year, 3, 31),
                        0.8m
                    );
                    await context.SeasonalPricings.AddAsync(lowSeasonEarlyYear);
                    await context.SaveChangesAsync();
                    context.Entry(lowSeasonEarlyYear).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
            }
    }

    private static async Task SeedDiscounts(CampsiteBookingDbContext context)
    {
        // Delete existing discounts
        await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0");
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Discounts");
        await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1");
        await context.Database.ExecuteSqlRawAsync("ALTER TABLE Discounts AUTO_INCREMENT = 1");

        // Summer 2024 Discount - Active
            var summer2024 = Discount.Create(
                "SUMMER2024",
                "Summer Special Discount",
                "Percentage",
                15m,
                DateTime.Now.AddDays(-30),
                DateTime.Now.AddDays(60),
                100,
                0m
            );
            await context.Discounts.AddAsync(summer2024);
            await context.SaveChangesAsync();
            context.Entry(summer2024).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Early Bird Discount - Active
            var earlyBird = Discount.Create(
                "EARLYBIRD",
                "Early Bird Booking",
                "Percentage",
                10m,
                DateTime.Now.AddDays(-60),
                DateTime.Now.AddDays(90),
                0, // Unlimited uses
                0m
            );
            await context.Discounts.AddAsync(earlyBird);
            await context.SaveChangesAsync();
            context.Entry(earlyBird).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Family Package Discount - Active
            var family50 = Discount.Create(
                "FAMILY50",
                "Family Package Discount",
                "Fixed",
                50m,
                DateTime.Now.AddDays(-15),
                DateTime.Now.AddDays(45),
                50,
                200m
            );
            await context.Discounts.AddAsync(family50);
            await context.SaveChangesAsync();
            context.Entry(family50).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Winter 2023 Discount - Inactive (expired)
            var winter2023 = Discount.Create(
                "WINTER2023",
                "Winter Promotion",
                "Percentage",
                20m,
                DateTime.Now.AddDays(-120),
                DateTime.Now.AddDays(-30),
                100,
                0m
            );
            // Deactivate the expired discount
            winter2023.GetType().GetMethod("Deactivate")?.Invoke(winter2023, null);
            await context.Discounts.AddAsync(winter2023);
            await context.SaveChangesAsync();
            context.Entry(winter2023).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    }

    private static async Task SeedBookings(CampsiteBookingDbContext context)
    {
        // Delete existing bookings
        await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0");
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Bookings");
        await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1");

        // Get the actual campsite IDs from the database
        var campsiteIds = await context.Database.SqlQueryRaw<int>("SELECT Id as Value FROM Campsites ORDER BY Id").ToListAsync();
        if (campsiteIds.Count < 5)
        {
            Console.WriteLine($"❌ Expected 5 campsites, found {campsiteIds.Count}. Skipping bookings seeding.");
            return;
        }

        // Get all guests
            var allGuests = await context.Guests.ToListAsync();
            if (allGuests.Count == 0) return; // No guests to create bookings for

            // Get all accommodation types
            var allAccommodationTypes = await context.AccommodationTypes.ToListAsync();
            if (allAccommodationTypes.Count == 0) return; // No accommodation types

            // No longer need to synchronize ID columns since we're using the Id property directly with value converters

            // Update AccommodationSpots.AccommodationTypeId based on the mapping between spot types and accommodation types
            // The mapping is: Cabin -> Cabin, Tent -> Tent Site, Caravan -> RV Spot, Premium -> Glamping
            await context.Database.ExecuteSqlRawAsync(@"
                UPDATE AccommodationSpots AS s
                INNER JOIN AccommodationTypes AS t ON s.CampsiteId = t.CampsiteId
                SET s.AccommodationTypeId = t.Id
                WHERE s.AccommodationTypeId = 0
                AND (
                    (s.Type = 'Cabin' AND t.Type = 'Cabin') OR
                    (s.Type = 'Tent' AND t.Type = 'Tent Site') OR
                    (s.Type = 'Caravan' AND t.Type = 'RV Spot') OR
                    (s.Type = 'Premium' AND t.Type = 'Glamping')
                )
            ");

            // Get all accommodation spots
            var allAccommodationSpots = await context.AccommodationSpots.ToListAsync();
            if (allAccommodationSpots.Count == 0) return; // No accommodation spots

            // Booking 1: John Doe - Copenhagen Beach Camp - Cabin - Confirmed
            var guest1 = allGuests.FirstOrDefault(g => g.Email?.Value == "john.doe@example.com");
            var cabin1 = allAccommodationTypes.FirstOrDefault(a => a.CampsiteId?.Value == 1 && a.Type == "Cabin");
            if (guest1 != null && cabin1 != null)
            {
                var spot1 = allAccommodationSpots.FirstOrDefault(s => s.AccommodationTypeId.Value == cabin1.Id.Value);
                if (spot1 != null)
                {
                    var booking1 = Booking.Create(
                        GuestId.Create(guest1.GuestId),
                        CampsiteId.Create(campsiteIds[0]),
                        cabin1.Id,
                        DateRange.Create(DateTime.Now.AddDays(30), DateTime.Now.AddDays(37)),
                        Money.Create(150m, "DKK"),
                        2,
                        1,
                        "Please provide extra towels"
                    );
                    booking1.GetType().GetMethod("AssignAccommodationSpot")?.Invoke(booking1, new object[] { spot1.Id });
                    booking1.GetType().GetMethod("Confirm")?.Invoke(booking1, Array.Empty<object>());
                    await context.Bookings.AddAsync(booking1);
                    await context.SaveChangesAsync();
                    context.Entry(booking1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
            }

            // Booking 2: Jane Smith - Skagen North Point - Tent Site - Pending
            var guest2 = allGuests.FirstOrDefault(g => g.Email?.Value == "jane.smith@example.com");
            var tent2 = allAccommodationTypes.FirstOrDefault(a => a.CampsiteId?.Value == 2 && a.Type == "Tent Site");
            if (guest2 != null && tent2 != null)
            {
                var spot2 = allAccommodationSpots.FirstOrDefault(s => s.AccommodationTypeId.Value == tent2.Id.Value);
                if (spot2 != null)
                {
                    var booking2 = Booking.Create(
                        GuestId.Create(guest2.GuestId),
                        CampsiteId.Create(campsiteIds[1]),
                        tent2.Id,
                        DateRange.Create(DateTime.Now.AddDays(15), DateTime.Now.AddDays(18)),
                        Money.Create(55m, "DKK"),
                        2,
                        0,
                        ""
                    );
                    booking2.GetType().GetMethod("AssignAccommodationSpot")?.Invoke(booking2, new object[] { spot2.Id });
                    await context.Bookings.AddAsync(booking2);
                    await context.SaveChangesAsync();
                    context.Entry(booking2).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
            }

            // Booking 3: Mike Johnson - Aarhus Forest Retreat - Cabin - Confirmed
            var guest3 = allGuests.FirstOrDefault(g => g.Email?.Value == "mike.johnson@example.com");
            var cabin3 = allAccommodationTypes.FirstOrDefault(a => a.CampsiteId?.Value == 3 && a.Type == "Cabin");
            if (guest3 != null && cabin3 != null)
            {
                var spot3 = allAccommodationSpots.FirstOrDefault(s => s.AccommodationTypeId.Value == cabin3.Id.Value);
                if (spot3 != null)
                {
                    var booking3 = Booking.Create(
                        GuestId.Create(guest3.GuestId),
                        CampsiteId.Create(campsiteIds[2]),
                        cabin3.Id,
                        DateRange.Create(DateTime.Now.AddDays(45), DateTime.Now.AddDays(52)),
                        Money.Create(120m, "DKK"),
                        2,
                        2,
                        "Arriving late, please keep key at reception"
                    );
                    booking3.GetType().GetMethod("AssignAccommodationSpot")?.Invoke(booking3, new object[] { spot3.Id });
                    booking3.GetType().GetMethod("Confirm")?.Invoke(booking3, Array.Empty<object>());
                    await context.Bookings.AddAsync(booking3);
                    await context.SaveChangesAsync();
                    context.Entry(booking3).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
            }

            // Booking 4: John Doe - Copenhagen Beach Camp - Glamping - Completed (changed to future dates to avoid validation error)
            if (guest1 != null)
            {
                var glamping1 = allAccommodationTypes.FirstOrDefault(a => a.CampsiteId?.Value == 1 && a.Type == "Glamping");
                if (glamping1 != null)
                {
                    var spot4 = allAccommodationSpots.FirstOrDefault(s => s.AccommodationTypeId.Value == glamping1.Id.Value);
                    if (spot4 != null)
                    {
                        var booking4 = Booking.Create(
                            GuestId.Create(guest1.GuestId),
                            CampsiteId.Create(campsiteIds[0]),
                            glamping1.Id,
                            DateRange.Create(DateTime.Now.AddDays(60), DateTime.Now.AddDays(65)),
                            Money.Create(220m, "DKK"),
                            2,
                            0,
                            ""
                        );
                        booking4.GetType().GetMethod("AssignAccommodationSpot")?.Invoke(booking4, new object[] { spot4.Id });
                        booking4.GetType().GetMethod("Confirm")?.Invoke(booking4, Array.Empty<object>());
                        booking4.GetType().GetMethod("Complete")?.Invoke(booking4, Array.Empty<object>());
                        await context.Bookings.AddAsync(booking4);
                        await context.SaveChangesAsync();
                        context.Entry(booking4).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    }
                }
            }

            // Booking 5: Jane Smith - Bornholm Island Camp - Cabin - Cancelled
            if (guest2 != null)
            {
                var cabin5 = allAccommodationTypes.FirstOrDefault(a => a.CampsiteId?.Value == 5 && a.Type == "Cabin");
                if (cabin5 != null)
                {
                    var spot5 = allAccommodationSpots.FirstOrDefault(s => s.AccommodationTypeId.Value == cabin5.Id.Value);
                    if (spot5 != null)
                    {
                        var booking5 = Booking.Create(
                            GuestId.Create(guest2.GuestId),
                            CampsiteId.Create(campsiteIds[4]),
                            cabin5.Id,
                            DateRange.Create(DateTime.Now.AddDays(60), DateTime.Now.AddDays(67)),
                            Money.Create(140m, "DKK"),
                            2,
                            1,
                            ""
                        );
                        booking5.GetType().GetMethod("AssignAccommodationSpot")?.Invoke(booking5, new object[] { spot5.Id });
                        booking5.GetType().GetMethod("Cancel")?.Invoke(booking5, Array.Empty<object>());
                        await context.Bookings.AddAsync(booking5);
                        await context.SaveChangesAsync();
                        context.Entry(booking5).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                    }
                }
            }

            // No longer need to synchronize BookingId column since we're using the Id property directly with value converters
    }

    private static async Task SeedAccommodationSpots(CampsiteBookingDbContext context)
    {
        if (await context.AccommodationSpots.AnyAsync())
        {
            Console.WriteLine("⚠️ Accommodation spots already exist, skipping seeding");
            return;
        }

        Console.WriteLine("🔵 Seeding accommodation spots...");

        // Get all accommodation types with their campsites
        var allAccommodationTypes = await context.AccommodationTypes.ToListAsync();
        if (allAccommodationTypes.Count == 0)
        {
            Console.WriteLine("⚠️ No accommodation types found, skipping spot seeding");
            return;
        }

        // Get the actual campsite IDs from the database
        var campsiteIds = await context.Database.SqlQueryRaw<int>("SELECT Id as Value FROM Campsites ORDER BY Id").ToListAsync();

        // If no campsites found, skip spot seeding
        if (campsiteIds.Count == 0)
        {
            Console.WriteLine("⚠️ No campsites found in database, skipping spot seeding");
            return;
        }

        // Get all campsites for campsite names using raw SQL to avoid EF Core issues with strongly-typed IDs
        var campsiteData = await context.Database.SqlQueryRaw<CampsiteRawData>(
            "SELECT Id, Name FROM Campsites ORDER BY Id"
        ).ToListAsync();

        // Create dictionary using database ID
        var campsiteDict = campsiteData.ToDictionary(c => c.Id, c => c.Name ?? "Unknown");

        int spotCounter = 1;
        int totalSpotsCreated = 0;

        foreach (var accommodationType in allAccommodationTypes)
        {
            // Create spots based on the available units for this accommodation type
            int totalSpots = accommodationType.AvailableUnits;
            var campsiteId = accommodationType.CampsiteId?.Value ?? 1;
            var campsiteName = campsiteDict.ContainsKey(campsiteId) ? campsiteDict[campsiteId] : "Unknown";

            // Map accommodation type to spot type
            string spotType = accommodationType.Type switch
            {
                "Cabin" => "Cabin",
                "Tent Site" => "Tent",
                "RV Spot" => "Caravan",
                "Glamping" => "Premium",
                _ => "Tent"
            };

            for (int i = 1; i <= totalSpots; i++)
            {
                var spot = AccommodationSpot.Create(
                    $"{accommodationType.Type}-{i}",
                    accommodationType.CampsiteId ?? CampsiteId.Create(campsiteIds[0]),
                    campsiteName,
                    accommodationType.Id,
                    spotType,
                    55.6761 + (spotCounter * 0.0001), // Slightly offset latitude
                    12.5683 + (spotCounter * 0.0001), // Slightly offset longitude
                    1.0m // Default price modifier
                );

                await context.AccommodationSpots.AddAsync(spot);
                await context.SaveChangesAsync();
                context.Entry(spot).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                spotCounter++;
                totalSpotsCreated++;
            }
        }

        Console.WriteLine($"✅ Seeded {totalSpotsCreated} accommodation spots");

        // Synchronize strongly-typed ID columns with auto-increment ID columns
        await context.Database.ExecuteSqlRawAsync("UPDATE AccommodationTypes SET AccommodationTypeId = Id");
        await context.Database.ExecuteSqlRawAsync("UPDATE Campsites SET CampsiteId = Id");
        await context.Database.ExecuteSqlRawAsync(@"
            UPDATE AccommodationSpots AS s
            INNER JOIN AccommodationTypes AS t ON s.Type = t.Type AND s.CampsiteId = t.CampsiteId
            SET s.AccommodationTypeId = t.Id
            WHERE s.AccommodationTypeId = 0
        ");
        Console.WriteLine("✅ Synchronized strongly-typed ID columns");
    }

    private static async Task SeedEvents(CampsiteBookingDbContext context)
    {
        // Force re-seed events to ensure they have proper CampsiteId values
        if (await context.Events.AnyAsync())
        {
            Console.WriteLine("🔄 Events exist - clearing and re-seeding to ensure proper data...");
            await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Events");
            await context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1");
        }

        Console.WriteLine("🔵 Seeding events...");

        // Get the actual campsite IDs from the database
        var campsiteIds = await context.Database.SqlQueryRaw<int>("SELECT Id as Value FROM Campsites ORDER BY Id").ToListAsync();
        if (campsiteIds.Count < 5)
        {
            Console.WriteLine($"❌ Expected 5 campsites, found {campsiteIds.Count}. Skipping events seeding.");
            return;
        }

        // Event 1: Copenhagen Beach Camp - Summer Beach Party
        var event1 = Event.Create(
            CampsiteId.Create(campsiteIds[0]),
            "Summer Beach Party",
            "Join us for a fantastic beach party with live music, BBQ, and beach games. Fun for the whole family!",
            DateTime.Now.AddDays(45),
            100,
            Money.Create(0m, "DKK") // Free event
        );
        await context.Events.AddAsync(event1);
        await context.SaveChangesAsync();
        context.Entry(event1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Event 2: Skagen North Point - Northern Lights Photography Workshop
        var event2 = Event.Create(
            CampsiteId.Create(campsiteIds[1]),
            "Northern Lights Photography Workshop",
            "Learn to capture the stunning northern landscapes and night sky with professional photographers.",
            DateTime.Now.AddDays(60),
            30,
            Money.Create(150m, "DKK")
        );
        await context.Events.AddAsync(event2);
        await context.SaveChangesAsync();
        context.Entry(event2).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Event 3: Aarhus Forest Retreat - Forest Hiking Tour
        var event3 = Event.Create(
            CampsiteId.Create(campsiteIds[2]),
            "Guided Forest Hiking Tour",
            "Explore the ancient Danish forests with our experienced guides. Learn about local flora and fauna.",
            DateTime.Now.AddDays(30),
            50,
            Money.Create(50m, "DKK")
        );
        await context.Events.AddAsync(event3);
        await context.SaveChangesAsync();
        context.Entry(event3).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Event 4: Odense Family Camp - Kids Adventure Day
        var event4 = Event.Create(
            CampsiteId.Create(campsiteIds[3]),
            "Kids Adventure Day",
            "A full day of activities for children including treasure hunts, crafts, and outdoor games.",
            DateTime.Now.AddDays(20),
            80,
            Money.Create(25m, "DKK")
        );
        await context.Events.AddAsync(event4);
        await context.SaveChangesAsync();
        context.Entry(event4).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Event 5: Bornholm Island Camp - Island Cycling Tour
        var event5 = Event.Create(
            CampsiteId.Create(campsiteIds[4]),
            "Bornholm Island Cycling Tour",
            "Discover the beauty of Bornholm on two wheels. Guided cycling tour around the island's highlights.",
            DateTime.Now.AddDays(50),
            40,
            Money.Create(100m, "DKK")
        );
        await context.Events.AddAsync(event5);
        await context.SaveChangesAsync();
        context.Entry(event5).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Event 6: Copenhagen Beach Camp - Yoga on the Beach
        var event6 = Event.Create(
            CampsiteId.Create(campsiteIds[0]),
            "Sunrise Yoga on the Beach",
            "Start your day with peaceful yoga sessions on the beach. All levels welcome.",
            DateTime.Now.AddDays(15),
            25,
            Money.Create(75m, "DKK")
        );
        await context.Events.AddAsync(event6);
        await context.SaveChangesAsync();
        context.Entry(event6).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        Console.WriteLine("✅ Seeded 6 events across all campsites");
    }

    private static async Task SeedNewsletters(CampsiteBookingDbContext context)
    {
        if (await context.Newsletters.AnyAsync())
        {
            Console.WriteLine("⚠️ Newsletters already exist, skipping seeding");
            return;
        }

        Console.WriteLine("🔵 Seeding newsletters...");

        // Newsletter 1: Summer 2025 Special Offers (Sent)
        var newsletter1 = Newsletter.Create(
            "Summer 2025 Special Offers",
            "Get ready for summer! Book now and save up to 20% on all accommodation types. Limited time offer!",
            DateTime.UtcNow.AddDays(1) // Create with future date first
        );
        newsletter1.Send(250); // Mark as sent with 250 recipients
        await context.Newsletters.AddAsync(newsletter1);
        await context.SaveChangesAsync();
        context.Entry(newsletter1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Newsletter 2: New Campsite Opening (Sent)
        var newsletter2 = Newsletter.Create(
            "New Campsite Opening in Bornholm",
            "We're excited to announce the opening of our newest location on the beautiful island of Bornholm!",
            DateTime.UtcNow.AddDays(1)
        );
        newsletter2.Send(300);
        await context.Newsletters.AddAsync(newsletter2);
        await context.SaveChangesAsync();
        context.Entry(newsletter2).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Newsletter 3: Winter Activities (Sent)
        var newsletter3 = Newsletter.Create(
            "Winter Activities Guide",
            "Discover the magic of winter camping in Denmark. Check out our guide to winter activities and cozy accommodations.",
            DateTime.UtcNow.AddDays(1)
        );
        newsletter3.Send(280);
        await context.Newsletters.AddAsync(newsletter3);
        await context.SaveChangesAsync();
        context.Entry(newsletter3).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Newsletter 4: Upcoming Events (Scheduled)
        var newsletter4 = Newsletter.Create(
            "Upcoming Events This Season",
            "Don't miss our exciting lineup of events including beach parties, hiking tours, and photography workshops!",
            DateTime.UtcNow.AddDays(7)
        );
        newsletter4.Schedule(DateTime.UtcNow.AddDays(7));
        await context.Newsletters.AddAsync(newsletter4);
        await context.SaveChangesAsync();
        context.Entry(newsletter4).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        // Newsletter 5: Draft Newsletter (not published)
        var newsletter5 = Newsletter.Create(
            "Fall Season Preview",
            "Get a sneak peek at our fall season offerings and special autumn discounts. Coming soon!",
            DateTime.UtcNow.AddDays(14)
        );
        // Leave as Draft (default status)
        await context.Newsletters.AddAsync(newsletter5);
        await context.SaveChangesAsync();
        context.Entry(newsletter5).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

        Console.WriteLine("✅ Seeded 5 newsletters (3 sent, 1 scheduled, 1 draft)");
    }

    // ============================================================================
    // SEED PAYMENTS
    // ============================================================================
    private static async Task SeedPayments(CampsiteBookingDbContext context)
    {
        if (await context.Payments.AnyAsync())
        {
            Console.WriteLine("⏭️  Payments already seeded, skipping...");
            return;
        }

        Console.WriteLine("🔄 Seeding payments...");

        // Get all bookings to link payments
        var bookings = await context.Bookings.ToListAsync();
        if (!bookings.Any())
        {
            Console.WriteLine("❌ No bookings found, cannot seed payments");
            return;
        }

        var paymentMethods = new[] { "CreditCard", "DebitCard", "MobilePay", "BankTransfer", "Cash" };
        var random = new Random(42); // Fixed seed for reproducibility

        foreach (var booking in bookings)
        {
            var method = paymentMethods[random.Next(paymentMethods.Length)];
            var transactionId = $"TXN-{DateTime.UtcNow.Ticks}-{random.Next(1000, 9999)}";

            var payment = Payment.Create(
                BookingId.Create(booking.BookingId),
                booking.TotalPrice,
                method,
                transactionId
            );

            // Mark most payments as completed, some as pending
            if (random.Next(100) < 80) // 80% completed
            {
                payment.MarkAsCompleted();
            }

            await context.Payments.AddAsync(payment);
            await context.SaveChangesAsync();
            context.Entry(payment).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        Console.WriteLine($"✅ Seeded {bookings.Count} payments");
    }

    // ============================================================================
    // SEED REVIEWS
    // ============================================================================
    private static async Task SeedReviews(CampsiteBookingDbContext context)
    {
        if (await context.Reviews.AnyAsync())
        {
            Console.WriteLine("⏭️  Reviews already seeded, skipping...");
            return;
        }

        Console.WriteLine("🔄 Seeding reviews...");

        // Get users and campsites
        var users = await context.Users.ToListAsync();
        var campsites = await context.Campsites.ToListAsync();
        var bookings = await context.Bookings.ToListAsync();

        if (!users.Any() || !campsites.Any())
        {
            Console.WriteLine("❌ No users or campsites found, cannot seed reviews");
            return;
        }

        var reviewsData = new[]
        {
            new { CampsiteId = 1, UserId = 1, Rating = 5, Comment = "Absolutely amazing campsite! The beach access was perfect and the facilities were spotless. Will definitely come back!", ReviewerName = "John Doe", BookingId = (int?)1 },
            new { CampsiteId = 1, UserId = 2, Rating = 4, Comment = "Great location and friendly staff. Only minor issue was the WiFi could be stronger.", ReviewerName = "Jane Smith", BookingId = (int?)null },
            new { CampsiteId = 2, UserId = 3, Rating = 5, Comment = "The northern views are breathtaking! Perfect for a peaceful getaway. Highly recommended.", ReviewerName = "Mike Johnson", BookingId = (int?)2 },
            new { CampsiteId = 2, UserId = 1, Rating = 3, Comment = "Nice campsite but a bit remote. Good for those seeking solitude.", ReviewerName = "John Doe", BookingId = (int?)null },
            new { CampsiteId = 3, UserId = 2, Rating = 5, Comment = "The forest setting is magical! Kids loved the hiking trails. Perfect family destination.", ReviewerName = "Jane Smith", BookingId = (int?)3 },
            new { CampsiteId = 3, UserId = 3, Rating = 4, Comment = "Very peaceful and well-maintained. Great for nature lovers.", ReviewerName = "Mike Johnson", BookingId = (int?)null },
            new { CampsiteId = 4, UserId = 1, Rating = 5, Comment = "Best family campsite we've ever visited! The playground and activities kept the kids entertained all day.", ReviewerName = "John Doe", BookingId = (int?)4 },
            new { CampsiteId = 4, UserId = 2, Rating = 4, Comment = "Excellent facilities for families. Very clean and organized.", ReviewerName = "Jane Smith", BookingId = (int?)null },
            new { CampsiteId = 5, UserId = 3, Rating = 5, Comment = "Bornholm is beautiful and this campsite captures it perfectly. The island charm is real!", ReviewerName = "Mike Johnson", BookingId = (int?)5 },
            new { CampsiteId = 5, UserId = 1, Rating = 4, Comment = "Lovely island location. Worth the ferry trip!", ReviewerName = "John Doe", BookingId = (int?)null },
        };

        foreach (var reviewData in reviewsData)
        {
            var review = Review.Create(
                CampsiteId.Create(reviewData.CampsiteId),
                UserId.Create(reviewData.UserId),
                reviewData.Rating,
                reviewData.Comment,
                reviewData.ReviewerName,
                reviewData.BookingId.HasValue ? BookingId.Create(reviewData.BookingId.Value) : null
            );

            // Approve most reviews (80%)
            var random = new Random(reviewData.CampsiteId * 100 + reviewData.UserId);
            if (random.Next(100) < 80)
            {
                review.Approve();
            }

            await context.Reviews.AddAsync(review);
            await context.SaveChangesAsync();
            context.Entry(review).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        Console.WriteLine($"✅ Seeded {reviewsData.Length} reviews");
    }

    // ============================================================================
    // SEED AMENITIES
    // ============================================================================
    private static async Task SeedAmenities(CampsiteBookingDbContext context)
    {
        // Only seed amenities if none exist (allow admin-managed amenities to persist)
        if (await context.Amenities.AnyAsync())
        {
            Console.WriteLine("⏭️  Amenities already exist, skipping seeding...");
            return;
        }

        Console.WriteLine("🔄 Seeding amenities...");

        var amenitiesData = new[]
        {
            // Copenhagen Beach Camp (ID 1)
            new { CampsiteId = 1, Name = "WiFi", Description = "Free high-speed WiFi throughout the campsite", IconUrl = "wifi-icon.svg", Category = "Facilities" },
            new { CampsiteId = 1, Name = "Swimming Pool", Description = "Heated outdoor swimming pool", IconUrl = "pool-icon.svg", Category = "Facilities" },
            new { CampsiteId = 1, Name = "Beach Access", Description = "Direct access to private beach", IconUrl = "beach-icon.svg", Category = "Activities" },
            new { CampsiteId = 1, Name = "Restaurant", Description = "On-site restaurant serving local cuisine", IconUrl = "restaurant-icon.svg", Category = "Services" },
            new { CampsiteId = 1, Name = "Showers", Description = "Modern shower facilities with hot water", IconUrl = "shower-icon.svg", Category = "Facilities" },

            // Skagen North Point (ID 2)
            new { CampsiteId = 2, Name = "WiFi", Description = "Free WiFi in common areas", IconUrl = "wifi-icon.svg", Category = "Facilities" },
            new { CampsiteId = 2, Name = "Hiking Trails", Description = "Access to scenic coastal hiking trails", IconUrl = "hiking-icon.svg", Category = "Activities" },
            new { CampsiteId = 2, Name = "Laundry", Description = "Coin-operated laundry facilities", IconUrl = "laundry-icon.svg", Category = "Services" },
            new { CampsiteId = 2, Name = "Playground", Description = "Children's playground area", IconUrl = "playground-icon.svg", Category = "Activities" },

            // Aarhus Forest Retreat (ID 3)
            new { CampsiteId = 3, Name = "WiFi", Description = "Free WiFi coverage", IconUrl = "wifi-icon.svg", Category = "Facilities" },
            new { CampsiteId = 3, Name = "Nature Trails", Description = "Forest walking and biking trails", IconUrl = "trail-icon.svg", Category = "Activities" },
            new { CampsiteId = 3, Name = "Bike Rental", Description = "Mountain bike rental service", IconUrl = "bike-icon.svg", Category = "Services" },
            new { CampsiteId = 3, Name = "Campfire Areas", Description = "Designated campfire and BBQ areas", IconUrl = "fire-icon.svg", Category = "Facilities" },
            new { CampsiteId = 3, Name = "Showers", Description = "Clean shower facilities", IconUrl = "shower-icon.svg", Category = "Facilities" },
        };

        foreach (var amenityData in amenitiesData)
        {
            var amenity = Amenity.Create(
                CampsiteId.Create(amenityData.CampsiteId),
                amenityData.Name,
                amenityData.Description,
                amenityData.IconUrl,
                amenityData.Category
            );

            await context.Amenities.AddAsync(amenity);
            await context.SaveChangesAsync();
            context.Entry(amenity).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        Console.WriteLine($"✅ Seeded {amenitiesData.Length} amenities");
    }

    // ============================================================================
    // SEED EVENT REGISTRATIONS
    // ============================================================================
    private static async Task SeedEventRegistrations(CampsiteBookingDbContext context)
    {
        if (await context.EventRegistrations.AnyAsync())
        {
            Console.WriteLine("⏭️  Event registrations already seeded, skipping...");
            return;
        }

        Console.WriteLine("🔄 Seeding event registrations...");

        var events = await context.Events.ToListAsync();
        var users = await context.Users.Where(u => u is Guest).ToListAsync();

        if (!events.Any() || !users.Any())
        {
            Console.WriteLine("❌ No events or guest users found, cannot seed event registrations");
            return;
        }

        var registrationsData = new[]
        {
            new { EventId = 1, UserId = 1, Participants = 2, Names = "John Doe, Jane Doe", SpecialRequests = "Vegetarian meal options please" },
            new { EventId = 1, UserId = 2, Participants = 1, Names = "Jane Smith", SpecialRequests = "" },
            new { EventId = 2, UserId = 3, Participants = 3, Names = "Mike Johnson, Sarah Johnson, Tommy Johnson", SpecialRequests = "Child seat needed for 5-year-old" },
            new { EventId = 3, UserId = 1, Participants = 2, Names = "John Doe, Jane Doe", SpecialRequests = "Beginner level please" },
            new { EventId = 4, UserId = 2, Participants = 4, Names = "Jane Smith, Emma Smith, Oliver Smith, Sophia Smith", SpecialRequests = "Two children ages 6 and 8" },
            new { EventId = 5, UserId = 3, Participants = 1, Names = "Mike Johnson", SpecialRequests = "Interested in advanced techniques" },
            new { EventId = 6, UserId = 1, Participants = 2, Names = "John Doe, Jane Doe", SpecialRequests = "" },
        };

        foreach (var regData in registrationsData)
        {
            var registration = EventRegistration.Create(
                CampsiteBooking.Models.ValueObjects.EventId.Create(regData.EventId),
                UserId.Create(regData.UserId),
                regData.Participants,
                regData.Names,
                regData.SpecialRequests
            );

            // Register participants with the event
            var eventEntity = events.FirstOrDefault(e => e.EventId == regData.EventId);
            if (eventEntity != null)
            {
                eventEntity.RegisterParticipants(regData.Participants);
                context.Events.Update(eventEntity);
            }

            await context.EventRegistrations.AddAsync(registration);
            await context.SaveChangesAsync();
            context.Entry(registration).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        Console.WriteLine($"✅ Seeded {registrationsData.Length} event registrations");
    }

    // ============================================================================
    // SEED NEWSLETTER SUBSCRIPTIONS
    // ============================================================================
    private static async Task SeedNewsletterSubscriptions(CampsiteBookingDbContext context)
    {
        if (await context.NewsletterSubscriptions.AnyAsync())
        {
            Console.WriteLine("⏭️  Newsletter subscriptions already seeded, skipping...");
            return;
        }

        Console.WriteLine("🔄 Seeding newsletter subscriptions...");

        var subscriptionsData = new[]
        {
            new { Email = "john.doe@example.com", FirstName = "John", LastName = "Doe", Language = "en", IsActive = true },
            new { Email = "jane.smith@example.com", FirstName = "Jane", LastName = "Smith", Language = "da", IsActive = true },
            new { Email = "mike.johnson@example.com", FirstName = "Mike", LastName = "Johnson", Language = "sv", IsActive = true },
            new { Email = "sarah.wilson@example.com", FirstName = "Sarah", LastName = "Wilson", Language = "en", IsActive = false },
            new { Email = "peter.hansen@example.com", FirstName = "Peter", LastName = "Hansen", Language = "da", IsActive = true },
            new { Email = "anna.larsen@example.com", FirstName = "Anna", LastName = "Larsen", Language = "da", IsActive = true },
            new { Email = "erik.nielsen@example.com", FirstName = "Erik", LastName = "Nielsen", Language = "no", IsActive = true },
        };

        foreach (var subData in subscriptionsData)
        {
            var subscription = NewsletterSubscription.Create(
                Email.Create(subData.Email),
                subData.FirstName,
                subData.LastName,
                subData.Language
            );

            // Unsubscribe inactive ones
            if (!subData.IsActive)
            {
                subscription.Unsubscribe();
            }

            await context.NewsletterSubscriptions.AddAsync(subscription);
            await context.SaveChangesAsync();
            context.Entry(subscription).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        Console.WriteLine($"✅ Seeded {subscriptionsData.Length} newsletter subscriptions");
    }

    // ============================================================================
    // SEED PERIPHERAL PURCHASES
    // ============================================================================
    private static async Task SeedPeripheralPurchases(CampsiteBookingDbContext context)
    {
        if (await context.PeripheralPurchases.AnyAsync())
        {
            Console.WriteLine("⏭️  Peripheral purchases already seeded, skipping...");
            return;
        }

        Console.WriteLine("🔄 Seeding peripheral purchases...");

        var bookings = await context.Bookings.ToListAsync();
        if (!bookings.Any())
        {
            Console.WriteLine("❌ No bookings found, cannot seed peripheral purchases");
            return;
        }

        var purchasesData = new[]
        {
            new { BookingId = 1, ItemName = "Firewood Bundle", Description = "Bundle of seasoned firewood for campfire", Quantity = 2, UnitPrice = 50m },
            new { BookingId = 1, ItemName = "Bike Rental", Description = "Mountain bike rental for 3 days", Quantity = 2, UnitPrice = 150m },
            new { BookingId = 2, ItemName = "Late Checkout", Description = "Late checkout until 2 PM", Quantity = 1, UnitPrice = 100m },
            new { BookingId = 2, ItemName = "Firewood Bundle", Description = "Bundle of seasoned firewood", Quantity = 1, UnitPrice = 50m },
            new { BookingId = 3, ItemName = "Early Check-in", Description = "Early check-in from 10 AM", Quantity = 1, UnitPrice = 75m },
            new { BookingId = 3, ItemName = "BBQ Grill Rental", Description = "Portable BBQ grill rental", Quantity = 1, UnitPrice = 100m },
            new { BookingId = 4, ItemName = "Kayak Rental", Description = "Kayak rental for 2 days", Quantity = 2, UnitPrice = 200m },
            new { BookingId = 4, ItemName = "Firewood Bundle", Description = "Bundle of seasoned firewood", Quantity = 3, UnitPrice = 50m },
            new { BookingId = 5, ItemName = "Bike Rental", Description = "City bike rental for 4 days", Quantity = 1, UnitPrice = 120m },
            new { BookingId = 5, ItemName = "Beach Equipment", Description = "Beach chairs and umbrella set", Quantity = 1, UnitPrice = 80m },
        };

        foreach (var purchaseData in purchasesData)
        {
            var purchase = PeripheralPurchase.Create(
                BookingId.Create(purchaseData.BookingId),
                purchaseData.ItemName,
                purchaseData.Description,
                purchaseData.Quantity,
                Money.Create(purchaseData.UnitPrice, "DKK")
            );

            // Confirm most purchases (90%)
            var random = new Random(purchaseData.BookingId * 100);
            if (random.Next(100) < 90)
            {
                purchase.Confirm();

                // Mark some as delivered (70% of confirmed)
                if (random.Next(100) < 70)
                {
                    purchase.MarkAsDelivered();
                }
            }

            await context.PeripheralPurchases.AddAsync(purchase);
            await context.SaveChangesAsync();
            context.Entry(purchase).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        Console.WriteLine($"✅ Seeded {purchasesData.Length} peripheral purchases");
    }

    // ============================================================================
    // SEED NEWSLETTER ANALYTICS
    // ============================================================================
    private static async Task SeedNewsletterAnalytics(CampsiteBookingDbContext context)
    {
        if (await context.NewsletterAnalytics.AnyAsync())
        {
            Console.WriteLine("⏭️  Newsletter analytics already seeded, skipping...");
            return;
        }

        Console.WriteLine("🔄 Seeding newsletter analytics...");

        // Load all newsletters and filter in memory (Status is not mapped to database)
        var allNewsletters = await context.Newsletters.ToListAsync();
        var newsletters = allNewsletters.Where(n => n.Status == "Sent").ToList();
        if (!newsletters.Any())
        {
            Console.WriteLine("⏭️  No sent newsletters found, skipping analytics");
            return;
        }

        foreach (var newsletter in newsletters)
        {
            var analytics = NewsletterAnalytics.Create(
                NewsletterId.Create(newsletter.NewsletterId),
                newsletter.RecipientCount
            );

            // Simulate realistic open and click rates
            var random = new Random(newsletter.NewsletterId * 42);
            var openRate = random.Next(30, 70); // 30-70% open rate
            var clickRate = random.Next(10, 30); // 10-30% click rate (of opens)

            var totalOpened = (int)(newsletter.RecipientCount * openRate / 100.0);
            var totalClicked = (int)(totalOpened * clickRate / 100.0);
            var totalUnsubscribed = random.Next(0, 3); // 0-2 unsubscribes per newsletter

            // Record opens
            for (int i = 0; i < totalOpened; i++)
            {
                analytics.RecordOpen();
            }

            // Record clicks
            for (int i = 0; i < totalClicked; i++)
            {
                analytics.RecordClick();
            }

            // Record unsubscribes
            for (int i = 0; i < totalUnsubscribed; i++)
            {
                analytics.RecordUnsubscribe();
            }

            await context.NewsletterAnalytics.AddAsync(analytics);
            await context.SaveChangesAsync();
            context.Entry(analytics).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        Console.WriteLine($"✅ Seeded {newsletters.Count} newsletter analytics records");
    }

    // ============================================================================
    // SEED AVAILABILITIES
    // ============================================================================
    private static async Task SeedAvailabilities(CampsiteBookingDbContext context)
    {
        if (await context.Availabilities.AnyAsync())
        {
            Console.WriteLine("⏭️  Availabilities already seeded, skipping...");
            return;
        }

        Console.WriteLine("🔄 Seeding availabilities...");

        // Get all accommodation types using EF Core
        // Note: AccommodationType doesn't have CampsiteId in the database, so we'll use a default value
        var accommodationTypes = await context.AccommodationTypes.ToListAsync();

        if (accommodationTypes.Count == 0)
        {
            Console.WriteLine("❌ No accommodation types found, cannot seed availabilities");
            return;
        }

        // Get campsite IDs from the database
        var campsiteIds = await context.Database.SqlQueryRaw<int>("SELECT Id as Value FROM Campsites ORDER BY Id").ToListAsync();
        if (campsiteIds.Count == 0)
        {
            Console.WriteLine("❌ No campsites found, cannot seed availabilities");
            return;
        }

        int totalAvailabilities = 0;

        // Create availability records for the next 90 days
        // Since AccommodationType doesn't store CampsiteId in the database, we'll assign them round-robin
        // Save individually to avoid tracking conflicts (all entities have Id = 0 before persistence)

        for (int i = 0; i < accommodationTypes.Count; i++)
        {
            var accommodationType = accommodationTypes[i];
            var campsiteId = campsiteIds[i % campsiteIds.Count]; // Round-robin assignment

            for (int dayOffset = 0; dayOffset < 90; dayOffset++)
            {
                var date = DateTime.UtcNow.Date.AddDays(dayOffset);

                var availability = Availability.Create(
                    CampsiteId.Create(campsiteId),
                    AccommodationTypeId.Create(i + 1), // Use 1-based index as ID
                    date,
                    accommodationType.AvailableUnits
                );

                // Simulate some reservations (random between 0 and 50% of capacity)
                var random = new Random((i + 1) * 1000 + dayOffset);
                var reservedUnits = random.Next(0, accommodationType.AvailableUnits / 2);

                if (reservedUnits > 0)
                {
                    availability.ReserveUnits(reservedUnits);
                }

                await context.Availabilities.AddAsync(availability);
                await context.SaveChangesAsync();

                // Detach to avoid tracking conflicts
                context.Entry(availability).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                totalAvailabilities++;
            }
        }

        Console.WriteLine($"✅ Seeded {totalAvailabilities} availability records (90 days × {accommodationTypes.Count} accommodation types)");
    }

    // ============================================================================
    // SEED MAINTENANCE TASKS
    // ============================================================================
    private static async Task SeedMaintenanceTasks(CampsiteBookingDbContext context)
    {
        if (await context.MaintenanceTasks.AnyAsync())
        {
            Console.WriteLine("⏭️  Maintenance tasks already seeded, skipping...");
            return;
        }

        Console.WriteLine("🔄 Seeding maintenance tasks...");

        var campsites = await context.Campsites.ToListAsync();
        var accommodationSpots = await context.AccommodationSpots.ToListAsync();
        var staffUsers = await context.Users.Where(u => u is Staff).ToListAsync();

        if (!campsites.Any())
        {
            Console.WriteLine("❌ No campsites found, cannot seed maintenance tasks");
            return;
        }

        var tasksData = new[]
        {
            new { CampsiteId = 1, SpotId = (int?)null, Title = "Pool Cleaning", Description = "Weekly pool cleaning and chemical balance check", Priority = "High", DaysFromNow = -2, Status = "Completed" },
            new { CampsiteId = 1, SpotId = (int?)null, Title = "Beach Area Maintenance", Description = "Clean beach area and repair beach chairs", Priority = "Medium", DaysFromNow = 1, Status = "Scheduled" },
            new { CampsiteId = 1, SpotId = (int?)1, Title = "Cabin Roof Repair", Description = "Fix leaking roof on Cabin A1", Priority = "Critical", DaysFromNow = 0, Status = "InProgress" },
            new { CampsiteId = 2, SpotId = (int?)null, Title = "Trail Maintenance", Description = "Clear hiking trails and repair signage", Priority = "Medium", DaysFromNow = 3, Status = "Scheduled" },
            new { CampsiteId = 2, SpotId = (int?)50, Title = "Tent Platform Repair", Description = "Replace damaged wooden platform", Priority = "High", DaysFromNow = 2, Status = "Assigned" },
            new { CampsiteId = 3, SpotId = (int?)null, Title = "Forest Path Clearing", Description = "Remove fallen branches from forest paths", Priority = "Low", DaysFromNow = 7, Status = "Pending" },
            new { CampsiteId = 3, SpotId = (int?)100, Title = "RV Hookup Repair", Description = "Fix electrical hookup at RV spot", Priority = "Critical", DaysFromNow = 0, Status = "InProgress" },
            new { CampsiteId = 4, SpotId = (int?)null, Title = "Playground Inspection", Description = "Monthly safety inspection of playground equipment", Priority = "High", DaysFromNow = 5, Status = "Scheduled" },
            new { CampsiteId = 4, SpotId = (int?)null, Title = "Shower Facility Upgrade", Description = "Install new shower heads and repair tiles", Priority = "Medium", DaysFromNow = -5, Status = "Completed" },
            new { CampsiteId = 5, SpotId = (int?)null, Title = "General Grounds Maintenance", Description = "Mow lawns and trim hedges", Priority = "Low", DaysFromNow = 4, Status = "Scheduled" },
        };

        foreach (var taskData in tasksData)
        {
            var task = MaintenanceTask.Create(
                CampsiteId.Create(taskData.CampsiteId),
                taskData.Title,
                taskData.Description,
                taskData.Priority,
                DateTime.UtcNow.AddDays(taskData.DaysFromNow),
                taskData.SpotId.HasValue ? AccommodationSpotId.Create(taskData.SpotId.Value) : null
            );

            // Apply status transitions based on desired status
            if (taskData.Status == "Assigned" && staffUsers.Any())
            {
                task.AssignTo(UserId.Create(staffUsers.First().UserId));
            }
            else if (taskData.Status == "InProgress")
            {
                if (staffUsers.Any())
                {
                    task.AssignTo(UserId.Create(staffUsers.First().UserId));
                }
                task.Start();
            }
            else if (taskData.Status == "Completed")
            {
                if (staffUsers.Any())
                {
                    task.AssignTo(UserId.Create(staffUsers.First().UserId));
                }
                task.Start();
                task.Complete();
            }

            await context.MaintenanceTasks.AddAsync(task);
            await context.SaveChangesAsync();
            context.Entry(task).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        Console.WriteLine($"✅ Seeded {tasksData.Length} maintenance tasks");
    }

    /// <summary>
    /// Sets up AUTO_INCREMENT for all tables with strongly-typed IDs.
    /// This is required because EF Core migrations don't set up AUTO_INCREMENT by default for strongly-typed ID columns.
    /// </summary>
    private static async Task SetupAutoIncrementForAllTables(CampsiteBookingDbContext context)
    {
        var tables = new[]
        {
            "Campsites",
            "AccommodationTypes",
            "AccommodationSpots",
            "Bookings",
            "Payments",
            "Discounts",
            "Events",
            "Newsletters",
            "Reviews",
            "Amenities",
            "Photos",
            "EventRegistrations",
            "NewsletterSubscriptions",
            "PeripheralPurchases",
            "NewsletterAnalytics",
            "Availabilities",
            "MaintenanceTasks",
            "SeasonalPricings"
        };

        foreach (var table in tables)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync($"ALTER TABLE {table} MODIFY COLUMN Id INT AUTO_INCREMENT");
            }
            catch
            {
                // Ignore if already auto-increment or table doesn't exist
            }
        }
    }
}

/// <summary>
/// Helper class for raw SQL queries to get campsite data
/// </summary>
public class CampsiteRawData
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

/// <summary>
/// Helper class for raw SQL queries to get booking data
/// </summary>
public class BookingRawData
{
    public int Id { get; set; }
    public int AccommodationTypeId { get; set; }
    public int CampsiteId { get; set; }
    public int GuestId { get; set; }
}
