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
            .Where(b => b.GuestId == guestId)
            .OrderByDescending(b => b.Period.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetByCampsiteIdAsync(CampsiteId campsiteId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.CampsiteId == campsiteId)
            .OrderByDescending(b => b.Period.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetByAccommodationSpotIdAsync(AccommodationSpotId spotId, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.AccommodationSpotId == spotId)
            .OrderByDescending(b => b.Period.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Booking>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
            .Where(b => b.Period.StartDate >= startDate && b.Period.EndDate <= endDate)
            .OrderBy(b => b.Period.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsSpotAvailableAsync(AccommodationSpotId spotId, DateRange period, CancellationToken cancellationToken = default)
    {
        // Check if there are any confirmed bookings that overlap with the requested period
        var hasOverlap = await _context.Bookings
            .Where(b => b.AccommodationSpotId == spotId)
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending)
            .AnyAsync(b => 
                // Overlap logic: booking overlaps if it starts before period ends AND ends after period starts
                b.Period.StartDate < period.EndDate && b.Period.EndDate > period.StartDate,
                cancellationToken);

        return !hasOverlap; // Available if no overlap
    }
}

