using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using CampsiteBooking.Models.DomainEvents;

namespace CampsiteBooking.Models;

/// <summary>
/// User aggregate root representing any user in the system.
/// Base class for Guest, Admin, and Staff.
/// </summary>
public class User : AggregateRoot<UserId>
{
    // ============================================================================
    // PRIVATE FIELDS - Encapsulation
    // ============================================================================

    private Email _email = null!;
    private string _passwordHash = string.Empty; // ASP.NET Core Identity password hash
    private string _phone = string.Empty;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _country = string.Empty;
    private DateTime _joinedDate;
    private DateTime? _lastLogin;
    private bool _isActive;

    // ============================================================================
    // PUBLIC PROPERTIES - Read-only access
    // ============================================================================
    
    /// <summary>Legacy property for EF Core and backward compatibility</summary>
    public int UserId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.UserId.Create(value) : ValueObjects.UserId.CreateNew();
    }

    public Email Email => _email;
    public string PasswordHash => _passwordHash; // For EF Core
    public string Phone => _phone;
    public string FirstName => _firstName;
    public string LastName => _lastName;
    public string Country => _country;
    public DateTime JoinedDate => _joinedDate;
    public DateTime? LastLogin => _lastLogin;
    public bool IsActive => _isActive;

    /// <summary>Gets the full name of the user</summary>
    public string FullName => $"{_firstName} {_lastName}";

    // ============================================================================
    // FACTORY METHODS - Controlled creation
    // ============================================================================
    
    /// <summary>
    /// Create a new user (factory method)
    /// </summary>
    public static User Create(
        Email email,
        string firstName,
        string lastName,
        string phone = "",
        string country = "")
    {
        // Validate business rules
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty");

        if (firstName.Length > 100)
            throw new DomainException("First name cannot exceed 100 characters");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty");

        if (lastName.Length > 100)
            throw new DomainException("Last name cannot exceed 100 characters");

        var user = new User
        {
            Id = ValueObjects.UserId.CreateNew(),
            _email = email,
            _firstName = firstName.Trim(),
            _lastName = lastName.Trim(),
            _phone = phone?.Trim() ?? string.Empty,
            _country = country?.Trim() ?? string.Empty,
            _joinedDate = DateTime.UtcNow,
            _isActive = true
        };

        // Raise domain event
        user.RaiseDomainEvent(new UserCreatedEvent(user.Id.Value, email.Value));

        return user;
    }

    // ============================================================================
    // CONSTRUCTORS
    // ============================================================================

    /// <summary>
    /// Protected constructor for EF Core and derived classes
    /// </summary>
    protected User()
    {
    }

    /// <summary>
    /// Internal constructor for derived classes (Guest, Admin, Staff)
    /// </summary>
    internal User(Email email, string firstName, string lastName, string phone, string country)
    {
        // Validate business rules
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty");

        if (firstName.Length > 100)
            throw new DomainException("First name cannot exceed 100 characters");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty");

        if (lastName.Length > 100)
            throw new DomainException("Last name cannot exceed 100 characters");

        Id = ValueObjects.UserId.CreateNew();
        _email = email;
        _firstName = firstName.Trim();
        _lastName = lastName.Trim();
        _phone = phone?.Trim() ?? string.Empty;
        _country = country?.Trim() ?? string.Empty;
        _joinedDate = DateTime.UtcNow;
        _isActive = true;
    }

    // ============================================================================
    // BUSINESS METHODS - Rich behavior
    // ============================================================================
    
    /// <summary>
    /// Update the last login timestamp
    /// </summary>
    public void UpdateLastLogin()
    {
        _lastLogin = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivate the user account
    /// </summary>
    public void Deactivate()
    {
        if (!_isActive)
            throw new DomainException("User is already deactivated");

        _isActive = false;
        RaiseDomainEvent(new UserDeactivatedEvent(Id.Value, _email.Value));
    }

    /// <summary>
    /// Activate the user account
    /// </summary>
    public void Activate()
    {
        if (_isActive)
            throw new DomainException("User is already active");

        _isActive = true;
    }

    /// <summary>
    /// Update user contact information
    /// </summary>
    public void UpdateContactInfo(string phone, string country)
    {
        _phone = phone?.Trim() ?? string.Empty;
        _country = country?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Update user name
    /// </summary>
    public void UpdateName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty");

        _firstName = firstName.Trim();
        _lastName = lastName.Trim();
    }

    /// <summary>
    /// Set password hash (used by ASP.NET Core Identity)
    /// </summary>
    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash cannot be empty");

        _passwordHash = passwordHash;
    }
}

