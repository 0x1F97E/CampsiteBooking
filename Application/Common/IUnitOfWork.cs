using CampsiteBooking.Models.Repositories;
using System.Data;

namespace CampsiteBooking.Application.Common;

/// <summary>
/// Unit of Work pattern interface.
/// Manages transactions and coordinates repository operations.
/// Uses IsolationLevel.Serializable to prevent phantom reads (as required by teacher).
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Booking repository
    /// </summary>
    IBookingRepository Bookings { get; }

    /// <summary>
    /// User repository
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Campsite repository
    /// </summary>
    ICampsiteRepository Campsites { get; }

    /// <summary>
    /// Begin a database transaction with specified isolation level.
    /// Default: IsolationLevel.Serializable (prevents phantom reads).
    /// </summary>
    Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Serializable, CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Save all changes to the database.
    /// This will also publish domain events to Kafka (via DbContext override).
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

