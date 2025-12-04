using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Data;

/// <summary>
/// EF Core value converters for Value Objects
/// </summary>

// Email Converter
public class EmailConverter : ValueConverter<Email, string>
{
    public EmailConverter()
        : base(
            v => v.Value,
            v => Email.Create(v))
    {
    }
}

// Money Converter
public class MoneyConverter : ValueConverter<Money, string>
{
    public MoneyConverter()
        : base(
            v => $"{v.Amount}|{v.Currency}",
            v => ConvertToMoney(v))
    {
    }

    private static Money ConvertToMoney(string value)
    {
        var parts = value.Split('|');
        return Money.Create(decimal.Parse(parts[0]), parts[1]);
    }
}

// DateRange Converter
public class DateRangeConverter : ValueConverter<DateRange, string>
{
    public DateRangeConverter()
        : base(
            v => $"{v.StartDate:O}|{v.EndDate:O}",
            v => ConvertToDateRange(v))
    {
    }

    private static DateRange ConvertToDateRange(string value)
    {
        var parts = value.Split('|');
        return DateRange.Create(DateTime.Parse(parts[0]), DateTime.Parse(parts[1]));
    }
}

// BookingStatus Converter
public class BookingStatusConverter : ValueConverter<BookingStatus, string>
{
    public BookingStatusConverter()
        : base(
            v => v.Value,
            v => BookingStatus.FromString(v))
    {
    }
}

// PaymentStatus Converter
public class PaymentStatusConverter : ValueConverter<PaymentStatus, string>
{
    public PaymentStatusConverter()
        : base(
            v => v.Value,
            v => PaymentStatus.FromString(v))
    {
    }
}

// Strongly-Typed ID Converters
public class BookingIdConverter : ValueConverter<BookingId, int>
{
    public BookingIdConverter() : base(v => v.Value, v => BookingId.Create(v)) { }
}

public class PaymentIdConverter : ValueConverter<PaymentId, int>
{
    public PaymentIdConverter() : base(v => v.Value, v => PaymentId.Create(v)) { }
}

public class UserIdConverter : ValueConverter<UserId, int>
{
    public UserIdConverter() : base(v => v.Value, v => UserId.Create(v)) { }
}

public class GuestIdConverter : ValueConverter<GuestId, int>
{
    public GuestIdConverter() : base(v => v.Value, v => GuestId.Create(v)) { }
}

public class AdminIdConverter : ValueConverter<AdminId, int>
{
    public AdminIdConverter() : base(v => v.Value, v => AdminId.Create(v)) { }
}

public class StaffIdConverter : ValueConverter<StaffId, int>
{
    public StaffIdConverter() : base(v => v.Value, v => StaffId.Create(v)) { }
}

public class CampsiteIdConverter : ValueConverter<CampsiteId, int>
{
    public CampsiteIdConverter() : base(
        v => v.Value,
        v => v == 0 ? CampsiteId.CreateNew() : CampsiteId.Create(v)) { }
}

public class AccommodationTypeIdConverter : ValueConverter<AccommodationTypeId, int>
{
    public AccommodationTypeIdConverter() : base(v => v.Value, v => AccommodationTypeId.Create(v)) { }
}

public class AccommodationSpotIdConverter : ValueConverter<AccommodationSpotId, int>
{
    public AccommodationSpotIdConverter() : base(v => v.Value, v => AccommodationSpotId.Create(v)) { }
}

public class AvailabilityIdConverter : ValueConverter<AvailabilityId, int>
{
    public AvailabilityIdConverter() : base(v => v.Value, v => AvailabilityId.Create(v)) { }
}

public class SeasonalPricingIdConverter : ValueConverter<SeasonalPricingId, int>
{
    public SeasonalPricingIdConverter() : base(v => v.Value, v => SeasonalPricingId.Create(v)) { }
}

public class DiscountIdConverter : ValueConverter<DiscountId, int>
{
    public DiscountIdConverter() : base(v => v.Value, v => DiscountId.Create(v)) { }
}

public class PeripheralPurchaseIdConverter : ValueConverter<PeripheralPurchaseId, int>
{
    public PeripheralPurchaseIdConverter() : base(v => v.Value, v => PeripheralPurchaseId.Create(v)) { }
}

public class PurchaseOptionIdConverter : ValueConverter<PurchaseOptionId, int>
{
    public PurchaseOptionIdConverter() : base(v => v.Value, v => PurchaseOptionId.Create(v)) { }
}

public class AmenityIdConverter : ValueConverter<AmenityId, int>
{
    public AmenityIdConverter() : base(v => v.Value, v => AmenityId.Create(v)) { }
}

public class AmenityLookupIdConverter : ValueConverter<AmenityLookupId, int>
{
    public AmenityLookupIdConverter() : base(v => v.Value, v => AmenityLookupId.Create(v)) { }
}

public class PhotoIdConverter : ValueConverter<PhotoId, int>
{
    public PhotoIdConverter() : base(
        v => v.Value,
        v => v == 0 ? PhotoId.CreateNew() : PhotoId.Create(v)) { }
}

public class ReviewIdConverter : ValueConverter<ReviewId, int>
{
    public ReviewIdConverter() : base(v => v.Value, v => ReviewId.Create(v)) { }
}

public class EventIdConverter : ValueConverter<CampsiteBooking.Models.ValueObjects.EventId, int>
{
    public EventIdConverter() : base(v => v.Value, v => CampsiteBooking.Models.ValueObjects.EventId.Create(v)) { }
}

public class EventRegistrationIdConverter : ValueConverter<EventRegistrationId, int>
{
    public EventRegistrationIdConverter() : base(v => v.Value, v => EventRegistrationId.Create(v)) { }
}

public class NewsletterIdConverter : ValueConverter<NewsletterId, int>
{
    public NewsletterIdConverter() : base(v => v.Value, v => NewsletterId.Create(v)) { }
}

public class NewsletterSubscriptionIdConverter : ValueConverter<NewsletterSubscriptionId, int>
{
    public NewsletterSubscriptionIdConverter() : base(v => v.Value, v => NewsletterSubscriptionId.Create(v)) { }
}

public class NewsletterAnalyticsIdConverter : ValueConverter<NewsletterAnalyticsId, int>
{
    public NewsletterAnalyticsIdConverter() : base(v => v.Value, v => NewsletterAnalyticsId.Create(v)) { }
}

public class MaintenanceTaskIdConverter : ValueConverter<MaintenanceTaskId, int>
{
    public MaintenanceTaskIdConverter() : base(v => v.Value, v => MaintenanceTaskId.Create(v)) { }
}
