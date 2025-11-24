using CampsiteBooking.Models;
using CampsiteBooking.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CampsiteBooking.Data;

/// <summary>
/// Seeds initial data into the database
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(CampsiteBookingDbContext context)
    {
        // Drop the UserId column if it exists (legacy column, no longer needed)
        try
        {
            await context.Database.ExecuteSqlRawAsync("ALTER TABLE Users DROP COLUMN UserId");
        }
        catch
        {
            // Column might not exist, ignore error
        }

        // Make Id column auto-increment if it isn't already
        await context.Database.ExecuteSqlRawAsync("ALTER TABLE Users MODIFY COLUMN Id INT AUTO_INCREMENT");

        // Add missing columns to Users table if they don't exist
        var columnsToAdd = new[]
        {
            "ALTER TABLE Users ADD COLUMN Email VARCHAR(255) NULL",
            "ALTER TABLE Users ADD COLUMN FirstName VARCHAR(100) NULL",
            "ALTER TABLE Users ADD COLUMN LastName VARCHAR(100) NULL",
            "ALTER TABLE Users ADD COLUMN Phone VARCHAR(20) NULL",
            "ALTER TABLE Users ADD COLUMN Country VARCHAR(100) NULL",
            "ALTER TABLE Users ADD COLUMN JoinedDate DATETIME NULL",
            "ALTER TABLE Users ADD COLUMN LastLogin DATETIME NULL",
            "ALTER TABLE Users ADD COLUMN IsActive TINYINT(1) NULL DEFAULT 1"
        };

        foreach (var sql in columnsToAdd)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync(sql);
            }
            catch
            {
                // Column might already exist, ignore error
            }
        }

        // Make Campsites Id column auto-increment if it isn't already
        try
        {
            await context.Database.ExecuteSqlRawAsync("ALTER TABLE Campsites MODIFY COLUMN Id INT AUTO_INCREMENT");
        }
        catch
        {
            // Might already be auto-increment, ignore error
        }

        // Add missing columns to Campsites table if they don't exist
        var campsiteColumnsToAdd = new[]
        {
            "ALTER TABLE Campsites ADD COLUMN Name VARCHAR(200) NULL",
            "ALTER TABLE Campsites ADD COLUMN Region VARCHAR(100) NULL",
            "ALTER TABLE Campsites ADD COLUMN Description TEXT NULL",
            "ALTER TABLE Campsites ADD COLUMN Latitude DOUBLE NULL",
            "ALTER TABLE Campsites ADD COLUMN Longitude DOUBLE NULL",
            "ALTER TABLE Campsites ADD COLUMN Attractiveness VARCHAR(50) NULL",
            "ALTER TABLE Campsites ADD COLUMN PhoneNumber VARCHAR(20) NULL",
            "ALTER TABLE Campsites ADD COLUMN Email VARCHAR(255) NULL",
            "ALTER TABLE Campsites ADD COLUMN WebsiteUrl VARCHAR(500) NULL",
            "ALTER TABLE Campsites ADD COLUMN EstablishedYear INT NULL",
            "ALTER TABLE Campsites ADD COLUMN IsActive TINYINT(1) NULL DEFAULT 1",
            "ALTER TABLE Campsites ADD COLUMN TotalArea DECIMAL(18,2) NULL",
            "ALTER TABLE Campsites ADD COLUMN CreatedDate DATETIME NULL",
            "ALTER TABLE Campsites ADD COLUMN UpdatedDate DATETIME NULL"
        };

        foreach (var sql in campsiteColumnsToAdd)
        {
            try
            {
                await context.Database.ExecuteSqlRawAsync(sql);
            }
            catch
            {
                // Column might already exist, ignore error
            }
        }

        // Delete ALL campsites (to clean up any invalid data with NULL values)
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Campsites");

        // Reset auto-increment counter for Campsites
        await context.Database.ExecuteSqlRawAsync("ALTER TABLE Campsites AUTO_INCREMENT = 1");

        // Seed Campsites first (before users, since Staff references CampsiteId)
        if (!await context.Campsites.AnyAsync())
        {
            var campsite1 = Campsite.Create(
                name: "Copenhagen Beach Camp",
                region: "Zealand",
                latitude: 55.6761,
                longitude: 12.5683,
                totalArea: 50000m,
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
                region: "North Jutland",
                latitude: 57.7209,
                longitude: 10.5882,
                totalArea: 40000m,
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
                region: "East Jutland",
                latitude: 56.1629,
                longitude: 10.2039,
                totalArea: 35000m,
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
                region: "Funen",
                latitude: 55.4038,
                longitude: 10.4024,
                totalArea: 60000m,
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
                region: "Bornholm",
                latitude: 55.1367,
                longitude: 14.9155,
                totalArea: 30000m,
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
        }

        // Make AccommodationTypes Id column auto-increment if it isn't already
        try
        {
            await context.Database.ExecuteSqlRawAsync("ALTER TABLE AccommodationTypes MODIFY COLUMN Id INT AUTO_INCREMENT");
        }
        catch
        {
            // Might already be auto-increment, ignore error
        }

        // Delete ALL accommodation types (to clean up any invalid data)
        await context.Database.ExecuteSqlRawAsync("DELETE FROM AccommodationTypes");

        // Reset auto-increment counter for AccommodationTypes
        await context.Database.ExecuteSqlRawAsync("ALTER TABLE AccommodationTypes AUTO_INCREMENT = 1");

        // Seed Accommodation Types for each campsite
        if (!await context.AccommodationTypes.AnyAsync())
        {
            // Copenhagen Beach Camp (ID 1) - High attractiveness
            var cabin1 = AccommodationType.Create(
                CampsiteId.Create(1),
                "Cabin",
                6,
                Money.Create(150m, "DKK"),
                12,
                "Cozy wooden cabins with modern amenities, perfect for families",
                "https://images.unsplash.com/photo-1587061949409-02df41d5e562?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(cabin1);
            await context.SaveChangesAsync();
            context.Entry(cabin1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var tent1 = AccommodationType.Create(
                CampsiteId.Create(1),
                "Tent Site",
                4,
                Money.Create(60m, "DKK"),
                25,
                "Spacious tent sites with electricity hookup",
                "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(tent1);
            await context.SaveChangesAsync();
            context.Entry(tent1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var glamping1 = AccommodationType.Create(
                CampsiteId.Create(1),
                "Glamping",
                4,
                Money.Create(220m, "DKK"),
                8,
                "Luxury glamping tents with premium furnishings",
                "https://images.unsplash.com/photo-1504280390367-361c6d9f38f4?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(glamping1);
            await context.SaveChangesAsync();
            context.Entry(glamping1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var rv1 = AccommodationType.Create(
                CampsiteId.Create(1),
                "RV Spot",
                6,
                Money.Create(100m, "DKK"),
                15,
                "Full hookup RV spots with water, electricity, and sewage",
                "https://images.unsplash.com/photo-1523987355523-c7b5b0dd90a7?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(rv1);
            await context.SaveChangesAsync();
            context.Entry(rv1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Skagen North Point (ID 2) - Very High attractiveness
            var cabin2 = AccommodationType.Create(
                CampsiteId.Create(2),
                "Cabin",
                6,
                Money.Create(145m, "DKK"),
                10,
                "Modern cabins with sea views",
                "https://images.unsplash.com/photo-1587061949409-02df41d5e562?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(cabin2);
            await context.SaveChangesAsync();
            context.Entry(cabin2).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var tent2 = AccommodationType.Create(
                CampsiteId.Create(2),
                "Tent Site",
                4,
                Money.Create(55m, "DKK"),
                30,
                "Tent sites near the beach",
                "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(tent2);
            await context.SaveChangesAsync();
            context.Entry(tent2).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Aarhus Forest Retreat (ID 3) - High attractiveness
            var cabin3 = AccommodationType.Create(
                CampsiteId.Create(3),
                "Cabin",
                5,
                Money.Create(120m, "DKK"),
                15,
                "Forest cabins surrounded by nature",
                "https://images.unsplash.com/photo-1587061949409-02df41d5e562?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(cabin3);
            await context.SaveChangesAsync();
            context.Entry(cabin3).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var tent3 = AccommodationType.Create(
                CampsiteId.Create(3),
                "Tent Site",
                4,
                Money.Create(45m, "DKK"),
                40,
                "Peaceful forest tent sites",
                "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(tent3);
            await context.SaveChangesAsync();
            context.Entry(tent3).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Odense Family Camp (ID 4) - Medium attractiveness
            var cabin4 = AccommodationType.Create(
                CampsiteId.Create(4),
                "Cabin",
                5,
                Money.Create(100m, "DKK"),
                18,
                "Family-friendly cabins near playground",
                "https://images.unsplash.com/photo-1587061949409-02df41d5e562?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(cabin4);
            await context.SaveChangesAsync();
            context.Entry(cabin4).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var tent4 = AccommodationType.Create(
                CampsiteId.Create(4),
                "Tent Site",
                4,
                Money.Create(40m, "DKK"),
                50,
                "Large tent area for families",
                "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(tent4);
            await context.SaveChangesAsync();
            context.Entry(tent4).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            // Bornholm Island Camp (ID 5) - Very High attractiveness
            var cabin5 = AccommodationType.Create(
                CampsiteId.Create(5),
                "Cabin",
                6,
                Money.Create(140m, "DKK"),
                8,
                "Island cabins with stunning views",
                "https://images.unsplash.com/photo-1587061949409-02df41d5e562?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(cabin5);
            await context.SaveChangesAsync();
            context.Entry(cabin5).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var tent5 = AccommodationType.Create(
                CampsiteId.Create(5),
                "Tent Site",
                4,
                Money.Create(55m, "DKK"),
                20,
                "Tent sites with island charm",
                "https://images.unsplash.com/photo-1478131143081-80f7f84ca84d?w=800&q=80"
            );
            await context.AccommodationTypes.AddAsync(tent5);
            await context.SaveChangesAsync();
            context.Entry(tent5).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        // Seed Seasonal Pricing
        // await SeedSeasonalPricing(context);

        // Seed Discounts
        // await SeedDiscounts(context);

        // Seed Sample Bookings
        // await SeedBookings(context);

        // Delete ALL users (they have NULL values in new columns and need to be recreated)
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Users");

        // Reset auto-increment counter
        await context.Database.ExecuteSqlRawAsync("ALTER TABLE Users AUTO_INCREMENT = 1");

        // Check if users exist using raw SQL (to avoid EF Core trying to load NULL values)
        var userCount = await context.Database.SqlQueryRaw<int>("SELECT COUNT(*) as Value FROM Users").FirstOrDefaultAsync();

        // Seed Users if none exist
        if (userCount == 0)
        {
            // Add users one by one to avoid ID conflicts (all have ID = 0 before persistence)
            var guest1 = Guest.Create(Email.Create("john.doe@example.com"), "John", "Doe", "+45 12 34 56 78", "Denmark");
            await context.Users.AddAsync(guest1);
            await context.SaveChangesAsync();
            context.Entry(guest1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var guest2 = Guest.Create(Email.Create("jane.smith@example.com"), "Jane", "Smith", "+45 23 45 67 89", "Denmark");
            await context.Users.AddAsync(guest2);
            await context.SaveChangesAsync();
            context.Entry(guest2).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var guest3 = Guest.Create(Email.Create("mike.johnson@example.com"), "Mike", "Johnson", "+45 34 56 78 90", "Sweden");
            await context.Users.AddAsync(guest3);
            await context.SaveChangesAsync();
            context.Entry(guest3).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var admin = Admin.Create(Email.Create("admin@campsitebooking.dk"), "Admin", "User", "+45 45 67 89 01", "Denmark");
            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
            context.Entry(admin).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

            var staff = Staff.Create(Email.Create("staff@campsitebooking.dk"), "Staff", "Member", "EMP001", CampsiteId.Create(1), DateTime.UtcNow.AddYears(-2), "", "+45 56 78 90 12", "Denmark");
            await context.Users.AddAsync(staff);
            await context.SaveChangesAsync();
            context.Entry(staff).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        // Check if photos already exist
        if (await context.Photos.AnyAsync())
        {
            // Check if we need to add campsite-specific photos
            var allPhotos = await context.Photos.ToListAsync();
            var campsiteSpecificPhotos = allPhotos.Count(p => p.CampsiteId.Value > 0);
            if (campsiteSpecificPhotos > 0)
            {
                return; // Campsite-specific photos already seeded
            }

            // Add some sample photos for specific campsites
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
            };

            int nextPhotoId = await context.Photos.MaxAsync(p => p.PhotoId) + 1;

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

                typeof(Photo).GetProperty("PhotoId")!.SetValue(photo, nextPhotoId);
                nextPhotoId++;

                await context.Photos.AddAsync(photo);
            }

            await context.SaveChangesAsync();
            return;
        }

        // Seed Activity Photos (CampsiteId = 0 for "All Campsites")
        var campsiteIdZero = CampsiteId.CreateNew(); // This creates CampsiteId with value 0

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

        // Add photos with manually assigned IDs (since auto-increment is not configured)
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

        // Get all guests
            var allGuests = await context.Guests.ToListAsync();
            if (allGuests.Count == 0) return; // No guests to create bookings for

            // Get all accommodation types
            var allAccommodationTypes = await context.AccommodationTypes.ToListAsync();
            if (allAccommodationTypes.Count == 0) return; // No accommodation types

            // Booking 1: John Doe - Copenhagen Beach Camp - Cabin - Confirmed
            var guest1 = allGuests.FirstOrDefault(g => g.Email?.Value == "john.doe@example.com");
            var cabin1 = allAccommodationTypes.FirstOrDefault(a => a.CampsiteId?.Value == 1 && a.Type == "Cabin");
            if (guest1 != null && cabin1 != null)
            {
                var booking1 = Booking.Create(
                    GuestId.Create(guest1.GuestId),
                    CampsiteId.Create(1),
                    cabin1.Id,
                    DateRange.Create(DateTime.Now.AddDays(30), DateTime.Now.AddDays(37)),
                    Money.Create(150m, "DKK"),
                    2,
                    1,
                    "Please provide extra towels"
                );
                booking1.GetType().GetMethod("Confirm")?.Invoke(booking1, null);
                await context.Bookings.AddAsync(booking1);
                await context.SaveChangesAsync();
                context.Entry(booking1).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            // Booking 2: Jane Smith - Skagen North Point - Tent Site - Pending
            var guest2 = allGuests.FirstOrDefault(g => g.Email?.Value == "jane.smith@example.com");
            var tent2 = allAccommodationTypes.FirstOrDefault(a => a.CampsiteId?.Value == 2 && a.Type == "Tent Site");
            if (guest2 != null && tent2 != null)
            {
                var booking2 = Booking.Create(
                    GuestId.Create(guest2.GuestId),
                    CampsiteId.Create(2),
                    tent2.Id,
                    DateRange.Create(DateTime.Now.AddDays(15), DateTime.Now.AddDays(18)),
                    Money.Create(55m, "DKK"),
                    2,
                    0,
                    ""
                );
                await context.Bookings.AddAsync(booking2);
                await context.SaveChangesAsync();
                context.Entry(booking2).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            // Booking 3: Mike Johnson - Aarhus Forest Retreat - Cabin - Confirmed
            var guest3 = allGuests.FirstOrDefault(g => g.Email?.Value == "mike.johnson@example.com");
            var cabin3 = allAccommodationTypes.FirstOrDefault(a => a.CampsiteId?.Value == 3 && a.Type == "Cabin");
            if (guest3 != null && cabin3 != null)
            {
                var booking3 = Booking.Create(
                    GuestId.Create(guest3.GuestId),
                    CampsiteId.Create(3),
                    cabin3.Id,
                    DateRange.Create(DateTime.Now.AddDays(45), DateTime.Now.AddDays(52)),
                    Money.Create(120m, "DKK"),
                    2,
                    2,
                    "Arriving late, please keep key at reception"
                );
                booking3.GetType().GetMethod("Confirm")?.Invoke(booking3, null);
                await context.Bookings.AddAsync(booking3);
                await context.SaveChangesAsync();
                context.Entry(booking3).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }

            // Booking 4: John Doe - Copenhagen Beach Camp - Glamping - Completed (past booking)
            if (guest1 != null)
            {
                var glamping1 = allAccommodationTypes.FirstOrDefault(a => a.CampsiteId?.Value == 1 && a.Type == "Glamping");
                if (glamping1 != null)
                {
                    var booking4 = Booking.Create(
                        GuestId.Create(guest1.GuestId),
                        CampsiteId.Create(1),
                        glamping1.Id,
                        DateRange.Create(DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-15)),
                        Money.Create(220m, "DKK"),
                        2,
                        0,
                        ""
                    );
                    booking4.GetType().GetMethod("Confirm")?.Invoke(booking4, null);
                    booking4.GetType().GetMethod("Complete")?.Invoke(booking4, null);
                    await context.Bookings.AddAsync(booking4);
                    await context.SaveChangesAsync();
                    context.Entry(booking4).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
            }

            // Booking 5: Jane Smith - Bornholm Island Camp - Cabin - Cancelled
            if (guest2 != null)
            {
                var cabin5 = allAccommodationTypes.FirstOrDefault(a => a.CampsiteId?.Value == 5 && a.Type == "Cabin");
                if (cabin5 != null)
                {
                    var booking5 = Booking.Create(
                        GuestId.Create(guest2.GuestId),
                        CampsiteId.Create(5),
                        cabin5.Id,
                        DateRange.Create(DateTime.Now.AddDays(60), DateTime.Now.AddDays(67)),
                        Money.Create(140m, "DKK"),
                        2,
                        1,
                        ""
                    );
                    booking5.GetType().GetMethod("Cancel")?.Invoke(booking5, new object[] { "Changed travel plans" });
                    await context.Bookings.AddAsync(booking5);
                    await context.SaveChangesAsync();
                    context.Entry(booking5).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }
            }
    }
}

