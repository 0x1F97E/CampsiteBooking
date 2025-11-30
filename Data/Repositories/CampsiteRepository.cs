using CampsiteBooking.Data;
using CampsiteBooking.Models;
using CampsiteBooking.Models.Repositories;
using CampsiteBooking.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CampsiteBooking.Data.Repositories;

/// <summary>
/// Concrete implementation of ICampsiteRepository using EF Core.
/// Handles data access for Campsite aggregate root.
/// </summary>
public class CampsiteRepository : ICampsiteRepository
{
    private readonly CampsiteBookingDbContext _context;

    public CampsiteRepository(CampsiteBookingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // ============================================================================
    // ICampsiteRepository Implementation
    // ============================================================================

    public async Task<IEnumerable<Campsite>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Campsites
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Campsite>> GetByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        return await _context.Campsites
            .Where(c => c.City.Contains(location) || c.PostalCode.Contains(location))
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Campsite>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Campsites
            .Where(c => c.Name.Contains(searchTerm))
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    // ============================================================================
    // Generic Repository Methods (not in interface yet, but useful)
    // ============================================================================

    public async Task<Campsite?> GetByIdAsync(CampsiteId id, CancellationToken cancellationToken = default)
    {
        return await _context.Campsites
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Campsite>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Campsites
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Campsite campsite, CancellationToken cancellationToken = default)
    {
        await _context.Campsites.AddAsync(campsite, cancellationToken);
    }

    public Task UpdateAsync(Campsite campsite, CancellationToken cancellationToken = default)
    {
        _context.Campsites.Update(campsite);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Campsite campsite, CancellationToken cancellationToken = default)
    {
        _context.Campsites.Remove(campsite);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // ============================================================================
    // Additional Domain-Specific Queries
    // ============================================================================

    public async Task<IEnumerable<Campsite>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        return await _context.Campsites
            .Where(c => c.City == city)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Campsite>> GetByAttractivenessAsync(string attractiveness, CancellationToken cancellationToken = default)
    {
        return await _context.Campsites
            .Where(c => c.Attractiveness == attractiveness)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AccommodationType>> GetAccommodationTypesByCampsiteAsync(CampsiteId campsiteId, CancellationToken cancellationToken = default)
    {
        return await _context.AccommodationTypes
            .Where(at => at.CampsiteId == campsiteId)
            .OrderBy(at => at.Type)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AccommodationSpot>> GetAccommodationSpotsByTypeAsync(AccommodationTypeId typeId, CancellationToken cancellationToken = default)
    {
        return await _context.AccommodationSpots
            .Where(spot => spot.AccommodationTypeId == typeId)
            .OrderBy(spot => spot.SpotIdentifier)
            .ToListAsync(cancellationToken);
    }
}

