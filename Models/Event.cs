using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

public class Event : Entity<ValueObjects.EventId>
{
    private CampsiteId _campsiteId = null!;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private DateTime _eventDate;
    private int _maxParticipants;
    private int _currentParticipants;
    private Money? _price;
    private bool _isActive;
    private DateTime _createdDate;
    private string _eventLink = string.Empty;

    public CampsiteId CampsiteId => _campsiteId;
    public string Title => _title;
    public string Description => _description;
    public DateTime EventDate => _eventDate;
    public int MaxParticipants => _maxParticipants;
    public int CurrentParticipants => _currentParticipants;
    public Money? Price => _price;
    public bool IsActive => _isActive;
    public DateTime CreatedDate => _createdDate;
    public string EventLink => _eventLink;
    
    public int EventId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.EventId.Create(value) : ValueObjects.EventId.CreateNew();
    }
    
    public static Event Create(CampsiteId campsiteId, string title, string description, DateTime eventDate, int maxParticipants, Money? price = null, string? eventLink = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");

        if (title.Length > 200)
            throw new DomainException("Title cannot exceed 200 characters");

        if (eventDate < DateTime.UtcNow.Date)
            throw new DomainException("Event date cannot be in the past");

        if (maxParticipants <= 0)
            throw new DomainException("Max participants must be greater than 0");

        if (maxParticipants > 1000)
            throw new DomainException("Max participants cannot exceed 1000");

        return new Event
        {
            Id = ValueObjects.EventId.CreateNew(),
            _campsiteId = campsiteId,
            _title = title.Trim(),
            _description = description?.Trim() ?? string.Empty,
            _eventDate = eventDate,
            _maxParticipants = maxParticipants,
            _currentParticipants = 0,
            _price = price,
            _isActive = true,
            _createdDate = DateTime.UtcNow,
            _eventLink = eventLink?.Trim() ?? string.Empty
        };
    }
    
    private Event() { }
    
    public bool IsFull() => _currentParticipants >= _maxParticipants;
    public int AvailableSpots() => _maxParticipants - _currentParticipants;
    
    public void RegisterParticipants(int count)
    {
        if (count <= 0)
            throw new DomainException("Participant count must be greater than 0");
        
        if (!_isActive)
            throw new DomainException("Cannot register for inactive event");
        
        if (_currentParticipants + count > _maxParticipants)
            throw new DomainException($"Not enough spots available. Only {AvailableSpots()} spots remaining");
        
        _currentParticipants += count;
    }
    
    public void CancelRegistrations(int count)
    {
        if (count <= 0)
            throw new DomainException("Participant count must be greater than 0");
        
        if (_currentParticipants - count < 0)
            throw new DomainException("Cannot cancel more participants than registered");
        
        _currentParticipants -= count;
    }
    
    public void Deactivate() => _isActive = false;
    
    public void Activate()
    {
        if (_eventDate < DateTime.UtcNow.Date)
            throw new DomainException("Cannot activate past events");
        _isActive = true;
    }

    public void Update(string title, string description, DateTime eventDate, int maxParticipants, string? eventLink = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");

        if (title.Length > 200)
            throw new DomainException("Title cannot exceed 200 characters");

        if (eventDate < DateTime.UtcNow.Date)
            throw new DomainException("Event date cannot be in the past");

        if (maxParticipants <= 0)
            throw new DomainException("Max participants must be greater than 0");

        if (maxParticipants > 1000)
            throw new DomainException("Max participants cannot exceed 1000");

        if (maxParticipants < _currentParticipants)
            throw new DomainException($"Cannot reduce max participants below current registrations ({_currentParticipants})");

        _title = title.Trim();
        _description = description?.Trim() ?? string.Empty;
        _eventDate = eventDate;
        _maxParticipants = maxParticipants;
        _eventLink = eventLink?.Trim() ?? string.Empty;
    }
}
