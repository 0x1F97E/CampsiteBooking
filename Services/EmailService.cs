using System.Net;
using System.Net.Mail;

namespace CampsiteBooking.Services;

/// <summary>
/// Email service implementation using SMTP
/// For production, configure SMTP settings in appsettings.json
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Read SMTP configuration from appsettings.json
        _smtpHost = _configuration["SMTP:Host"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["SMTP:Port"] ?? "587");
        _smtpUsername = _configuration["SMTP:Username"] ?? "";
        _smtpPassword = _configuration["SMTP:Password"] ?? "";
        _fromEmail = _configuration["SMTP:FromEmail"] ?? "noreply@campsitebooking.dk";
        _fromName = _configuration["SMTP:FromName"] ?? "CampsiteBooking";
    }

    public async Task SendBookingConfirmationAsync(int bookingId, string guestEmail, string guestName, string campsiteName, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default)
    {
        var subject = $"Booking Confirmation - {campsiteName}";
        var body = $@"
            <html>
            <body>
                <h2>Booking Confirmation</h2>
                <p>Dear {guestName},</p>
                <p>Your booking has been confirmed!</p>
                <p><strong>Booking Details:</strong></p>
                <ul>
                    <li>Booking ID: {bookingId}</li>
                    <li>Campsite: {campsiteName}</li>
                    <li>Check-in: {checkIn:MMMM dd, yyyy}</li>
                    <li>Check-out: {checkOut:MMMM dd, yyyy}</li>
                </ul>
                <p>We look forward to welcoming you!</p>
                <p>Best regards,<br/>CampsiteBooking Team</p>
            </body>
            </html>";

        await SendEmailAsync(guestEmail, subject, body, cancellationToken);
    }

    public async Task SendBookingCancellationAsync(int bookingId, string guestEmail, string guestName, string campsiteName, CancellationToken cancellationToken = default)
    {
        var subject = $"Booking Cancellation - {campsiteName}";
        var body = $@"
            <html>
            <body>
                <h2>Booking Cancellation</h2>
                <p>Dear {guestName},</p>
                <p>Your booking has been cancelled.</p>
                <p><strong>Cancelled Booking:</strong></p>
                <ul>
                    <li>Booking ID: {bookingId}</li>
                    <li>Campsite: {campsiteName}</li>
                </ul>
                <p>If you have any questions, please contact us.</p>
                <p>Best regards,<br/>CampsiteBooking Team</p>
            </body>
            </html>";

        await SendEmailAsync(guestEmail, subject, body, cancellationToken);
    }

    public async Task SendPaymentReceiptAsync(int paymentId, int bookingId, string guestEmail, string guestName, decimal amount, string currency, CancellationToken cancellationToken = default)
    {
        var subject = $"Payment Receipt - Booking #{bookingId}";
        var body = $@"
            <html>
            <body>
                <h2>Payment Receipt</h2>
                <p>Dear {guestName},</p>
                <p>Thank you for your payment!</p>
                <p><strong>Payment Details:</strong></p>
                <ul>
                    <li>Payment ID: {paymentId}</li>
                    <li>Booking ID: {bookingId}</li>
                    <li>Amount: {amount:F2} {currency}</li>
                </ul>
                <p>Best regards,<br/>CampsiteBooking Team</p>
            </body>
            </html>";

        await SendEmailAsync(guestEmail, subject, body, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(string email, string firstName, CancellationToken cancellationToken = default)
    {
        var subject = "Welcome to CampsiteBooking!";
        var body = $@"
            <html>
            <body>
                <h2>Welcome to CampsiteBooking!</h2>
                <p>Dear {firstName},</p>
                <p>Thank you for creating an account with us.</p>
                <p>You can now browse and book campsites across Denmark.</p>
                <p>Best regards,<br/>CampsiteBooking Team</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body, cancellationToken);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
    {
        try
        {
            // For development/demo: just log the email instead of actually sending
            if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
            {
                _logger.LogInformation("📧 [DEMO MODE] Email would be sent to {Email}: {Subject}", toEmail, subject);
                _logger.LogDebug("Email body: {Body}", htmlBody);
                return;
            }

            // Production: Actually send email via SMTP
            using var message = new MailMessage();
            message.From = new MailAddress(_fromEmail, _fromName);
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = htmlBody;
            message.IsBodyHtml = true;

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
            smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("✅ Email sent successfully to {Email}: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to send email to {Email}: {Subject}", toEmail, subject);
            throw;
        }
    }
}

