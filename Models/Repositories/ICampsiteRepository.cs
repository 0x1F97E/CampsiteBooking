using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models.Repositories;

/// <summary>
/// Repository interface for Campsite aggregate.
/// TODO: Implement after Campsite is converted to AggregateRoot
/// </summary>
public interface ICampsiteRepository // : IRepository<Campsite, CampsiteId>
{
    /// <summary>
    /// Get all active campsites
    /// </summary>
    Task<IEnumerable<Campsite>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get campsites by location
    /// </summary>
    Task<IEnumerable<Campsite>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search campsites by name
    /// </summary>
    Task<IEnumerable<Campsite>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
}

