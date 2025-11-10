using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class UserTests
{
    [Fact]
    public void User_CanBeCreated_WithValidEmail()
    {
        // Arrange & Act
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        
        // Assert
        Assert.Equal("test@example.com", user.Email);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void User_Email_CannotBeEmpty(string invalidEmail)
    {
        // Arrange
        var user = new User();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.Email = invalidEmail);
    }
    
    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@domain")]
    [InlineData("missingat.com")]
    public void User_Email_MustBeInValidFormat(string invalidEmail)
    {
        // Arrange
        var user = new User();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.Email = invalidEmail);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void User_FirstName_CannotBeEmpty(string invalidName)
    {
        // Arrange
        var user = new User();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.FirstName = invalidName);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void User_LastName_CannotBeEmpty(string invalidName)
    {
        // Arrange
        var user = new User();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.LastName = invalidName);
    }
    
    [Fact]
    public void User_IsActive_DefaultsToTrue()
    {
        // Arrange & Act
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        
        // Assert
        Assert.True(user.IsActive);
    }
    
    [Fact]
    public void User_UpdateLastLogin_SetsLastLoginTimestamp()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var beforeUpdate = DateTime.UtcNow;
        
        // Act
        user.UpdateLastLogin();
        var afterUpdate = DateTime.UtcNow;
        
        // Assert
        Assert.NotNull(user.LastLogin);
        Assert.True(user.LastLogin >= beforeUpdate);
        Assert.True(user.LastLogin <= afterUpdate);
    }
    
    [Fact]
    public void User_FullName_ReturnsFirstNameAndLastName()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        
        // Act
        var fullName = user.FullName;
        
        // Assert
        Assert.Equal("John Doe", fullName);
    }
    
    [Fact]
    public void User_JoinedDate_IsSetAutomatically()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;
        
        // Act
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var afterCreation = DateTime.UtcNow;
        
        // Assert
        Assert.True(user.JoinedDate >= beforeCreation);
        Assert.True(user.JoinedDate <= afterCreation);
    }
}

