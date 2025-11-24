using CampsiteBooking.Data;
using CampsiteBooking.Models;
using CampsiteBooking.Models.Repositories;
using CampsiteBooking.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace CampsiteBooking.Data.Repositories;

/// <summary>
/// Concrete implementation of IUserRepository using EF Core.
/// Handles data access for User aggregate root (including Guest, Admin, Staff).
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly CampsiteBookingDbContext _context;

    public UserRepository(CampsiteBookingDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // ============================================================================
    // IUserRepository Implementation
    // ============================================================================

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => EF.Property<Email>(u, "_email") == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => EF.Property<Email>(u, "_email") == email, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        // TPH (Table Per Hierarchy) - filter by discriminator
        return role.ToLower() switch
        {
            "guest" => await _context.Users.OfType<Guest>().ToListAsync(cancellationToken),
            "admin" => await _context.Users.OfType<Admin>().ToListAsync(cancellationToken),
            "staff" => await _context.Users.OfType<Staff>().ToListAsync(cancellationToken),
            _ => await _context.Users.ToListAsync(cancellationToken)
        };
    }

    // ============================================================================
    // Generic Repository Methods (not in interface yet, but useful)
    // ============================================================================

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    // ============================================================================
    // Additional Domain-Specific Queries
    // ============================================================================

    public async Task<IEnumerable<Guest>> GetAllGuestsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .OfType<Guest>()
            .OrderBy(g => g.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Staff>> GetStaffByCampsiteAsync(CampsiteId campsiteId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .OfType<Staff>()
            .Where(s => s.CampsiteId == campsiteId)
            .OrderBy(s => s.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.LastName)
            .ToListAsync(cancellationToken);
    }
}

