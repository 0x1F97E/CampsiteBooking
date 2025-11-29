namespace CampsiteBooking.Services;

/// <summary>
/// Mock payment service for testing without real Stripe integration
/// This bypasses Stripe and simulates successful payments
/// </summary>
public class MockPaymentService
{
    private readonly ILogger<MockPaymentService> _logger;
    private static readonly Dictionary<string, MockSession> _sessions = new();

    public MockPaymentService(ILogger<MockPaymentService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Create a mock checkout session (simulates Stripe)
    /// </summary>
    public Task<string> CreateCheckoutSessionAsync(
        int bookingId,
        string campsiteName,
        string accommodationType,
        decimal amount,
        string currency = "DKK",
        CancellationToken cancellationToken = default)
    {
        var sessionId = $"mock_session_{Guid.NewGuid():N}";
        var paymentIntentId = $"mock_pi_{Guid.NewGuid():N}";

        _sessions[sessionId] = new MockSession
        {
            SessionId = sessionId,
            PaymentIntentId = paymentIntentId,
            BookingId = bookingId,
            CampsiteName = campsiteName,
            AccommodationType = accommodationType,
            Amount = amount,
            Currency = currency,
            Status = "open"
        };

        _logger.LogInformation("✅ Created MOCK Checkout Session {SessionId} for Booking {BookingId} (Amount: {Amount} {Currency})",
            sessionId, bookingId, amount, currency);

        // Return a mock checkout URL that redirects to success page
        var checkoutUrl = $"http://localhost:5063/payment-success?session_id={sessionId}";
        return Task.FromResult(checkoutUrl);
    }

    /// <summary>
    /// Verify a mock checkout session
    /// </summary>
    public Task<(bool success, string paymentIntentId)> VerifyCheckoutSessionAsync(string sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            session.Status = "complete";
            _logger.LogInformation("✅ Verified MOCK Checkout Session {SessionId} - Payment Intent: {PaymentIntentId}",
                sessionId, session.PaymentIntentId);
            return Task.FromResult((true, session.PaymentIntentId));
        }

        _logger.LogWarning("⚠️ MOCK Checkout Session {SessionId} not found", sessionId);
        return Task.FromResult((false, string.Empty));
    }

    /// <summary>
    /// Get booking ID from mock session
    /// </summary>
    public Task<int?> GetBookingIdFromSessionAsync(string sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            return Task.FromResult<int?>(session.BookingId);
        }

        return Task.FromResult<int?>(null);
    }

    /// <summary>
    /// Retrieve a mock checkout session
    /// </summary>
    public Task<MockSession?> RetrieveCheckoutSessionAsync(string sessionId)
    {
        _sessions.TryGetValue(sessionId, out var session);
        return Task.FromResult(session);
    }

    public class MockSession
    {
        public string SessionId { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public int BookingId { get; set; }
        public string CampsiteName { get; set; } = string.Empty;
        public string AccommodationType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}

