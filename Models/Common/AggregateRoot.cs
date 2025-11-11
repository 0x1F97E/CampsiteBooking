using CampsiteBooking.Models.DomainEvents;

namespace CampsiteBooking.Models.Common;

/// <summary>
/// Base class for aggregate roots in the domain.
/// Aggregate roots are the entry point to aggregates and enforce invariants.
/// They are responsible for raising domain events.
/// </summary>
/// <typeparam name="TId">The type of the aggregate root's identifier</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : notnull
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Domain events that have been raised by this aggregate
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Protected constructor
    /// </summary>
    protected AggregateRoot()
    {
    }

    /// <summary>
    /// Constructor that accepts an ID
    /// </summary>
    protected AggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>
    /// Raise a domain event
    /// </summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clear all domain events (typically called after events are dispatched)
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

