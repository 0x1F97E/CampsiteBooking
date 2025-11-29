using CampsiteBooking.Data;
using CampsiteBooking.Models;
using CampsiteBooking.Models.DomainEvents;
using CampsiteBooking.Models.ValueObjects;
using CampsiteBooking.Services;
using Microsoft.EntityFrameworkCore;

namespace CampsiteBooking.Infrastructure.EventHandlers;

/// <summary>
/// Event handler for sending email and SMS notifications based on user preferences
/// </summary>
public class NotificationEventHandler : IEventHandler
{
    private readonly IDbContextFactory<CampsiteBookingDbContext> _dbContextFactory;
    private readonly IEmailService _emailService;
    private readonly ISMSService _smsService;
    private readonly ILogger<NotificationEventHandler> _logger;

    public NotificationEventHandler(
        IDbContextFactory<CampsiteBookingDbContext> dbContextFactory,
        IEmailService emailService,
        ISMSService smsService,
        ILogger<NotificationEventHandler> logger)
    {
        _dbContextFactory = dbContextFactory;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public bool CanHandle(string eventType)
    {
        return eventType switch
        {
            nameof(BookingCreatedEvent) => true,
            nameof(BookingConfirmedEvent) => true,
            nameof(BookingCancelledEvent) => true,
            nameof(PaymentCompletedEvent) => true,
            nameof(UserCreatedEvent) => true,
            _ => false
        };
    }

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            switch (domainEvent)
            {
                case BookingCreatedEvent evt:
                    await HandleBookingCreatedAsync(evt, cancellationToken);
                    break;

                case BookingConfirmedEvent evt:
                    await HandleBookingConfirmedAsync(evt, cancellationToken);
                    break;

                case BookingCancelledEvent evt:
                    await HandleBookingCancelledAsync(evt, cancellationToken);
                    break;

                case PaymentCompletedEvent evt:
                    await HandlePaymentCompletedAsync(evt, cancellationToken);
                    break;

                case UserCreatedEvent evt:
                    await HandleUserCreatedAsync(evt, cancellationToken);
                    break;

                default:
                    _logger.LogWarning("Unhandled event type: {EventType}", domainEvent.GetType().Name);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling event {EventType}", domainEvent.GetType().Name);
            throw;
        }
    }

    private async Task HandleBookingCreatedAsync(BookingCreatedEvent evt, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        // Load guest and campsite information
        var guest = await context.Users.OfType<Guest>()
            .FirstOrDefaultAsync(u => u.Id == GuestId.Create(evt.GuestId), cancellationToken);

        var campsite = await context.Campsites
            .FirstOrDefaultAsync(c => c.Id == CampsiteId.Create(evt.CampsiteId), cancellationToken);

        if (guest == null || campsite == null)
        {
            _logger.LogWarning("Guest or Campsite not found for BookingCreatedEvent: GuestId={GuestId}, CampsiteId={CampsiteId}", 
                evt.GuestId, evt.CampsiteId);
            return;
        }

        var guestName = $"{guest.FirstName} {guest.LastName}";
        var guestEmail = guest.Email.Value;
        var guestPhone = guest.Phone;
        var preferredCommunication = guest.PreferredCommunication;

        _logger.LogInformation("Sending booking created notification to {GuestName} via {PreferredCommunication}", 
            guestName, preferredCommunication);

        // Send notification based on preferred communication method
        if (preferredCommunication == "Email" || preferredCommunication == "Both")
        {
            await _emailService.SendBookingConfirmationAsync(
                evt.BookingId,
                guestEmail,
                guestName,
                campsite.Name,
                evt.CheckInDate,
                evt.CheckOutDate,
                cancellationToken);
        }

        if ((preferredCommunication == "SMS" || preferredCommunication == "Both") && !string.IsNullOrEmpty(guestPhone))
        {
            await _smsService.SendBookingConfirmationSMSAsync(
                guestPhone,
                guestName,
                campsite.Name,
                evt.CheckInDate,
                evt.CheckOutDate,
                cancellationToken);
        }
    }

    private async Task HandleBookingConfirmedAsync(BookingConfirmedEvent evt, CancellationToken cancellationToken)
    {
        // Similar to BookingCreatedAsync - send confirmation notification
        await HandleBookingCreatedAsync(
            new BookingCreatedEvent(evt.BookingId, evt.GuestId, 0, evt.CheckInDate, evt.CheckOutDate),
            cancellationToken);
    }

    private async Task HandleBookingCancelledAsync(BookingCancelledEvent evt, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var guest = await context.Users.OfType<Guest>()
            .FirstOrDefaultAsync(u => u.Id == GuestId.Create(evt.GuestId), cancellationToken);

        var booking = await context.Bookings
            .FirstOrDefaultAsync(b => EF.Property<BookingId>(b, "_id") == BookingId.Create(evt.BookingId), cancellationToken);

        if (guest == null || booking == null)
        {
            _logger.LogWarning("Guest or Booking not found for BookingCancelledEvent: GuestId={GuestId}, BookingId={BookingId}",
                evt.GuestId, evt.BookingId);
            return;
        }

        var campsite = await context.Campsites
            .FirstOrDefaultAsync(c => c.Id == EF.Property<CampsiteId>(booking, "_campsiteId"), cancellationToken);

        if (campsite == null)
        {
            _logger.LogWarning("Campsite not found for booking {BookingId}", evt.BookingId);
            return;
        }

        var guestName = $"{guest.FirstName} {guest.LastName}";
        var guestEmail = guest.Email.Value;
        var guestPhone = guest.Phone;
        var preferredCommunication = guest.PreferredCommunication;

        // Send notification based on preferred communication method
        if (preferredCommunication == "Email" || preferredCommunication == "Both")
        {
            await _emailService.SendBookingCancellationAsync(
                evt.BookingId,
                guestEmail,
                guestName,
                campsite.Name,
                cancellationToken);
        }

        if ((preferredCommunication == "SMS" || preferredCommunication == "Both") && !string.IsNullOrEmpty(guestPhone))
        {
            await _smsService.SendBookingCancellationSMSAsync(
                guestPhone,
                guestName,
                campsite.Name,
                cancellationToken);
        }
    }

    private async Task HandlePaymentCompletedAsync(PaymentCompletedEvent evt, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var booking = await context.Bookings
            .FirstOrDefaultAsync(b => EF.Property<BookingId>(b, "_id") == BookingId.Create(evt.BookingId), cancellationToken);

        if (booking == null)
        {
            _logger.LogWarning("Booking not found for PaymentCompletedEvent: BookingId={BookingId}", evt.BookingId);
            return;
        }

        var guest = await context.Users.OfType<Guest>()
            .FirstOrDefaultAsync(u => u.Id == EF.Property<GuestId>(booking, "_guestId"), cancellationToken);

        if (guest == null)
        {
            _logger.LogWarning("Guest not found for booking {BookingId}", evt.BookingId);
            return;
        }

        var guestName = $"{guest.FirstName} {guest.LastName}";
        var guestEmail = guest.Email.Value;
        var guestPhone = guest.Phone;
        var preferredCommunication = guest.PreferredCommunication;

        // Send notification based on preferred communication method
        if (preferredCommunication == "Email" || preferredCommunication == "Both")
        {
            await _emailService.SendPaymentReceiptAsync(
                evt.PaymentId,
                evt.BookingId,
                guestEmail,
                guestName,
                evt.Amount,
                evt.Currency,
                cancellationToken);
        }

        if ((preferredCommunication == "SMS" || preferredCommunication == "Both") && !string.IsNullOrEmpty(guestPhone))
        {
            await _smsService.SendPaymentConfirmationSMSAsync(
                guestPhone,
                guestName,
                evt.Amount,
                evt.Currency,
                cancellationToken);
        }
    }

    private async Task HandleUserCreatedAsync(UserCreatedEvent evt, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == UserId.Create(evt.UserId), cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User not found for UserCreatedEvent: UserId={UserId}", evt.UserId);
            return;
        }

        await _emailService.SendWelcomeEmailAsync(user.Email.Value, user.FirstName, cancellationToken);
    }
}

