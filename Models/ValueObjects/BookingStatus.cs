namespace CampsiteBooking.Models.ValueObjects;

/// <summary>
/// Value Object representing booking status with transition rules.
/// Ensures only valid status transitions are allowed.
/// </summary>
public class BookingStatus : ValueObject
{
    public string Value { get; }

    // Valid statuses
    public static readonly BookingStatus Pending = new BookingStatus("Pending");
    public static readonly BookingStatus Confirmed = new BookingStatus("Confirmed");
    public static readonly BookingStatus Completed = new BookingStatus("Completed");
    public static readonly BookingStatus Cancelled = new BookingStatus("Cancelled");

    private BookingStatus(string value)
    {
        Value = value;
    }

    public static BookingStatus FromString(string value)
    {
        return value switch
        {
            "Pending" => Pending,
            "Confirmed" => Confirmed,
            "Completed" => Completed,
            "Cancelled" => Cancelled,
            _ => throw new ArgumentException($"Invalid booking status: {value}", nameof(value))
        };
    }

    public bool CanTransitionTo(BookingStatus newStatus)
    {
        // Pending -> Confirmed, Cancelled
        if (this == Pending)
            return newStatus == Confirmed || newStatus == Cancelled;

        // Confirmed -> Completed, Cancelled
        if (this == Confirmed)
            return newStatus == Completed || newStatus == Cancelled;

        // Completed -> (no transitions allowed)
        if (this == Completed)
            return false;

        // Cancelled -> (no transitions allowed)
        if (this == Cancelled)
            return false;

        return false;
    }

    public bool IsPending => this == Pending;
    public bool IsConfirmed => this == Confirmed;
    public bool IsCompleted => this == Completed;
    public bool IsCancelled => this == Cancelled;

    public bool IsActive => IsPending || IsConfirmed;
    public bool IsFinal => IsCompleted || IsCancelled;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    // Implicit conversion to string for convenience
    public static implicit operator string(BookingStatus status) => status.Value;
}

