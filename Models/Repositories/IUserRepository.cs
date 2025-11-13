using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models.Repositories;

/// <summary>
/// Repository interface for User aggregate.
/// </summary>
public interface IUserRepository : IRepository<User, UserId>
{
    /// <summary>
    /// Get user by email
    /// </summary>
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if email exists
    /// </summary>
    Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all users by role
    /// </summary>
    Task<IEnumerable<User>> GetByRoleAsync(string role, CancellationToken cancellationToken = default);
}

