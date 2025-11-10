using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class GuestTests
{
    [Fact]
    public void Guest_InheritsFromUser_Correctly()
    {
        // Arrange & Act
        var guest = new Guest
        {
            Email = "guest@example.com",
            FirstName = "Jane",
            LastName = "Smith"
        };
        
        // Assert
        Assert.IsAssignableFrom<User>(guest);
        Assert.Equal("guest@example.com", guest.Email);
        Assert.Equal("Jane", guest.FirstName);
        Assert.Equal("Jane Smith", guest.FullName);
    }
    
    [Fact]
    public void Guest_LoyaltyPoints_CannotBeNegative()
    {
        // Arrange
        var guest = new Guest
        {
            Email = "guest@example.com",
            FirstName = "Jane",
            LastName = "Smith"
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => guest.LoyaltyPoints = -10);
    }
    
    [Fact]
    public void Guest_CanAccumulateLoyaltyPoints()
    {
        // Arrange
        var guest = new Guest
        {
            Email = "guest@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            LoyaltyPoints = 100
        };
        
        // Act
        guest.AddLoyaltyPoints(50);
        
        // Assert
        Assert.Equal(150, guest.LoyaltyPoints);
    }
    
    [Fact]
    public void Guest_AddLoyaltyPoints_ThrowsException_WhenNegative()
    {
        // Arrange
        var guest = new Guest
        {
            Email = "guest@example.com",
            FirstName = "Jane",
            LastName = "Smith"
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => guest.AddLoyaltyPoints(-10));
    }
    
    [Fact]
    public void Guest_CanRedeemLoyaltyPoints()
    {
        // Arrange
        var guest = new Guest
        {
            Email = "guest@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            LoyaltyPoints = 100
        };
        
        // Act
        guest.RedeemLoyaltyPoints(30);
        
        // Assert
        Assert.Equal(70, guest.LoyaltyPoints);
    }
    
    [Fact]
    public void Guest_RedeemLoyaltyPoints_ThrowsException_WhenInsufficientPoints()
    {
        // Arrange
        var guest = new Guest
        {
            Email = "guest@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            LoyaltyPoints = 50
        };
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => guest.RedeemLoyaltyPoints(100));
    }
    
    [Fact]
    public void Guest_RedeemLoyaltyPoints_ThrowsException_WhenNegative()
    {
        // Arrange
        var guest = new Guest
        {
            Email = "guest@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            LoyaltyPoints = 100
        };
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => guest.RedeemLoyaltyPoints(-10));
    }
    
    [Theory]
    [InlineData("Email", true)]
    [InlineData("SMS", true)]
    [InlineData("Both", true)]
    [InlineData("Invalid", false)]
    public void Guest_PreferredCommunication_ValidatesCorrectly(string communication, bool expectedValid)
    {
        // Arrange
        var guest = new Guest
        {
            Email = "guest@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            PreferredCommunication = communication
        };
        
        // Act
        var isValid = guest.IsValidPreferredCommunication();
        
        // Assert
        Assert.Equal(expectedValid, isValid);
    }
}

