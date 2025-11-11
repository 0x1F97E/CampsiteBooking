using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// Admin user entity - full system access
/// </summary>
public class Admin : User
{
    private readonly List<string> _permissions = new();
    private DateTime _createdDate;

    public int AdminId => UserId; // Admin uses same ID as User

    public IReadOnlyList<string> Permissions => _permissions.AsReadOnly();
    public DateTime CreatedDate => _createdDate;

    // ============================================================================
    // FACTORY METHOD
    // ============================================================================

    public static new Admin Create(
        Email email,
        string firstName,
        string lastName,
        string phone = "",
        string country = "")
    {
        var admin = new Admin(email, firstName, lastName, phone, country)
        {
            _createdDate = DateTime.UtcNow
        };

        return admin;
    }

    // ============================================================================
    // CONSTRUCTORS
    // ============================================================================

    /// <summary>
    /// Protected constructor for EF Core
    /// </summary>
    private Admin()
    {
    }

    /// <summary>
    /// Internal constructor using base User constructor
    /// </summary>
    private Admin(Email email, string firstName, string lastName, string phone, string country)
        : base(email, firstName, lastName, phone, country)
    {
    }

    // ============================================================================
    // BUSINESS METHODS
    // ============================================================================

    /// <summary>
    /// Adds a permission to the admin
    /// </summary>
    public void AddPermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new DomainException("Permission cannot be empty");

        if (_permissions.Contains(permission))
            throw new DomainException($"Permission '{permission}' already exists");

        _permissions.Add(permission);
    }

    /// <summary>
    /// Removes a permission from the admin
    /// </summary>
    public void RemovePermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new DomainException("Permission cannot be empty");

        if (!_permissions.Contains(permission))
            throw new DomainException($"Permission '{permission}' does not exist");

        _permissions.Remove(permission);
    }

    /// <summary>
    /// Checks if admin has a specific permission
    /// </summary>
    public bool HasPermission(string permission)
    {
        return _permissions.Contains(permission);
    }
}

