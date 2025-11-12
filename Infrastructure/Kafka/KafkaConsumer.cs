using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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

        // TODO: Deserialize and route to appropriate event handler
        // For now, just log the event
        switch (eventType)
        {
            case "BookingConfirmedEvent":
                _logger.LogInformation("BookingConfirmedEvent received - would send confirmation email/SMS");
                break;

            case "BookingCancelledEvent":
                _logger.LogInformation("BookingCancelledEvent received - would send cancellation email/SMS");
                break;

            case "BookingCreatedEvent":
                _logger.LogInformation("BookingCreatedEvent received - would send booking created notification");
                break;

            case "PaymentCompletedEvent":
                _logger.LogInformation("PaymentCompletedEvent received - would send payment receipt");
                break;

            case "UserCreatedEvent":
                _logger.LogInformation("UserCreatedEvent received - would send welcome email");
                break;

            default:
                _logger.LogWarning("Unknown event type: {EventType}", eventType);
                break;
        }

        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
        _logger.LogInformation("Kafka consumer disposed");
    }
}

