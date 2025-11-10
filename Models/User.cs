namespace CampsiteBooking.Models;

/// <summary>
/// Base user entity representing any user in the system
/// </summary>
public class User
{
    public int UserId { get; set; }
    
    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty", nameof(Email));
            
            if (!IsValidEmail(value))
                throw new ArgumentException("Email must be in valid format", nameof(Email));
            
            _email = value;
        }
    }
    
    private string _phone = string.Empty;
    public string Phone
    {
        get => _phone;
        set => _phone = value ?? string.Empty;
    }
    
    private string _firstName = string.Empty;
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("FirstName cannot be empty", nameof(FirstName));
            _firstName = value;
        }
    }
    
    private string _lastName = string.Empty;
    public string LastName
    {
        get => _lastName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("LastName cannot be empty", nameof(LastName));
            _lastName = value;
        }
    }
    
    public string Country { get; set; } = string.Empty;
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets the full name of the user
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
    
    /// <summary>
    /// Updates the last login timestamp
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLogin = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Validates email format
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        
        return email.Contains('@') && email.Contains('.');
    }
}

