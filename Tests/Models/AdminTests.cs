using CampsiteBooking.Models;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class AdminTests
{
    [Fact]
    public void Admin_InheritsFromUser()
    {
        // Arrange & Act
        var admin = new Admin
        {
            Email = Email.Create("admin@campsite.com"),
            FirstName = "John",
            LastName = "Admin"
        };

        // Assert
        Assert.IsAssignableFrom<User>(admin);
        Assert.Equal("admin@campsite.com", admin.Email?.Value);
        Assert.Equal("John Admin", admin.FullName);
    }

    [Fact]
    public void AddPermission_ValidPermission_AddsToList()
    {
        // Arrange
        var admin = new Admin
        {
            Email = Email.Create("admin@campsite.com"),
            FirstName = "John",
            LastName = "Admin"
        };

        // Act
        admin.AddPermission("ManageUsers");

        // Assert
        Assert.Single(admin.Permissions);
        Assert.Contains("ManageUsers", admin.Permissions);
    }

    [Fact]
    public void AddPermission_EmptyPermission_ThrowsArgumentException()
    {
        // Arrange
        var admin = new Admin
        {
            Email = Email.Create("admin@campsite.com"),
            FirstName = "John",
            LastName = "Admin"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => admin.AddPermission(""));
        Assert.Throws<ArgumentException>(() => admin.AddPermission("   "));
    }

    [Fact]
    public void AddPermission_DuplicatePermission_ThrowsInvalidOperationException()
    {
        // Arrange
        var admin = new Admin
        {
            Email = Email.Create("admin@campsite.com"),
            FirstName = "John",
            LastName = "Admin"
        };
        admin.AddPermission("ManageUsers");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => admin.AddPermission("ManageUsers"));
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void RemovePermission_ExistingPermission_RemovesFromList()
    {
        // Arrange
        var admin = new Admin
        {
            Email = Email.Create("admin@campsite.com"),
            FirstName = "John",
            LastName = "Admin"
        };
        admin.AddPermission("ManageUsers");
        admin.AddPermission("ManageBookings");

        // Act
        admin.RemovePermission("ManageUsers");

        // Assert
        Assert.Single(admin.Permissions);
        Assert.DoesNotContain("ManageUsers", admin.Permissions);
        Assert.Contains("ManageBookings", admin.Permissions);
    }

    [Fact]
    public void RemovePermission_NonExistingPermission_ThrowsInvalidOperationException()
    {
        // Arrange
        var admin = new Admin
        {
            Email = Email.Create("admin@campsite.com"),
            FirstName = "John",
            LastName = "Admin"
        };

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => admin.RemovePermission("ManageUsers"));
        Assert.Contains("does not exist", exception.Message);
    }

    [Fact]
    public void RemovePermission_EmptyPermission_ThrowsArgumentException()
    {
        // Arrange
        var admin = new Admin
        {
            Email = Email.Create("admin@campsite.com"),
            FirstName = "John",
            LastName = "Admin"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => admin.RemovePermission(""));
        Assert.Throws<ArgumentException>(() => admin.RemovePermission("   "));
    }

    [Fact]
    public void HasPermission_ExistingPermission_ReturnsTrue()
    {
        // Arrange
        var admin = new Admin
        {
            Email = Email.Create("admin@campsite.com"),
            FirstName = "John",
            LastName = "Admin"
        };
        admin.AddPermission("ManageUsers");

        // Act
        var result = admin.HasPermission("ManageUsers");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasPermission_NonExistingPermission_ReturnsFalse()
    {
        // Arrange
        var admin = new Admin
        {
            Email = Email.Create("admin@campsite.com"),
            FirstName = "John",
            LastName = "Admin"
        };

        // Act
        var result = admin.HasPermission("ManageUsers");

        // Assert
        Assert.False(result);
    }
}

