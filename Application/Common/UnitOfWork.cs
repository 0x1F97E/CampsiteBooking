using CampsiteBooking.Data;
using CampsiteBooking.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;

namespace CampsiteBooking.Application.Common;

/// <summary>
/// Unit of Work implementation using EF Core.
/// Manages transactions with IsolationLevel.Serializable to prevent phantom reads.
/// Coordinates multiple repository operations in a single transaction.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly CampsiteBookingDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(
        CampsiteBookingDbContext context,
        IBookingRepository bookings,
        IUserRepository users,
        ICampsiteRepository campsites)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Bookings = bookings ?? throw new ArgumentNullException(nameof(bookings));
        Users = users ?? throw new ArgumentNullException(nameof(users));
        Campsites = campsites ?? throw new ArgumentNullException(nameof(campsites));
    }

    public IBookingRepository Bookings { get; }
    public IUserRepository Users { get; }
    public ICampsiteRepository Campsites { get; }

    public async Task BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.Serializable,
        CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
            throw new InvalidOperationException("Transaction already started");

        // Open connection if not already open
        var connection = _context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        // Begin transaction with specified isolation level
        var dbTransaction = connection.BeginTransaction(isolationLevel);

        // Use the transaction in EF Core context
        _transaction = await _context.Database.UseTransactionAsync(dbTransaction, cancellationToken);

        // Note: IsolationLevel.Serializable prevents phantom reads (required by teacher)
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No transaction started");

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No transaction started");

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // This will trigger DbContext.SaveChangesAsync override
        // which publishes domain events to Kafka
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context?.Dispose();
            }
            _disposed = true;
        }
    }
}

