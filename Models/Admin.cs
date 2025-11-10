namespace CampsiteBooking.Models;

/// <summary>
/// Admin user entity - full system access
/// </summary>
public class Admin : User
{
    public int AdminId { get; set; }
    public List<string> Permissions { get; set; } = new();
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Adds a permission to the admin
    /// </summary>
    public void AddPermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentException("Permission cannot be empty", nameof(permission));
        
        if (Permissions.Contains(permission))
            throw new InvalidOperationException($"Permission '{permission}' already exists");
        
        Permissions.Add(permission);
    }
    
    /// <summary>
    /// Removes a permission from the admin
    /// </summary>
    public void RemovePermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission))
            throw new ArgumentException("Permission cannot be empty", nameof(permission));
        
        if (!Permissions.Contains(permission))
            throw new InvalidOperationException($"Permission '{permission}' does not exist");
        
        Permissions.Remove(permission);
    }
    
    /// <summary>
    /// Checks if admin has a specific permission
    /// </summary>
    public bool HasPermission(string permission)
    {
        return Permissions.Contains(permission);
    }
}

