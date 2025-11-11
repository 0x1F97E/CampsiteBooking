using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// Event entity representing campsite events and activities
/// </summary>
public class Event
{
    public int EventId { get; set; }

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

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Title cannot be empty", nameof(Title));
            if (value.Length > 200)
                throw new ArgumentException("Title cannot exceed 200 characters", nameof(Title));
            _title = value;
        }
    }

    public string Description { get; set; } = string.Empty;

    private DateTime _eventDate;
    public DateTime EventDate
    {
        get => _eventDate;
        set
        {
            if (value < DateTime.UtcNow.Date)
                throw new ArgumentException("Event date cannot be in the past", nameof(EventDate));
            _eventDate = value;
        }
    }

    private int _maxParticipants;
    public int MaxParticipants
    {
        get => _maxParticipants;
        set
        {
            if (value <= 0)
                throw new ArgumentException("MaxParticipants must be greater than 0", nameof(MaxParticipants));
            if (value > 1000)
                throw new ArgumentException("MaxParticipants cannot exceed 1000", nameof(MaxParticipants));
            _maxParticipants = value;
        }
    }

    private int _currentParticipants;
    public int CurrentParticipants
    {
        get => _currentParticipants;
        set
        {
            if (value < 0)
                throw new ArgumentException("CurrentParticipants cannot be negative", nameof(CurrentParticipants));
            if (value > MaxParticipants)
                throw new ArgumentException("CurrentParticipants cannot exceed MaxParticipants", nameof(CurrentParticipants));
            _currentParticipants = value;
        }
    }

    // Value Object: Money
    private Money? _price;
    public Money? Price
    {
        get => _price;
        set => _price = value;
    }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Checks if the event is full
    /// </summary>
    public bool IsFull()
    {
        return CurrentParticipants >= MaxParticipants;
    }

    /// <summary>
    /// Checks if there are available spots
    /// </summary>
    public int AvailableSpots()
    {
        return MaxParticipants - CurrentParticipants;
    }

    /// <summary>
    /// Registers participants for the event
    /// </summary>
    public void RegisterParticipants(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Participant count must be greater than 0", nameof(count));

        if (!IsActive)
            throw new InvalidOperationException("Cannot register for inactive event");

        if (CurrentParticipants + count > MaxParticipants)
            throw new InvalidOperationException($"Not enough spots available. Only {AvailableSpots()} spots remaining");

        CurrentParticipants += count;
    }

    /// <summary>
    /// Cancels participant registrations
    /// </summary>
    public void CancelRegistrations(int count)
    {
        if (count <= 0)
            throw new ArgumentException("Participant count must be greater than 0", nameof(count));

        if (CurrentParticipants - count < 0)
            throw new InvalidOperationException("Cannot cancel more participants than registered");

        CurrentParticipants -= count;
    }

    /// <summary>
    /// Deactivates the event
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Activates the event
    /// </summary>
    public void Activate()
    {
        if (EventDate < DateTime.UtcNow.Date)
            throw new InvalidOperationException("Cannot activate past events");

        IsActive = true;
    }
}


