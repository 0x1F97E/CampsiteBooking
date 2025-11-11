namespace CampsiteBooking.Models.ValueObjects;

/// <summary>
/// Value Object representing payment status with transition rules.
/// Ensures only valid status transitions are allowed.
/// </summary>
public class PaymentStatus : ValueObject
{
    public string Value { get; }

    // Valid statuses
    public static readonly PaymentStatus Pending = new PaymentStatus("Pending");
    public static readonly PaymentStatus Completed = new PaymentStatus("Completed");
    public static readonly PaymentStatus Failed = new PaymentStatus("Failed");
    public static readonly PaymentStatus Refunded = new PaymentStatus("Refunded");

    private PaymentStatus(string value)
    {
        Value = value;
    }

    public static PaymentStatus FromString(string value)
    {
        return value switch
        {
            "Pending" => Pending,
            "Completed" => Completed,
            "Failed" => Failed,
            "Refunded" => Refunded,
            _ => throw new ArgumentException($"Invalid payment status: {value}", nameof(value))
        };
    }

    public bool CanTransitionTo(PaymentStatus newStatus)
    {
        // Pending -> Completed, Failed
        if (this == Pending)
            return newStatus == Completed || newStatus == Failed;

        // Completed -> Refunded
        if (this == Completed)
            return newStatus == Refunded;

        // Failed -> Pending (retry)
        if (this == Failed)
            return newStatus == Pending;

        // Refunded -> (no transitions allowed)
        if (this == Refunded)
            return false;

        return false;
    }

    public bool IsPending => this == Pending;
    public bool IsCompleted => this == Completed;
    public bool IsFailed => this == Failed;
    public bool IsRefunded => this == Refunded;

    public bool IsSuccessful => IsCompleted || IsRefunded;
    public bool IsFinal => IsCompleted || IsRefunded;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }

    // Implicit conversion to string for convenience
    public static implicit operator string(PaymentStatus status) => status.Value;
}

