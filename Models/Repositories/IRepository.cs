using CampsiteBooking.Models.Common;

namespace CampsiteBooking.Models.Repositories;

/// <summary>
/// Base repository interface for aggregate roots.
/// Repositories are defined in the domain layer but implemented in infrastructure.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type</typeparam>
/// <typeparam name="TId">The aggregate root's ID type</typeparam>
public interface IRepository<TAggregate, TId>
    where TAggregate : AggregateRoot<TId>
    where TId : notnull
{
    /// <summary>
    /// Get an aggregate by its ID
    /// </summary>
    Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all aggregates
    /// </summary>
    Task<IEnumerable<TAggregate>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a new aggregate
    /// </summary>
    Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing aggregate
    /// </summary>
    Task UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an aggregate
    /// </summary>
    Task DeleteAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Save all changes (unit of work pattern)
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

