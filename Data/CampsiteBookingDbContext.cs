using Microsoft.EntityFrameworkCore;
using CampsiteBooking.Models;
using CampsiteBooking.Models.ValueObjects;
using CampsiteBooking.Models.Common;

namespace CampsiteBooking.Data;

/// <summary>
/// Entity Framework DbContext for Campsite Booking system.
/// Implements domain-centric architecture with proper entity configurations.
/// </summary>
public class CampsiteBookingDbContext : DbContext
{
    public CampsiteBookingDbContext(DbContextOptions<CampsiteBookingDbContext> options)
        : base(options)
    {
    }

    // Aggregate Roots
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Campsite> Campsites => Set<Campsite>();

    // Entities (accessed through aggregate roots)
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Staff> StaffMembers => Set<Staff>();
    public DbSet<AccommodationType> AccommodationTypes => Set<AccommodationType>();
    public DbSet<AccommodationSpot> AccommodationSpots => Set<AccommodationSpot>();
    public DbSet<Availability> Availabilities => Set<Availability>();
    public DbSet<SeasonalPricing> SeasonalPricings => Set<SeasonalPricing>();
    public DbSet<Discount> Discounts => Set<Discount>();
    public DbSet<PeripheralPurchase> PeripheralPurchases => Set<PeripheralPurchase>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
    public DbSet<Newsletter> Newsletters => Set<Newsletter>();
    public DbSet<NewsletterSubscription> NewsletterSubscriptions => Set<NewsletterSubscription>();
    public DbSet<NewsletterAnalytics> NewsletterAnalytics => Set<NewsletterAnalytics>();
    public DbSet<MaintenanceTask> MaintenanceTasks => Set<MaintenanceTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Use EF Conventions - minimal configuration for clean architecture
        // Only configure what's absolutely necessary

        // Configure TPH (Table Per Hierarchy) for User inheritance
        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<User>("User")
            .HasValue<Guest>("Guest")
            .HasValue<Admin>("Admin")
            .HasValue<Staff>("Staff");

        // Configure User - UserId is computed from Id
        modelBuilder.Entity<User>()
            .Ignore(u => u.UserId); // UserId is a computed property, not stored in DB

        // Configure User entity to map private fields
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property("_email")
                .HasColumnName("Email")
                .HasConversion(new EmailConverter())
                .HasMaxLength(255)
                .IsRequired();

            entity.Property("_firstName").HasColumnName("FirstName").HasMaxLength(100).IsRequired();
            entity.Property("_lastName").HasColumnName("LastName").HasMaxLength(100).IsRequired();
            entity.Property("_phone").HasColumnName("Phone").HasMaxLength(20);
            entity.Property("_country").HasColumnName("Country").HasMaxLength(100);
            entity.Property("_joinedDate").HasColumnName("JoinedDate");
            entity.Property("_lastLogin").HasColumnName("LastLogin");
            entity.Property("_isActive").HasColumnName("IsActive");
        });

        // Add RowVersion for Optimistic Concurrency on Booking
        modelBuilder.Entity<Booking>()
            .Property<byte[]>("RowVersion")
            .IsRowVersion();

        // Configure Campsite entity to map private fields
        modelBuilder.Entity<Campsite>(entity =>
        {
            entity.Property("_name").HasColumnName("Name").IsRequired();
            entity.Property("_region").HasColumnName("Region").IsRequired();
            entity.Property("_description").HasColumnName("Description");
            entity.Property("_latitude").HasColumnName("Latitude");
            entity.Property("_longitude").HasColumnName("Longitude");
            entity.Property("_attractiveness").HasColumnName("Attractiveness");
            entity.Property("_phoneNumber").HasColumnName("PhoneNumber");
            entity.Property("_email").HasColumnName("Email");
            entity.Property("_websiteUrl").HasColumnName("WebsiteUrl");
            entity.Property("_establishedYear").HasColumnName("EstablishedYear");
            entity.Property("_isActive").HasColumnName("IsActive");
            entity.Property("_totalArea").HasColumnName("TotalArea");
            entity.Property("_createdDate").HasColumnName("CreatedDate");
            entity.Property("_updatedDate").HasColumnName("UpdatedDate");
        });

        // Configure Photo entity to map private fields
        modelBuilder.Entity<Photo>(entity =>
        {
            entity.Property("_campsiteId").HasColumnName("CampsiteId");
            entity.Property("_accommodationTypeId").HasColumnName("AccommodationTypeId");
            entity.Property("_url").HasColumnName("Url").IsRequired();
            entity.Property("_caption").HasColumnName("Caption");
            entity.Property("_altText").HasColumnName("AltText");
            entity.Property("_displayOrder").HasColumnName("DisplayOrder");
            entity.Property("_isPrimary").HasColumnName("IsPrimary");
            entity.Property("_uploadedDate").HasColumnName("UploadedDate");
            entity.Property("_updatedDate").HasColumnName("UpdatedDate");
            entity.Property("_category").HasColumnName("Category").HasMaxLength(50).HasDefaultValue("General");
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        // Configure Value Object conversions
        configurationBuilder
            .Properties<Email>()
            .HaveConversion<EmailConverter>();

        configurationBuilder
            .Properties<Money>()
            .HaveConversion<MoneyConverter>();

        configurationBuilder
            .Properties<DateRange>()
            .HaveConversion<DateRangeConverter>();

        configurationBuilder
            .Properties<BookingStatus>()
            .HaveConversion<BookingStatusConverter>();

        configurationBuilder
            .Properties<PaymentStatus>()
            .HaveConversion<PaymentStatusConverter>();

        // Configure Strongly-Typed IDs
        ConfigureStronglyTypedIds(configurationBuilder);
    }

    private void ConfigureStronglyTypedIds(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<BookingId>().HaveConversion<BookingIdConverter>();
        configurationBuilder.Properties<PaymentId>().HaveConversion<PaymentIdConverter>();
        configurationBuilder.Properties<UserId>().HaveConversion<UserIdConverter>();
        configurationBuilder.Properties<GuestId>().HaveConversion<GuestIdConverter>();
        configurationBuilder.Properties<AdminId>().HaveConversion<AdminIdConverter>();
        configurationBuilder.Properties<StaffId>().HaveConversion<StaffIdConverter>();
        configurationBuilder.Properties<CampsiteId>().HaveConversion<CampsiteIdConverter>();
        configurationBuilder.Properties<AccommodationTypeId>().HaveConversion<AccommodationTypeIdConverter>();
        configurationBuilder.Properties<AccommodationSpotId>().HaveConversion<AccommodationSpotIdConverter>();
        configurationBuilder.Properties<AvailabilityId>().HaveConversion<AvailabilityIdConverter>();
        configurationBuilder.Properties<SeasonalPricingId>().HaveConversion<SeasonalPricingIdConverter>();
        configurationBuilder.Properties<DiscountId>().HaveConversion<DiscountIdConverter>();
        configurationBuilder.Properties<PeripheralPurchaseId>().HaveConversion<PeripheralPurchaseIdConverter>();
        configurationBuilder.Properties<AmenityId>().HaveConversion<AmenityIdConverter>();
        configurationBuilder.Properties<PhotoId>().HaveConversion<PhotoIdConverter>();
        configurationBuilder.Properties<ReviewId>().HaveConversion<ReviewIdConverter>();
        configurationBuilder.Properties<CampsiteBooking.Models.ValueObjects.EventId>().HaveConversion<EventIdConverter>();
        configurationBuilder.Properties<EventRegistrationId>().HaveConversion<EventRegistrationIdConverter>();
        configurationBuilder.Properties<NewsletterId>().HaveConversion<NewsletterIdConverter>();
        configurationBuilder.Properties<NewsletterSubscriptionId>().HaveConversion<NewsletterSubscriptionIdConverter>();
        configurationBuilder.Properties<NewsletterAnalyticsId>().HaveConversion<NewsletterAnalyticsIdConverter>();
        configurationBuilder.Properties<MaintenanceTaskId>().HaveConversion<MaintenanceTaskIdConverter>();
    }

}

