using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using CampsiteBooking.Infrastructure.EventHandlers;
using CampsiteBooking.Models.DomainEvents;
using Microsoft.Extensions.DependencyInjection;

namespace CampsiteBooking.Infrastructure.Kafka;

/// <summary>
/// Kafka consumer background service that consumes domain events from Kafka.
/// Runs as a hosted service and processes events asynchronously.
/// </summary>
public class KafkaConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private const string TopicName = "bookmyhome.events";

    public KafkaConsumer(
        IConfiguration configuration,
        ILogger<KafkaConsumer> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = configuration["Kafka:GroupId"] ?? "bookmyhome-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false, // Manual commit for reliability
            ClientId = "bookmyhome-consumer"
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, error) =>
            {
                _logger.LogError("Kafka consumer error: {Reason}", error.Reason);
            })
            .Build();

        _logger.LogInformation(
            "Kafka consumer initialized with bootstrap servers: {Servers}, group: {GroupId}",
            config.BootstrapServers, config.GroupId);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(TopicName);
        _logger.LogInformation("Kafka consumer subscribed to topic: {Topic}", TopicName);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);

                    if (consumeResult?.Message != null)
                    {
                        await ProcessMessageAsync(consumeResult.Message, stoppingToken);

                        // Commit offset after successful processing
                        _consumer.Commit(consumeResult);

                        _logger.LogInformation(
                            "Processed and committed message at offset {Offset}",
                            consumeResult.Offset);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                    // Add delay to prevent tight loop when topic doesn't exist
                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Kafka message");
                }
            }
        }
        finally
        {
            _consumer.Close();
            _logger.LogInformation("Kafka consumer closed");
        }
    }

    private async Task ProcessMessageAsync(Message<string, string> message, CancellationToken cancellationToken)
    {
        var eventType = message.Key;
        var eventJson = message.Value;

        _logger.LogInformation(
            "Processing event {EventType}: {EventJson}",
            eventType, eventJson);

        try
        {
            // Deserialize the event based on type
            IDomainEvent? domainEvent = eventType switch
            {
                nameof(BookingCreatedEvent) => JsonSerializer.Deserialize<BookingCreatedEvent>(eventJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                nameof(BookingConfirmedEvent) => JsonSerializer.Deserialize<BookingConfirmedEvent>(eventJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                nameof(BookingCancelledEvent) => JsonSerializer.Deserialize<BookingCancelledEvent>(eventJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                nameof(PaymentCompletedEvent) => JsonSerializer.Deserialize<PaymentCompletedEvent>(eventJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                nameof(UserCreatedEvent) => JsonSerializer.Deserialize<UserCreatedEvent>(eventJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                _ => null
            };

            if (domainEvent == null)
            {
                _logger.LogWarning("Failed to deserialize event type: {EventType}", eventType);
                return;
            }

            // Create a scope to resolve scoped services (DbContext, etc.)
            using var scope = _serviceProvider.CreateScope();
            var eventHandler = scope.ServiceProvider.GetRequiredService<IEventHandler>();

            if (eventHandler.CanHandle(eventType))
            {
                await eventHandler.HandleAsync(domainEvent, cancellationToken);
                _logger.LogInformation("✅ Successfully handled event {EventType}", eventType);
            }
            else
            {
                _logger.LogWarning("No handler found for event type: {EventType}", eventType);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize event {EventType}", eventType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing event {EventType}", eventType);
            throw; // Re-throw to prevent committing the offset
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
        _logger.LogInformation("Kafka consumer disposed");
    }
}

