using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class GuestTests
{
    // Helper method to create a valid guest for testing
    private Guest CreateValidGuest(
        string email = "guest@example.com",
        string firstName = "Jane",
        string lastName = "Smith",
        string phone = "",
        string country = "",
        string preferredCommunication = "Email")
    {
        return Guest.Create(
            Email.Create(email),
            firstName,
            lastName,
            phone,
            country,
            preferredCommunication
        );
    }

    [Fact]
    public void Guest_InheritsFromUser_Correctly()
    {
        // Arrange & Act
        var guest = CreateValidGuest();
        
        // Assert
        Assert.IsAssignableFrom<User>(guest);
        Assert.Equal("guest@example.com", guest.Email.Value);
        Assert.Equal("Jane", guest.FirstName);
        Assert.Equal("Jane Smith", guest.FullName);
    }
    
    [Fact]
    public void Guest_CanAccumulateLoyaltyPoints()
    {
        // Arrange
        var guest = CreateValidGuest();
        
        // Act
        guest.AddLoyaltyPoints(50);
        guest.AddLoyaltyPoints(30);
        
        // Assert
        Assert.Equal(80, guest.LoyaltyPoints);
    }
    
    [Fact]
    public void Guest_AddLoyaltyPoints_ThrowsException_WhenNegative()
    {
        // Arrange
        var guest = CreateValidGuest();
        
        // Act & Assert
        Assert.Throws<DomainException>(() => guest.AddLoyaltyPoints(-10));
    }
    
    [Fact]
    public void Guest_CanRedeemLoyaltyPoints()
    {
        // Arrange
        var guest = CreateValidGuest();
        guest.AddLoyaltyPoints(100);
        
        // Act
        guest.RedeemLoyaltyPoints(30);
        
        // Assert
        Assert.Equal(70, guest.LoyaltyPoints);
    }
    
    [Fact]
    public void Guest_RedeemLoyaltyPoints_ThrowsException_WhenInsufficientPoints()
    {
        // Arrange
        var guest = CreateValidGuest();
        guest.AddLoyaltyPoints(50);
        
        // Act & Assert
        Assert.Throws<DomainException>(() => guest.RedeemLoyaltyPoints(100));
    }
    
    [Fact]
    public void Guest_RedeemLoyaltyPoints_ThrowsException_WhenNegative()
    {
        // Arrange
        var guest = CreateValidGuest();
        guest.AddLoyaltyPoints(100);
        
        // Act & Assert
        Assert.Throws<DomainException>(() => guest.RedeemLoyaltyPoints(-10));
    }
    
    [Theory]
    [InlineData("Email")]
    [InlineData("SMS")]
    [InlineData("Both")]
    public void Guest_Create_AcceptsValidPreferredCommunication(string communication)
    {
        // Arrange & Act
        var guest = CreateValidGuest(preferredCommunication: communication);
        
        // Assert
        Assert.Equal(communication, guest.PreferredCommunication);
    }
    
    [Fact]
    public void Guest_Create_ThrowsException_WhenInvalidPreferredCommunication()
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => CreateValidGuest(preferredCommunication: "Invalid"));
    }
    
    [Fact]
    public void Guest_UpdatePreferredCommunication_UpdatesValue()
    {
        // Arrange
        var guest = CreateValidGuest();
        
        // Act
        guest.UpdatePreferredCommunication("SMS");
        
        // Assert
        Assert.Equal("SMS", guest.PreferredCommunication);
    }
    
    [Fact]
    public void Guest_UpdatePreferredCommunication_ThrowsException_WhenInvalid()
    {
        // Arrange
        var guest = CreateValidGuest();
        
        // Act & Assert
        Assert.Throws<DomainException>(() => guest.UpdatePreferredCommunication("Invalid"));
    }
    
    [Fact]
    public void Guest_LoyaltyPoints_StartsAtZero()
    {
        // Arrange & Act
        var guest = CreateValidGuest();
        
        // Assert
        Assert.Equal(0, guest.LoyaltyPoints);
    }
}

