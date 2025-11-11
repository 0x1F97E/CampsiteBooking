using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

public class EventRegistration : Entity<EventRegistrationId>
{
    private ValueObjects.EventId _eventId = null!;
    private UserId _userId = null!;
    private int _numberOfParticipants;
    private string _participantNames = string.Empty;
    private DateTime _registrationDate;
    private string _status = "Confirmed";
    private string _specialRequests = string.Empty;
    
    public ValueObjects.EventId EventId => _eventId;
    public UserId UserId => _userId;
    public int NumberOfParticipants => _numberOfParticipants;
    public string ParticipantNames => _participantNames;
    public DateTime RegistrationDate => _registrationDate;
    public string Status => _status;
    public string SpecialRequests => _specialRequests;
    
    public int EventRegistrationId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.EventRegistrationId.Create(value) : ValueObjects.EventRegistrationId.CreateNew();
    }
    
    public static EventRegistration Create(ValueObjects.EventId eventId, UserId userId, int numberOfParticipants, string participantNames, string specialRequests = "")
    {
        if (numberOfParticipants <= 0)
            throw new DomainException("Number of participants must be greater than 0");
        
        if (numberOfParticipants > 10)
            throw new DomainException("Cannot register more than 10 participants at once");
        
        if (string.IsNullOrWhiteSpace(participantNames))
            throw new DomainException("Participant names cannot be empty");
        
        return new EventRegistration
        {
            Id = ValueObjects.EventRegistrationId.CreateNew(),
            _eventId = eventId,
            _userId = userId,
            _numberOfParticipants = numberOfParticipants,
            _participantNames = participantNames.Trim(),
            _registrationDate = DateTime.UtcNow,
            _status = "Confirmed",
            _specialRequests = specialRequests?.Trim() ?? string.Empty
        };
    }
    
    private EventRegistration() { }
    
    public void Cancel()
    {
        if (_status == "Cancelled")
            throw new DomainException("Registration is already cancelled");
        _status = "Cancelled";
    }
    
    public void Confirm()
    {
        if (_status == "Cancelled")
            throw new DomainException("Cannot confirm a cancelled registration");
        _status = "Confirmed";
    }
    
    public void UpdateSpecialRequests(string specialRequests)
    {
        _specialRequests = specialRequests?.Trim() ?? string.Empty;
    }
}
