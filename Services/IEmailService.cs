namespace CampsiteBooking.Services;

/// <summary>
/// Interface for email notification service
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send booking confirmation email
    /// </summary>
    Task SendBookingConfirmationAsync(int bookingId, string guestEmail, string guestName, string campsiteName, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send booking cancellation email
    /// </summary>
    Task SendBookingCancellationAsync(int bookingId, string guestEmail, string guestName, string campsiteName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send payment receipt email
    /// </summary>
    Task SendPaymentReceiptAsync(int paymentId, int bookingId, string guestEmail, string guestName, decimal amount, string currency, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send welcome email to new user
    /// </summary>
    Task SendWelcomeEmailAsync(string email, string firstName, CancellationToken cancellationToken = default);
}

