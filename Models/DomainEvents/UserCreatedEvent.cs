namespace CampsiteBooking.Models.DomainEvents;

/// <summary>
/// Domain event raised when a new user is created
/// </summary>
public class UserCreatedEvent : IDomainEvent
{
    public int UserId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; }

    public UserCreatedEvent(int userId, string email)
    {
        UserId = userId;
        Email = email;
        OccurredOn = DateTime.UtcNow;
    }
}

