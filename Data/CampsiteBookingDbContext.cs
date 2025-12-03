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
    public DbSet<PurchaseOption> PurchaseOptions => Set<PurchaseOption>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<AmenityLookup> AmenityLookups => Set<AmenityLookup>();
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

            entity.Property("_passwordHash").HasColumnName("PasswordHash").HasMaxLength(500);
            entity.Property("_firstName").HasColumnName("FirstName").HasMaxLength(100).IsRequired();
            entity.Property("_lastName").HasColumnName("LastName").HasMaxLength(100).IsRequired();
            entity.Property("_phone").HasColumnName("Phone").HasMaxLength(20);
            entity.Property("_country").HasColumnName("Country").HasMaxLength(100);
            entity.Property("_joinedDate").HasColumnName("JoinedDate");
            entity.Property("_lastLogin").HasColumnName("LastLogin");
            entity.Property("_isActive").HasColumnName("IsActive");
        });

        // Configure Booking entity to map private fields
        modelBuilder.Entity<Booking>(entity =>
        {
            // Configure the Id property to use the Id column with value converter
            entity.Property(b => b.Id).HasColumnName("Id")
                .HasConversion(new BookingIdConverter());

            // Ignore the legacy BookingId property - we use Id instead
            entity.Ignore(b => b.BookingId);

            // Ignore the public properties that are backed by private fields
            // We'll map the private fields directly to avoid conflicts
            entity.Ignore(b => b.GuestId);
            entity.Ignore(b => b.CampsiteId);
            entity.Ignore(b => b.AccommodationSpotId);
            entity.Ignore(b => b.AccommodationTypeId);
            entity.Ignore(b => b.Period);
            entity.Ignore(b => b.Status);
            entity.Ignore(b => b.BasePrice);
            entity.Ignore(b => b.TotalPrice);
            entity.Ignore(b => b.NumberOfAdults);
            entity.Ignore(b => b.NumberOfChildren);
            entity.Ignore(b => b.SpecialRequests);
            entity.Ignore(b => b.CreatedDate);
            entity.Ignore(b => b.LastModifiedDate);
            entity.Ignore(b => b.CancellationDate);

            // Ignore legacy properties
            entity.Ignore(b => b.UserId);
            entity.Ignore(b => b.CheckInDate);
            entity.Ignore(b => b.CheckOutDate);
            entity.Ignore(b => b.SpotId);

            // Map the private fields to database columns using value converters
            entity.Property("_guestId").HasColumnName("GuestId")
                .HasConversion(new GuestIdConverter());
            entity.Property("_campsiteId").HasColumnName("CampsiteId")
                .HasConversion(new CampsiteIdConverter());
            entity.Property("_accommodationSpotId").HasColumnName("AccommodationSpotId")
                .HasConversion(new AccommodationSpotIdConverter());
            entity.Property("_accommodationTypeId").HasColumnName("AccommodationTypeId")
                .HasConversion(new AccommodationTypeIdConverter());
            entity.Property("_period").HasColumnName("Period")
                .HasConversion(new DateRangeConverter());
            entity.Property("_status").HasColumnName("Status")
                .HasConversion(new BookingStatusConverter());
            entity.Property("_basePrice").HasColumnName("BasePrice");
            entity.Property("_totalPrice").HasColumnName("TotalPrice");
            entity.Property("_numberOfAdults").HasColumnName("NumberOfAdults");
            entity.Property("_numberOfChildren").HasColumnName("NumberOfChildren");
            entity.Property("_specialRequests").HasColumnName("SpecialRequests");
            entity.Property("_createdDate").HasColumnName("CreatedDate");
            entity.Property("_lastModifiedDate").HasColumnName("LastModifiedDate");
            entity.Property("_cancellationDate").HasColumnName("CancellationDate");

            // Ignore the RowVersion property for now - MySQL timestamp handling is different
            // entity.Property<byte[]>("RowVersion").IsRowVersion();
        });

        // Configure Payment entity to map private fields
        modelBuilder.Entity<Payment>(entity =>
        {
            // Configure the Id property to use the Id column with value converter
            entity.Property(p => p.Id).HasColumnName("Id")
                .HasConversion(new PaymentIdConverter());

            // Ignore the legacy PaymentId property - we use Id instead
            entity.Ignore(p => p.PaymentId);

            // Ignore the public properties that are backed by private fields
            entity.Ignore(p => p.BookingId);
            entity.Ignore(p => p.Amount);
            entity.Ignore(p => p.Method);
            entity.Ignore(p => p.TransactionId);
            entity.Ignore(p => p.Status);
            entity.Ignore(p => p.PaymentDate);
            entity.Ignore(p => p.RefundDate);
            entity.Ignore(p => p.RefundAmount);
            entity.Ignore(p => p.CreatedDate);

            // Ignore legacy properties
            entity.Ignore(p => p.Currency);

            // Map the private fields to database columns
            entity.Property("_bookingId").HasColumnName("BookingId")
                .HasConversion(new BookingIdConverter());
            entity.Property("_amount").HasColumnName("Amount");
            entity.Property("_method").HasColumnName("Method").HasMaxLength(50);
            entity.Property("_transactionId").HasColumnName("TransactionId").HasMaxLength(100);
            entity.Property("_status").HasColumnName("Status")
                .HasConversion(new PaymentStatusConverter());
            entity.Property("_paymentDate").HasColumnName("PaymentDate");
            entity.Property("_refundDate").HasColumnName("RefundDate");
            entity.Property("_refundAmount").HasColumnName("RefundAmount");
            entity.Property("_createdDate").HasColumnName("CreatedDate");
        });

        // Configure Campsite entity to map private fields
        modelBuilder.Entity<Campsite>(entity =>
        {
            // Configure the Id property to use the Id column with value converter
            entity.Property(c => c.Id).HasColumnName("Id")
                .HasConversion(new CampsiteIdConverter());

            // Ignore the legacy CampsiteId property - we use Id instead
            entity.Ignore(c => c.CampsiteId);

            // Ignore public properties that are backed by private fields
            entity.Ignore(c => c.Name);
            entity.Ignore(c => c.StreetAddress);
            entity.Ignore(c => c.City);
            entity.Ignore(c => c.PostalCode);
            entity.Ignore(c => c.Description);
            entity.Ignore(c => c.Latitude);
            entity.Ignore(c => c.Longitude);
            entity.Ignore(c => c.Attractiveness);
            entity.Ignore(c => c.PhoneNumber);
            entity.Ignore(c => c.Email);
            entity.Ignore(c => c.WebsiteUrl);
            entity.Ignore(c => c.EstablishedYear);
            entity.Ignore(c => c.IsActive);
            entity.Ignore(c => c.SeasonOpeningDate);
            entity.Ignore(c => c.CreatedDate);
            entity.Ignore(c => c.UpdatedDate);

            // Map private fields to database columns
            entity.Property("_name").HasColumnName("Name").IsRequired();
            entity.Property("_streetAddress").HasColumnName("StreetAddress").IsRequired();
            entity.Property("_city").HasColumnName("City").IsRequired();
            entity.Property("_postalCode").HasColumnName("PostalCode").IsRequired();
            entity.Property("_description").HasColumnName("Description");
            entity.Property("_latitude").HasColumnName("Latitude");
            entity.Property("_longitude").HasColumnName("Longitude");
            entity.Property("_attractiveness").HasColumnName("Attractiveness");
            entity.Property("_phoneNumber").HasColumnName("PhoneNumber");
            entity.Property("_email").HasColumnName("Email");
            entity.Property("_websiteUrl").HasColumnName("WebsiteUrl");
            entity.Property("_establishedYear").HasColumnName("EstablishedYear");
            entity.Property("_isActive").HasColumnName("IsActive");
            entity.Property("_seasonOpeningDate").HasColumnName("SeasonOpeningDate");
            entity.Property("_createdDate").HasColumnName("CreatedDate");
            entity.Property("_updatedDate").HasColumnName("UpdatedDate");
        });

        // Configure AccommodationType entity to map private fields
        modelBuilder.Entity<AccommodationType>(entity =>
        {
            // Configure the Id property to use the AccommodationTypeId column
            // The AccommodationTypeId property is a legacy property that provides int access to the Id
            entity.Property(a => a.Id).HasColumnName("Id")
                .HasConversion(new AccommodationTypeIdConverter());

            // Ignore the legacy AccommodationTypeId property - we use Id instead
            entity.Ignore(a => a.AccommodationTypeId);

            // Ignore public properties that are backed by private fields
            entity.Ignore(a => a.CampsiteId);
            entity.Ignore(a => a.Type);
            entity.Ignore(a => a.Description);
            entity.Ignore(a => a.MaxCapacity);
            entity.Ignore(a => a.BasePrice);
            entity.Ignore(a => a.ImageUrl);
            entity.Ignore(a => a.AvailableUnits);
            entity.Ignore(a => a.IsActive);
            entity.Ignore(a => a.CreatedDate);
            entity.Ignore(a => a.BasePrice_Legacy);
            entity.Ignore(a => a.Amenities);
            entity.Ignore(a => a.UnitNamingPrefix);
            entity.Ignore(a => a.UnitNamingPattern);
            entity.Ignore(a => a.AreaSquareMeters);

            // Map private fields to database columns
            entity.Property("_campsiteId").HasColumnName("CampsiteId")
                .HasConversion(new CampsiteIdConverter());
            entity.Property("_type").HasColumnName("Type").HasMaxLength(50).IsRequired();
            entity.Property("_description").HasColumnName("Description");
            entity.Property("_maxCapacity").HasColumnName("MaxCapacity");
            entity.Property("_basePrice").HasColumnName("BasePrice");
            entity.Property("_imageUrl").HasColumnName("ImageUrl");
            entity.Property("_availableUnits").HasColumnName("AvailableUnits");
            entity.Property("_isActive").HasColumnName("IsActive");
            entity.Property("_createdDate").HasColumnName("CreatedDate");
            entity.Property("_amenities").HasColumnName("Amenities");
            entity.Property("_unitNamingPrefix").HasColumnName("UnitNamingPrefix").HasMaxLength(50);
            entity.Property("_unitNamingPattern").HasColumnName("UnitNamingPattern").HasMaxLength(50);
            entity.Property("_areaSquareMeters").HasColumnName("AreaSquareMeters").HasColumnType("decimal(10, 2)");
        });

        // Configure AccommodationSpot entity to map private fields
        modelBuilder.Entity<AccommodationSpot>(entity =>
        {
            // Configure the Id property to use the Id column with value converter
            entity.Property(a => a.Id).HasColumnName("Id")
                .HasConversion(new AccommodationSpotIdConverter());

            // Ignore public properties that are backed by private fields
            entity.Ignore(a => a.SpotIdentifier);
            entity.Ignore(a => a.CampsiteId);
            entity.Ignore(a => a.CampsiteName);
            entity.Ignore(a => a.AccommodationTypeId);
            entity.Ignore(a => a.Latitude);
            entity.Ignore(a => a.Longitude);
            entity.Ignore(a => a.Type);
            entity.Ignore(a => a.Status);
            entity.Ignore(a => a.IsUnderMaintenance);
            entity.Ignore(a => a.PriceModifier);
            entity.Ignore(a => a.CreatedDate);
            entity.Ignore(a => a.SpotId);
            entity.Ignore(a => a.Status_Legacy);

            // Map private fields to database columns
            entity.Property("_spotIdentifier").HasColumnName("SpotIdentifier").HasMaxLength(50).IsRequired();
            entity.Property("_campsiteId").HasColumnName("CampsiteId")
                .HasConversion(new CampsiteIdConverter());
            entity.Property("_campsiteName").HasColumnName("CampsiteName").HasMaxLength(200);
            entity.Property("_accommodationTypeId").HasColumnName("AccommodationTypeId")
                .HasConversion(new AccommodationTypeIdConverter());
            entity.Property("_latitude").HasColumnName("Latitude");
            entity.Property("_longitude").HasColumnName("Longitude");
            entity.Property("_type").HasColumnName("Type").HasMaxLength(50).IsRequired();
            entity.Property("_status").HasColumnName("Status")
                .HasConversion<string>();
            entity.Property("_isUnderMaintenance").HasColumnName("IsUnderMaintenance")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
            entity.Property("_priceModifier").HasColumnName("PriceModifier");
            entity.Property("_createdDate").HasColumnName("CreatedDate");
        });

        // Configure Amenity entity
        modelBuilder.Entity<Amenity>(entity =>
        {
            // Configure the Id property to use the Id column with value converter
            entity.Property(a => a.Id).HasColumnName("Id")
                .HasConversion(new AmenityIdConverter());

            // Map the AmenityId property to the AmenityId column
            entity.Property(a => a.AmenityId).HasColumnName("AmenityId");

            // Map properties to database columns
            entity.Property(a => a.CampsiteId).HasColumnName("CampsiteId")
                .HasConversion(new CampsiteIdConverter());

            entity.Property(a => a.Name).HasColumnName("Name")
                .HasMaxLength(100).IsRequired();

            entity.Property(a => a.Description).HasColumnName("Description");

            entity.Property(a => a.IconUrl).HasColumnName("IconUrl");

            entity.Property(a => a.Category).HasColumnName("Category")
                .HasMaxLength(50);

            entity.Property(a => a.IsAvailable).HasColumnName("IsAvailable");

            entity.Property(a => a.CreatedDate).HasColumnName("CreatedDate");

            entity.Property(a => a.UpdatedDate).HasColumnName("UpdatedDate");
        });

        // Configure AmenityLookup entity
        modelBuilder.Entity<AmenityLookup>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id).HasColumnName("Id")
                .HasConversion(new AmenityLookupIdConverter());

            entity.Property(a => a.AmenityLookupId).HasColumnName("AmenityLookupId");

            entity.Property(a => a.Name).HasColumnName("Name")
                .HasMaxLength(100).IsRequired();

            entity.Property(a => a.DisplayName).HasColumnName("DisplayName")
                .HasMaxLength(100).IsRequired();

            entity.Property(a => a.Category).HasColumnName("Category")
                .HasMaxLength(50);

            entity.Property(a => a.IconClass).HasColumnName("IconClass")
                .HasMaxLength(100);

            entity.Property(a => a.IsActive).HasColumnName("IsActive");

            entity.Property(a => a.SortOrder).HasColumnName("SortOrder");

            entity.Property(a => a.CreatedDate).HasColumnName("CreatedDate");

            entity.Property(a => a.UpdatedDate).HasColumnName("UpdatedDate");

            // Create unique index on Name
            entity.HasIndex(a => a.Name).IsUnique();
        });

        // Configure PurchaseOption entity
        modelBuilder.Entity<PurchaseOption>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id).HasColumnName("Id")
                .HasConversion(new PurchaseOptionIdConverter());

            entity.Ignore(p => p.PurchaseOptionId);

            entity.Property(p => p.CampsiteId).HasColumnName("CampsiteId")
                .HasConversion(new CampsiteIdConverter())
                .IsRequired();

            entity.Property(p => p.AccommodationTypeId).HasColumnName("AccommodationTypeId")
                .HasConversion(new AccommodationTypeIdConverter());

            entity.Property(p => p.Name).HasColumnName("Name")
                .HasMaxLength(100).IsRequired();

            entity.Property(p => p.Description).HasColumnName("Description")
                .HasMaxLength(500);

            entity.Property(p => p.Price).HasColumnName("Price")
                .HasConversion(new MoneyConverter());

            entity.Property(p => p.Category).HasColumnName("Category")
                .HasMaxLength(50);

            entity.Property(p => p.IsActive).HasColumnName("IsActive");

            entity.Property(p => p.CreatedDate).HasColumnName("CreatedDate");

            entity.Property(p => p.UpdatedDate).HasColumnName("UpdatedDate");
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

        // Configure SeasonalPricing entity to map private fields
        modelBuilder.Entity<SeasonalPricing>(entity =>
        {
            // Configure the Id property to use the Id column with value converter
            entity.Property(sp => sp.Id).HasColumnName("Id")
                .HasConversion(new SeasonalPricingIdConverter());

            // Ignore the SeasonalPricingId property - it's a computed property
            entity.Ignore(sp => sp.SeasonalPricingId);

            // Ignore public properties that are backed by private fields
            entity.Ignore(sp => sp.CampsiteId);
            entity.Ignore(sp => sp.AccommodationTypeId);
            entity.Ignore(sp => sp.SeasonName);
            entity.Ignore(sp => sp.StartDate);
            entity.Ignore(sp => sp.EndDate);
            entity.Ignore(sp => sp.PriceMultiplier);
            entity.Ignore(sp => sp.IsActive);
            entity.Ignore(sp => sp.CreatedDate);
            entity.Ignore(sp => sp.UpdatedDate);

            // Map private fields to database columns
            entity.Property("_campsiteId").HasColumnName("CampsiteId")
                .HasConversion(new CampsiteIdConverter());
            entity.Property("_accommodationTypeId").HasColumnName("AccommodationTypeId")
                .HasConversion(new AccommodationTypeIdConverter());
            entity.Property("_seasonName").HasColumnName("SeasonName").HasMaxLength(100).IsRequired();
            entity.Property("_startDate").HasColumnName("StartDate");
            entity.Property("_endDate").HasColumnName("EndDate");
            entity.Property("_priceMultiplier").HasColumnName("PriceMultiplier").HasColumnType("decimal(5, 2)");
            entity.Property("_isActive").HasColumnName("IsActive");
            entity.Property("_createdDate").HasColumnName("CreatedDate");
            entity.Property("_updatedDate").HasColumnName("UpdatedDate");
        });

        // Configure Event entity to map private fields
        modelBuilder.Entity<Event>(entity =>
        {
            // Configure the Id property to use the Id column with value converter
            entity.Property(e => e.Id).HasColumnName("Id")
                .HasConversion(new EventIdConverter());

            // Ignore the EventId property - it's a computed property
            entity.Ignore(e => e.EventId);

            // Ignore public properties that are backed by private fields
            entity.Ignore(e => e.CampsiteId);
            entity.Ignore(e => e.Title);
            entity.Ignore(e => e.Description);
            entity.Ignore(e => e.EventDate);
            entity.Ignore(e => e.MaxParticipants);
            entity.Ignore(e => e.CurrentParticipants);
            entity.Ignore(e => e.Price);
            entity.Ignore(e => e.IsActive);
            entity.Ignore(e => e.CreatedDate);

            // Map private fields to database columns
            entity.Property("_campsiteId").HasColumnName("CampsiteId")
                .HasConversion(new CampsiteIdConverter());
            entity.Property("_title").HasColumnName("Title").HasMaxLength(200).IsRequired();
            entity.Property("_description").HasColumnName("Description");
            entity.Property("_eventDate").HasColumnName("EventDate");
            entity.Property("_maxParticipants").HasColumnName("MaxParticipants");
            entity.Property("_currentParticipants").HasColumnName("CurrentParticipants");
            entity.Property("_price").HasColumnName("Price")
                .HasConversion(new MoneyConverter());
            entity.Property("_isActive").HasColumnName("IsActive");
            entity.Property("_createdDate").HasColumnName("CreatedDate");
        });

        // Configure Newsletter entity to map private fields
        modelBuilder.Entity<Newsletter>(entity =>
        {
            // Configure the Id property to use the Id column with value converter
            entity.Property(n => n.Id).HasColumnName("Id")
                .HasConversion(new NewsletterIdConverter());

            // Ignore the NewsletterId property - it's a computed property
            entity.Ignore(n => n.NewsletterId);

            // Ignore public properties that are backed by private fields
            entity.Ignore(n => n.Subject);
            entity.Ignore(n => n.Content);
            entity.Ignore(n => n.ScheduledDate);
            entity.Ignore(n => n.SentDate);
            entity.Ignore(n => n.Status);
            entity.Ignore(n => n.RecipientCount);
            entity.Ignore(n => n.CreatedDate);

            // Map private fields to database columns
            entity.Property("_subject").HasColumnName("Subject").HasMaxLength(200).IsRequired();
            entity.Property("_content").HasColumnName("Content").IsRequired();
            entity.Property("_scheduledDate").HasColumnName("ScheduledDate");
            entity.Property("_sentDate").HasColumnName("SentDate");
            entity.Property("_status").HasColumnName("Status").HasMaxLength(50).IsRequired();
            entity.Property("_recipientCount").HasColumnName("RecipientCount");
            entity.Property("_createdDate").HasColumnName("CreatedDate");
        });

        // Configure Review entity to map private fields
        modelBuilder.Entity<Review>(entity =>
        {
            // Configure the Id property to use the Id column with value converter
            entity.Property(r => r.Id).HasColumnName("Id")
                .HasConversion(new ReviewIdConverter());

            // Ignore the ReviewId property - it's a computed property
            entity.Ignore(r => r.ReviewId);

            // Ignore public properties that are backed by private fields
            entity.Ignore(r => r.CampsiteId);
            entity.Ignore(r => r.UserId);
            entity.Ignore(r => r.BookingId);
            entity.Ignore(r => r.Rating);
            entity.Ignore(r => r.Comment);
            entity.Ignore(r => r.ReviewerName);
            entity.Ignore(r => r.CreatedDate);
            entity.Ignore(r => r.UpdatedDate);
            entity.Ignore(r => r.IsApproved);
            entity.Ignore(r => r.IsVisible);
            entity.Ignore(r => r.AdminResponse);
            entity.Ignore(r => r.AdminResponseDate);

            // Map private fields to database columns
            entity.Property("_campsiteId").HasColumnName("CampsiteId")
                .HasConversion(new CampsiteIdConverter());
            entity.Property("_userId").HasColumnName("UserId")
                .HasConversion(new UserIdConverter());
            entity.Property("_bookingId").HasColumnName("BookingId")
                .HasConversion(new BookingIdConverter());
            entity.Property("_rating").HasColumnName("Rating");
            entity.Property("_comment").HasColumnName("Comment");
            entity.Property("_reviewerName").HasColumnName("ReviewerName").HasMaxLength(100).IsRequired();
            entity.Property("_createdDate").HasColumnName("CreatedDate");
            entity.Property("_updatedDate").HasColumnName("UpdatedDate");
            entity.Property("_isApproved").HasColumnName("IsApproved");
            entity.Property("_isVisible").HasColumnName("IsVisible");
            entity.Property("_adminResponse").HasColumnName("AdminResponse");
            entity.Property("_adminResponseDate").HasColumnName("AdminResponseDate");
        });

        // Configure Discount entity to map private fields
        modelBuilder.Entity<Discount>(entity =>
        {
            // Configure the Id property to use the Id column with value converter
            entity.Property(d => d.Id).HasColumnName("Id")
                .HasConversion(new DiscountIdConverter());

            // Ignore the DiscountId property - it's a computed property
            entity.Ignore(d => d.DiscountId);

            // Ignore public properties that are backed by private fields
            entity.Ignore(d => d.Code);
            entity.Ignore(d => d.Description);
            entity.Ignore(d => d.Type);
            entity.Ignore(d => d.Value);
            entity.Ignore(d => d.ValidFrom);
            entity.Ignore(d => d.ValidUntil);
            entity.Ignore(d => d.UsedCount);
            entity.Ignore(d => d.MaxUses);
            entity.Ignore(d => d.MinimumBookingAmount);
            entity.Ignore(d => d.ApplicableCampsites);
            entity.Ignore(d => d.ApplicableAccommodationTypes);
            entity.Ignore(d => d.IsActive);
            entity.Ignore(d => d.CreatedDate);

            // Map private fields to database columns
            entity.Property("_code").HasColumnName("Code").HasMaxLength(20).IsRequired();
            entity.Property("_description").HasColumnName("Description").HasMaxLength(500);
            entity.Property("_type").HasColumnName("Type").HasMaxLength(20).IsRequired();
            entity.Property("_value").HasColumnName("Value").HasColumnType("decimal(10, 2)");
            entity.Property("_validFrom").HasColumnName("ValidFrom");
            entity.Property("_validUntil").HasColumnName("ValidUntil");
            entity.Property("_usedCount").HasColumnName("UsedCount");
            entity.Property("_maxUses").HasColumnName("MaxUses");
            entity.Property("_minimumBookingAmount").HasColumnName("MinimumBookingAmount").HasColumnType("decimal(10, 2)");
            entity.Property("_isActive").HasColumnName("IsActive");
            entity.Property("_createdDate").HasColumnName("CreatedDate");

            // Create unique index on Code
            entity.HasIndex("_code").IsUnique();
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
        configurationBuilder.Properties<PurchaseOptionId>().HaveConversion<PurchaseOptionIdConverter>();
        configurationBuilder.Properties<AmenityId>().HaveConversion<AmenityIdConverter>();
        configurationBuilder.Properties<AmenityLookupId>().HaveConversion<AmenityLookupIdConverter>();
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

