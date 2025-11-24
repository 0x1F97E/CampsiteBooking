using Stripe;
using Stripe.Checkout;

namespace CampsiteBooking.Services;

/// <summary>
/// Service for handling Stripe payment processing
/// </summary>
public class StripePaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<StripePaymentService> _logger;

    public StripePaymentService(IConfiguration configuration, ILogger<StripePaymentService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Set Stripe API key
        var secretKey = _configuration["Stripe:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Stripe SecretKey is not configured in appsettings.json");
        }
        StripeConfiguration.ApiKey = secretKey;
    }

    /// <summary>
    /// Create a Stripe Checkout Session for booking payment
    /// </summary>
    public async Task<string> CreateCheckoutSessionAsync(
        int bookingId,
        string campsiteName,
        string accommodationType,
        decimal amount,
        string currency = "DKK",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency.ToLower(),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"{campsiteName} - {accommodationType}",
                                Description = $"Booking #{bookingId}",
                            },
                            UnitAmount = (long)(amount * 100), // Stripe uses smallest currency unit (øre for DKK)
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = $"{GetBaseUrl()}/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{GetBaseUrl()}/payment-cancel?booking_id={bookingId}",
                Metadata = new Dictionary<string, string>
                {
                    { "booking_id", bookingId.ToString() },
                    { "campsite_name", campsiteName },
                    { "accommodation_type", accommodationType }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogInformation("Created Stripe Checkout Session {SessionId} for Booking {BookingId}", 
                session.Id, bookingId);

            return session.Url; // Return the Stripe Checkout URL
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error creating checkout session for Booking {BookingId}", bookingId);
            throw new InvalidOperationException($"Payment processing error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Verify a Stripe Checkout Session was successful
    /// </summary>
    public async Task<(bool Success, string? PaymentIntentId)> VerifyCheckoutSessionAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId, cancellationToken: cancellationToken);

            var success = session.PaymentStatus == "paid";
            var paymentIntentId = session.PaymentIntentId;

            _logger.LogInformation("Verified Stripe Session {SessionId}: Success={Success}, PaymentIntent={PaymentIntentId}",
                sessionId, success, paymentIntentId);

            return (success, paymentIntentId);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error verifying session {SessionId}", sessionId);
            return (false, null);
        }
    }

    /// <summary>
    /// Get booking ID from Stripe session metadata
    /// </summary>
    public async Task<int?> GetBookingIdFromSessionAsync(
        string sessionId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId, cancellationToken: cancellationToken);

            if (session.Metadata.TryGetValue("booking_id", out var bookingIdStr) &&
                int.TryParse(bookingIdStr, out var bookingId))
            {
                return bookingId;
            }

            return null;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Error getting booking ID from session {SessionId}", sessionId);
            return null;
        }
    }

    private string GetBaseUrl()
    {
        // In production, this should come from configuration
        return "http://localhost:5063";
    }
}

