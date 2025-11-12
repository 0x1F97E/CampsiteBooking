using CampsiteBooking.Models.DomainEvents;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CampsiteBooking.Infrastructure.Kafka;

/// <summary>
/// Kafka producer implementation for publishing domain events.
/// Publishes events to topic: "bookmyhome.events"
/// </summary>
public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    private const string TopicName = "bookmyhome.events";

    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            ClientId = "bookmyhome-producer",
            Acks = Acks.All, // Wait for all replicas to acknowledge
            EnableIdempotence = true, // Prevent duplicate messages
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, error) =>
            {
                _logger.LogError("Kafka producer error: {Reason}", error.Reason);
            })
            .Build();

        _logger.LogInformation("Kafka producer initialized with bootstrap servers: {Servers}", 
            config.BootstrapServers);
    }

    public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        if (domainEvent == null)
            throw new ArgumentNullException(nameof(domainEvent));

        try
        {
            var eventType = domainEvent.GetType().Name;
            var eventJson = JsonSerializer.Serialize(domainEvent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            var message = new Message<string, string>
            {
                Key = eventType, // Use event type as partition key
                Value = eventJson,
                Headers = new Headers
                {
                    { "event-type", System.Text.Encoding.UTF8.GetBytes(eventType) },
                    { "timestamp", System.Text.Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("O")) }
                }
            };

            var result = await _producer.ProduceAsync(TopicName, message, cancellationToken);

            _logger.LogInformation(
                "Published event {EventType} to Kafka topic {Topic} at offset {Offset}",
                eventType, TopicName, result.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, 
                "Failed to publish event {EventType} to Kafka: {Error}", 
                domainEvent.GetType().Name, ex.Error.Reason);
            throw;
        }
    }

    public async Task PublishBatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        if (domainEvents == null || !domainEvents.Any())
            return;

        var tasks = domainEvents.Select(evt => PublishAsync(evt, cancellationToken));
        await Task.WhenAll(tasks);

        _logger.LogInformation("Published batch of {Count} events to Kafka", domainEvents.Count());
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
        _logger.LogInformation("Kafka producer disposed");
    }
}

