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

        // Add RowVersion for Optimistic Concurrency on Booking
        modelBuilder.Entity<Booking>()
            .Property<byte[]>("RowVersion")
            .IsRowVersion();
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

