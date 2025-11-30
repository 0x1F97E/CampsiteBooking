using CampsiteBooking.Data;
using CampsiteBooking.Models;
using CampsiteBooking.Models.Repositories;
using CampsiteBooking.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CampsiteBooking.Data.Repositories;

/// <summary>
/// Concrete implementation of IBookingRepository using EF Core.
/// Handles data access for Booking aggregate root.
/// </summary>
public class BookingRepository : IBookingRepository
{
    private readonly CampsiteBookingDbContext _context;

    public BookingRepository(CampsiteBookingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // ============================================================================
    // IRepository<Booking, BookingId> Implementation
    // ============================================================================

    public async Task<Booking?> GetByIdAsync(BookingId id, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Booking aggregate, CancellationToken cancellationToken = default)
    {
        await _context.Bookings.AddAsync(aggregate, cancellationToken);
    }

    public Task UpdateAsync(Booking aggregate, CancellationToken cancellationToken = default)
    {
        _context.Bookings.Update(aggregate);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Booking aggregate, CancellationToken cancellationToken = default)
    {
        _context.Bookings.Remove(aggregate);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // ============================================================================
    // IBookingRepository Domain-Specific Queries
    // ============================================================================

    public async Task<IEnumerable<Booking>> GetByGuestIdAsync(GuestId guestId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => EF.Property<GuestId>(b, "_guestId") == guestId)
            .OrderByDescending(b => EF.Property<DateRange>(b, "_period").StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetByCampsiteIdAsync(CampsiteId campsiteId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => EF.Property<CampsiteId>(b, "_campsiteId") == campsiteId)
            .OrderByDescending(b => EF.Property<DateRange>(b, "_period").StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetByAccommodationSpotIdAsync(AccommodationSpotId spotId, CancellationToken cancellationToken = default)
    {
        // Load all bookings into memory and filter in C# to avoid EF Core translation issues with value objects
        var spotIdValue = spotId.Value;
        var allBookings = await _context.Bookings.ToListAsync(cancellationToken);
        return allBookings
            .Where(b => b.AccommodationSpotId != null && b.AccommodationSpotId.Value == spotIdValue)
            .OrderByDescending(b => b.Period.StartDate)
            .ToList();
    }

    public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        // Load all bookings into memory and filter in C# to avoid EF Core translation issues with value objects
        var allBookings = await _context.Bookings.ToListAsync(cancellationToken);
        return allBookings
            .Where(b => b.Period.StartDate >= startDate && b.Period.EndDate <= endDate)
            .OrderBy(b => b.Period.StartDate)
            .ToList();
    }

    public async Task<bool> IsSpotAvailableAsync(AccommodationSpotId spotId, DateRange period, CancellationToken cancellationToken = default)
    {
        // Load all bookings into memory and filter in C# to avoid EF Core translation issues with value objects
        var spotIdValue = spotId.Value;
        var allBookings = await _context.Bookings.ToListAsync(cancellationToken);

        // Check if there are any confirmed/pending bookings that overlap with the requested period
        var hasOverlap = allBookings
            .Where(b => b.AccommodationSpotId != null && b.AccommodationSpotId.Value == spotIdValue)
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending)
            .Any(b =>
                // Overlap logic: booking overlaps if it starts before period ends AND ends after period starts
                b.Period.StartDate < period.EndDate && b.Period.EndDate > period.StartDate);

        return !hasOverlap; // Available if no overlap
    }
}

