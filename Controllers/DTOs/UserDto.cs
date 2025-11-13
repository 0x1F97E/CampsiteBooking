namespace CampsiteBooking.Controllers;

/// <summary>
/// Data Transfer Object for User responses.
/// </summary>
public class UserDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public DateTime JoinedDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Request DTO for creating a new user.
/// </summary>
public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Country { get; set; }
}

