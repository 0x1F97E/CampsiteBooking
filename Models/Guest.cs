using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// Guest user entity - customers who book accommodations
/// </summary>
public class Guest : User
{
    private int _loyaltyPoints;
    private string _preferredCommunication = "Email";

    public int GuestId => UserId; // Guest uses same ID as User

    public string PreferredCommunication => _preferredCommunication;

    public int LoyaltyPoints => _loyaltyPoints;

    // ============================================================================
    // FACTORY METHOD
    // ============================================================================

    public static new Guest Create(
        Email email,
        string firstName,
        string lastName,
        string phone = "",
        string country = "",
        string preferredCommunication = "Email")
    {
        // Validate preferred communication
        var validOptions = new[] { "Email", "SMS", "Both" };
        if (!validOptions.Contains(preferredCommunication))
            throw new DomainException("Preferred communication must be Email, SMS, or Both");

        var guest = new Guest(email, firstName, lastName, phone, country)
        {
            _loyaltyPoints = 0,
            _preferredCommunication = preferredCommunication
        };

        return guest;
    }

    // ============================================================================
    // CONSTRUCTORS
    // ============================================================================

    /// <summary>
    /// Protected constructor for EF Core
    /// </summary>
    private Guest()
    {
    }

    /// <summary>
    /// Internal constructor using base User constructor
    /// </summary>
    private Guest(Email email, string firstName, string lastName, string phone, string country)
        : base(email, firstName, lastName, phone, country)
    {
    }

    // ============================================================================
    // BUSINESS METHODS
    // ============================================================================

    /// <summary>
    /// Adds loyalty points to the guest account
    /// </summary>
    public void AddLoyaltyPoints(int points)
    {
        if (points < 0)
            throw new DomainException("Cannot add negative points");

        _loyaltyPoints += points;
    }

    /// <summary>
    /// Redeems loyalty points from the guest account
    /// </summary>
    public void RedeemLoyaltyPoints(int points)
    {
        if (points < 0)
            throw new DomainException("Cannot redeem negative points");

        if (points > _loyaltyPoints)
            throw new DomainException("Insufficient loyalty points");

        _loyaltyPoints -= points;
    }

    /// <summary>
    /// Update preferred communication method
    /// </summary>
    public void UpdatePreferredCommunication(string preferredCommunication)
    {
        var validOptions = new[] { "Email", "SMS", "Both" };
        if (!validOptions.Contains(preferredCommunication))
            throw new DomainException("Preferred communication must be Email, SMS, or Both");

        _preferredCommunication = preferredCommunication;
    }
}

