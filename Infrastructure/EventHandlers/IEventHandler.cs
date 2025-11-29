using CampsiteBooking.Models.DomainEvents;

namespace CampsiteBooking.Infrastructure.EventHandlers;

/// <summary>
/// Interface for handling domain events
/// </summary>
public interface IEventHandler
{
    /// <summary>
    /// Handle a domain event
    /// </summary>
    Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if this handler can handle the given event type
    /// </summary>
    bool CanHandle(string eventType);
}

