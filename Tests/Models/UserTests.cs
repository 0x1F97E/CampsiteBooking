using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using CampsiteBooking.Models.DomainEvents;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class UserTests
{
    // Helper method to create a valid user for testing
    private User CreateValidUser(
        string email = "test@example.com",
        string firstName = "John",
        string lastName = "Doe",
        string phone = "",
        string country = "")
    {
        return User.Create(
            Email.Create(email),
            firstName,
            lastName,
            phone,
            country
        );
    }

    [Fact]
    public void User_CanBeCreated_WithValidData()
    {
        // Arrange & Act
        var user = CreateValidUser();

        // Assert
        Assert.NotNull(user);
        Assert.Equal("test@example.com", user.Email.Value);
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.True(user.IsActive);
        Assert.Contains(user.DomainEvents, e => e is UserCreatedEvent);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void User_Create_ThrowsException_WhenFirstNameIsEmpty(string invalidName)
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act & Assert
        Assert.Throws<DomainException>(() => User.Create(email, invalidName, "Doe"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void User_Create_ThrowsException_WhenLastNameIsEmpty(string invalidName)
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act & Assert
        Assert.Throws<DomainException>(() => User.Create(email, "John", invalidName));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void User_Email_CannotBeEmpty(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missingat.com")]
    [InlineData("@nodomain.com")]
    public void User_Email_MustBeInValidFormat(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
    }

    [Fact]
    public void User_IsActive_DefaultsToTrue()
    {
        // Arrange & Act
        var user = CreateValidUser();

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_UpdateLastLogin_SetsLastLoginTimestamp()
    {
        // Arrange
        var user = CreateValidUser();
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
        var user = CreateValidUser();

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
        var user = CreateValidUser();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(user.JoinedDate >= beforeCreation);
        Assert.True(user.JoinedDate <= afterCreation);
    }

    [Fact]
    public void User_Deactivate_ChangesIsActiveToFalse()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        user.Deactivate();

        // Assert
        Assert.False(user.IsActive);
        Assert.Contains(user.DomainEvents, e => e is UserDeactivatedEvent);
    }

    [Fact]
    public void User_Deactivate_ThrowsException_WhenAlreadyDeactivated()
    {
        // Arrange
        var user = CreateValidUser();
        user.Deactivate();

        // Act & Assert
        Assert.Throws<DomainException>(() => user.Deactivate());
    }

    [Fact]
    public void User_Activate_ChangesIsActiveToTrue()
    {
        // Arrange
        var user = CreateValidUser();
        user.Deactivate();

        // Act
        user.Activate();

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void User_UpdateContactInfo_UpdatesPhoneAndCountry()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        user.UpdateContactInfo("+45 12345678", "Denmark");

        // Assert
        Assert.Equal("+45 12345678", user.Phone);
        Assert.Equal("Denmark", user.Country);
    }

    [Fact]
    public void User_UpdateName_UpdatesFirstNameAndLastName()
    {
        // Arrange
        var user = CreateValidUser();

        // Act
        user.UpdateName("Jane", "Smith");

        // Assert
        Assert.Equal("Jane", user.FirstName);
        Assert.Equal("Smith", user.LastName);
        Assert.Equal("Jane Smith", user.FullName);
    }

    [Fact]
    public void User_UpdateName_ThrowsException_WhenFirstNameIsEmpty()
    {
        // Arrange
        var user = CreateValidUser();

        // Act & Assert
        Assert.Throws<DomainException>(() => user.UpdateName("", "Smith"));
    }
}

