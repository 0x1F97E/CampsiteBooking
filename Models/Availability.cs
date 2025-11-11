namespace CampsiteBooking.Models;

public class Availability
{
    public int AvailabilityId { get; set; }

    private int _campsiteId;
    public int CampsiteId
    {
        get => _campsiteId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("CampsiteId must be greater than 0", nameof(CampsiteId));
            _campsiteId = value;
        }
    }

    private int _accommodationTypeId;
    public int AccommodationTypeId
    {
        get => _accommodationTypeId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("AccommodationTypeId must be greater than 0", nameof(AccommodationTypeId));
            _accommodationTypeId = value;
        }
    }

    private DateTime _date;
    public DateTime Date
    {
        get => _date;
        set
        {
            if (value.Date < DateTime.UtcNow.Date)
                throw new ArgumentException("Date cannot be in the past", nameof(Date));
            _date = value.Date; // Store only the date part
        }
    }

    private int _availableUnits;
    public int AvailableUnits
    {
        get => _availableUnits;
        set
        {
            if (value < 0)
                throw new ArgumentException("AvailableUnits cannot be negative", nameof(AvailableUnits));
            _availableUnits = value;
        }
    }

    private int _reservedUnits;
    public int ReservedUnits
    {
        get => _reservedUnits;
        set
        {
            if (value < 0)
                throw new ArgumentException("ReservedUnits cannot be negative", nameof(ReservedUnits));
            _reservedUnits = value;
        }
    }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Business methods
    public void ReserveUnits(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be positive", nameof(count));

        if (count > AvailableUnits)
            throw new InvalidOperationException("Not enough available units to reserve");

        AvailableUnits -= count;
        ReservedUnits += count;
        UpdatedDate = DateTime.UtcNow;
    }

    public void ReleaseUnits(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Count must be positive", nameof(count));

        if (count > ReservedUnits)
            throw new InvalidOperationException("Cannot release more units than reserved");

        ReservedUnits -= count;
        AvailableUnits += count;
        UpdatedDate = DateTime.UtcNow;
    }

    public int GetTotalCapacity()
    {
        return AvailableUnits + ReservedUnits;
    }

    public bool HasAvailability(int requestedUnits)
    {
        return AvailableUnits >= requestedUnits;
    }
}

