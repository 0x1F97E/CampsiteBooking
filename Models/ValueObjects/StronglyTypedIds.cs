namespace CampsiteBooking.Models.ValueObjects;

/// <summary>
/// Base class for strongly-typed IDs.
/// Prevents primitive obsession and provides type safety.
/// </summary>
public abstract class StronglyTypedId : ValueObject
{
    public int Value { get; }

    protected StronglyTypedId(int value, bool allowZero = false)
    {
        if (!allowZero && value <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(value));

        if (allowZero && value < 0)
            throw new ArgumentException("ID cannot be negative", nameof(value));

        Value = value;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator int(StronglyTypedId id) => id.Value;
}

// ============================================================================
// STRONGLY-TYPED IDs FOR ALL ENTITIES
// ============================================================================

public sealed class UserId : StronglyTypedId
{
    private UserId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static UserId Create(int value) => new(value);
    public static UserId CreateNew() => new(0, allowZero: true); // For new entities before persistence
}

public sealed class GuestId : StronglyTypedId
{
    private GuestId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static GuestId Create(int value) => new(value);
    public static GuestId CreateNew() => new(0, allowZero: true);
}

public sealed class AdminId : StronglyTypedId
{
    private AdminId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static AdminId Create(int value) => new(value);
    public static AdminId CreateNew() => new(0, allowZero: true);
}

public sealed class StaffId : StronglyTypedId
{
    private StaffId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static StaffId Create(int value) => new(value);
    public static StaffId CreateNew() => new(0, allowZero: true);
}

public sealed class CampsiteId : StronglyTypedId
{
    private CampsiteId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static CampsiteId Create(int value) => new(value);
    public static CampsiteId CreateNew() => new(0, allowZero: true);
}

public sealed class AccommodationTypeId : StronglyTypedId
{
    private AccommodationTypeId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static AccommodationTypeId Create(int value) => new(value);
    public static AccommodationTypeId CreateNew() => new(0, allowZero: true);
}

public sealed class AccommodationSpotId : StronglyTypedId
{
    private AccommodationSpotId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static AccommodationSpotId Create(int value) => new(value);
    public static AccommodationSpotId CreateNew() => new(0, allowZero: true);
}

public sealed class BookingId : StronglyTypedId
{
    private BookingId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static BookingId Create(int value) => new(value);
    public static BookingId CreateNew() => new(0, allowZero: true);
}

public sealed class PaymentId : StronglyTypedId
{
    private PaymentId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static PaymentId Create(int value) => new(value);
    public static PaymentId CreateNew() => new(0, allowZero: true);
}

public sealed class AvailabilityId : StronglyTypedId
{
    private AvailabilityId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static AvailabilityId Create(int value) => new(value);
    public static AvailabilityId CreateNew() => new(0, allowZero: true);
}

public sealed class SeasonalPricingId : StronglyTypedId
{
    private SeasonalPricingId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static SeasonalPricingId Create(int value) => new(value);
    public static SeasonalPricingId CreateNew() => new(0, allowZero: true);
}

public sealed class DiscountId : StronglyTypedId
{
    private DiscountId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static DiscountId Create(int value) => new(value);
    public static DiscountId CreateNew() => new(0, allowZero: true);
}

public sealed class PeripheralPurchaseId : StronglyTypedId
{
    private PeripheralPurchaseId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static PeripheralPurchaseId Create(int value) => new(value);
    public static PeripheralPurchaseId CreateNew() => new(0, allowZero: true);
}

public sealed class AmenityId : StronglyTypedId
{
    private AmenityId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static AmenityId Create(int value) => new(value);
    public static AmenityId CreateNew() => new(0, allowZero: true);
}

public sealed class PhotoId : StronglyTypedId
{
    private PhotoId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static PhotoId Create(int value) => new(value);
    public static PhotoId CreateNew() => new(0, allowZero: true);
}

public sealed class ReviewId : StronglyTypedId
{
    private ReviewId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static ReviewId Create(int value) => new(value);
    public static ReviewId CreateNew() => new(0, allowZero: true);
}

public sealed class EventId : StronglyTypedId
{
    private EventId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static EventId Create(int value) => new(value);
    public static EventId CreateNew() => new(0, allowZero: true);
}


public sealed class NewsletterId : StronglyTypedId
{
    private NewsletterId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static NewsletterId Create(int value) => new(value);
    public static NewsletterId CreateNew() => new(0, allowZero: true);
}

public sealed class NewsletterSubscriptionId : StronglyTypedId
{
    private NewsletterSubscriptionId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static NewsletterSubscriptionId Create(int value) => new(value);
    public static NewsletterSubscriptionId CreateNew() => new(0, allowZero: true);
}

public sealed class NewsletterAnalyticsId : StronglyTypedId
{
    private NewsletterAnalyticsId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static NewsletterAnalyticsId Create(int value) => new(value);
    public static NewsletterAnalyticsId CreateNew() => new(0, allowZero: true);
}

public sealed class MaintenanceTaskId : StronglyTypedId
{
    private MaintenanceTaskId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static MaintenanceTaskId Create(int value) => new(value);
    public static MaintenanceTaskId CreateNew() => new(0, allowZero: true);
}

public sealed class EventRegistrationId : StronglyTypedId
{
    private EventRegistrationId(int value, bool allowZero = false) : base(value, allowZero) { }
    public static EventRegistrationId Create(int value) => new(value);
    public static EventRegistrationId CreateNew() => new(0, allowZero: true);
}

