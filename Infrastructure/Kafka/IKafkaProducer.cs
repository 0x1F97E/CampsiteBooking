using CampsiteBooking.Models.DomainEvents;

namespace CampsiteBooking.Infrastructure.Kafka;

/// <summary>
/// Interface for publishing domain events to Kafka topics.
/// Abstracts Kafka implementation details from domain layer.
/// </summary>
public interface IKafkaProducer
{
    /// <summary>
    /// Publish a domain event to Kafka
    /// </summary>
    /// <param name="domainEvent">The domain event to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;

    /// <summary>
    /// Publish multiple domain events to Kafka
    /// </summary>
    /// <param name="domainEvents">Collection of domain events to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishBatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

