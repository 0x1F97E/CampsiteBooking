namespace CampsiteBooking.Models;

/// <summary>
/// Guest user entity - customers who book accommodations
/// </summary>
public class Guest : User
{
    private int _loyaltyPoints;
    
    public int GuestId { get; set; }
    
    public string PreferredCommunication { get; set; } = "Email"; // Email, SMS, Both
    
    public int LoyaltyPoints
    {
        get => _loyaltyPoints;
        set
        {
            if (value < 0)
                throw new ArgumentException("LoyaltyPoints cannot be negative", nameof(LoyaltyPoints));
            _loyaltyPoints = value;
        }
    }
    
    /// <summary>
    /// Adds loyalty points to the guest account
    /// </summary>
    public void AddLoyaltyPoints(int points)
    {
        if (points < 0)
            throw new ArgumentException("Cannot add negative points", nameof(points));
        
        LoyaltyPoints += points;
    }
    
    /// <summary>
    /// Redeems loyalty points from the guest account
    /// </summary>
    public void RedeemLoyaltyPoints(int points)
    {
        if (points < 0)
            throw new ArgumentException("Cannot redeem negative points", nameof(points));
        
        if (points > LoyaltyPoints)
            throw new InvalidOperationException("Insufficient loyalty points");
        
        LoyaltyPoints -= points;
    }
    
    /// <summary>
    /// Validates preferred communication method
    /// </summary>
    public bool IsValidPreferredCommunication()
    {
        var validOptions = new[] { "Email", "SMS", "Both" };
        return validOptions.Contains(PreferredCommunication);
    }
}

