namespace CampsiteBooking.Models.DomainEvents;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent something that happened in the domain that domain experts care about.
/// </summary>
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

