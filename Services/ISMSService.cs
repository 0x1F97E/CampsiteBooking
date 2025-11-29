namespace CampsiteBooking.Services;

/// <summary>
/// Interface for SMS notification service
/// </summary>
public interface ISMSService
{
    /// <summary>
    /// Send booking confirmation SMS
    /// </summary>
    Task SendBookingConfirmationSMSAsync(string phoneNumber, string guestName, string campsiteName, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send booking cancellation SMS
    /// </summary>
    Task SendBookingCancellationSMSAsync(string phoneNumber, string guestName, string campsiteName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send payment confirmation SMS
    /// </summary>
    Task SendPaymentConfirmationSMSAsync(string phoneNumber, string guestName, decimal amount, string currency, CancellationToken cancellationToken = default);
}

