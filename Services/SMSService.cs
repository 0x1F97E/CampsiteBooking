// Twilio package not installed yet - uncomment when ready to test SMS
// using Twilio;
// using Twilio.Rest.Api.V2010.Account;
// using Twilio.Types;

namespace CampsiteBooking.Services;

/// <summary>
/// SMS service implementation using Twilio
/// For production, configure Twilio settings in appsettings.json
/// </summary>
public class SMSService : ISMSService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SMSService> _logger;
    private readonly string _twilioAccountSid;
    private readonly string _twilioAuthToken;
    private readonly string _twilioPhoneNumber;

    public SMSService(IConfiguration configuration, ILogger<SMSService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Read Twilio configuration from appsettings.json
        _twilioAccountSid = _configuration["Twilio:AccountSid"] ?? "";
        _twilioAuthToken = _configuration["Twilio:AuthToken"] ?? "";
        _twilioPhoneNumber = _configuration["Twilio:PhoneNumber"] ?? "";
    }

    public async Task SendBookingConfirmationSMSAsync(string phoneNumber, string guestName, string campsiteName, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default)
    {
        var message = $"Hi {guestName}, your booking at {campsiteName} is confirmed! Check-in: {checkIn:MMM dd}, Check-out: {checkOut:MMM dd}. See you soon!";
        await SendSMSAsync(phoneNumber, message, cancellationToken);
    }

    public async Task SendBookingCancellationSMSAsync(string phoneNumber, string guestName, string campsiteName, CancellationToken cancellationToken = default)
    {
        var message = $"Hi {guestName}, your booking at {campsiteName} has been cancelled. Contact us if you have questions.";
        await SendSMSAsync(phoneNumber, message, cancellationToken);
    }

    public async Task SendPaymentConfirmationSMSAsync(string phoneNumber, string guestName, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        var message = $"Hi {guestName}, payment of {amount:F2} {currency} received. Thank you!";
        await SendSMSAsync(phoneNumber, message, cancellationToken);
    }

    private async Task SendSMSAsync(string phoneNumber, string message, CancellationToken cancellationToken)
    {
        try
        {
            // For development/demo: just log the SMS instead of actually sending
            if (string.IsNullOrEmpty(_twilioAccountSid) || string.IsNullOrEmpty(_twilioAuthToken))
            {
                _logger.LogInformation("📱 [DEMO MODE] SMS would be sent to {PhoneNumber}: {Message}", phoneNumber, message);
                await Task.CompletedTask;
                return;
            }

            // Production: Actually send SMS via Twilio (commented out until Twilio package is installed)
            // TwilioClient.Init(_twilioAccountSid, _twilioAuthToken);

            // var messageResource = await MessageResource.CreateAsync(
            //     body: message,
            //     from: new PhoneNumber(_twilioPhoneNumber),
            //     to: new PhoneNumber(phoneNumber)
            // );

            // _logger.LogInformation("✅ SMS sent successfully to {PhoneNumber} - SID: {MessageSid}",
            //     phoneNumber, messageResource.Sid);

            _logger.LogWarning("⚠️ Twilio package not installed - SMS functionality disabled");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to send SMS to {PhoneNumber}", phoneNumber);
            throw;
        }
    }
}

