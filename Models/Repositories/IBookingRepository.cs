using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models.Repositories;

/// <summary>
/// Repository interface for Booking aggregate.
/// Defines domain-specific query methods.
/// </summary>
public interface IBookingRepository : IRepository<Booking, BookingId>
{
    /// <summary>
    /// Get all bookings for a specific guest
    /// </summary>
    Task<IEnumerable<Booking>> GetByGuestIdAsync(GuestId guestId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all bookings for a specific campsite
    /// </summary>
    Task<IEnumerable<Booking>> GetByCampsiteIdAsync(CampsiteId campsiteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all bookings for a specific accommodation spot
    /// </summary>
    Task<IEnumerable<Booking>> GetByAccommodationSpotIdAsync(AccommodationSpotId spotId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all bookings within a date range
    /// </summary>
    Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a spot is available for a given date range
    /// </summary>
    Task<bool> IsSpotAvailableAsync(AccommodationSpotId spotId, DateRange period, CancellationToken cancellationToken = default);
}

