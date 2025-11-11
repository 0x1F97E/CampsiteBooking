using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// Availability entity representing daily availability for an accommodation type
/// Part of the Campsite aggregate
/// </summary>
public class Availability : Entity<AvailabilityId>
{
    private CampsiteId _campsiteId = null!;
    private AccommodationTypeId _accommodationTypeId = null!;
    private DateTime _date;
    private int _availableUnits;
    private int _reservedUnits;
    private DateTime _createdDate;
    private DateTime _updatedDate;
    
    public CampsiteId CampsiteId => _campsiteId;
    public AccommodationTypeId AccommodationTypeId => _accommodationTypeId;
    public DateTime Date => _date;
    public int AvailableUnits => _availableUnits;
    public int ReservedUnits => _reservedUnits;
    public DateTime CreatedDate => _createdDate;
    public DateTime UpdatedDate => _updatedDate;
    
    public int AvailabilityId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.AvailabilityId.Create(value) : ValueObjects.AvailabilityId.CreateNew();
    }
    
    public static Availability Create(CampsiteId campsiteId, AccommodationTypeId accommodationTypeId, DateTime date, int totalUnits)
    {
        if (date.Date < DateTime.UtcNow.Date)
            throw new DomainException("Date cannot be in the past");
        
        if (totalUnits < 0)
            throw new DomainException("Total units cannot be negative");
        
        return new Availability
        {
            Id = ValueObjects.AvailabilityId.CreateNew(),
            _campsiteId = campsiteId,
            _accommodationTypeId = accommodationTypeId,
            _date = date.Date,
            _availableUnits = totalUnits,
            _reservedUnits = 0,
            _createdDate = DateTime.UtcNow,
            _updatedDate = DateTime.UtcNow
        };
    }
    
    private Availability() { }
    
    public void ReserveUnits(int count)
    {
        if (count <= 0) throw new DomainException("Count must be positive");
        if (count > _availableUnits) throw new DomainException("Not enough available units to reserve");
        _availableUnits -= count;
        _reservedUnits += count;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void ReleaseUnits(int count)
    {
        if (count <= 0) throw new DomainException("Count must be positive");
        if (count > _reservedUnits) throw new DomainException("Cannot release more units than reserved");
        _reservedUnits -= count;
        _availableUnits += count;
        _updatedDate = DateTime.UtcNow;
    }
    
    public int GetTotalCapacity() => _availableUnits + _reservedUnits;
    public bool HasAvailability(int requestedUnits) => _availableUnits >= requestedUnits;
}

