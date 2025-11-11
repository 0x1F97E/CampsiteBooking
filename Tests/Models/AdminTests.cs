using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class AdminTests
{
    // Helper method to create a valid admin for testing
    private Admin CreateValidAdmin(
        string email = "admin@campsite.com",
        string firstName = "John",
        string lastName = "Admin",
        string phone = "",
        string country = "")
    {
        return Admin.Create(
            Email.Create(email),
            firstName,
            lastName,
            phone,
            country
        );
    }

    [Fact]
    public void Admin_InheritsFromUser()
    {
        // Arrange & Act
        var admin = CreateValidAdmin();

        // Assert
        Assert.IsAssignableFrom<User>(admin);
        Assert.Equal("admin@campsite.com", admin.Email.Value);
        Assert.Equal("John Admin", admin.FullName);
    }

    [Fact]
    public void AddPermission_ValidPermission_AddsToList()
    {
        // Arrange
        var admin = CreateValidAdmin();

        // Act
        admin.AddPermission("ManageUsers");

        // Assert
        Assert.Single(admin.Permissions);
        Assert.Contains("ManageUsers", admin.Permissions);
    }

    [Fact]
    public void AddPermission_EmptyPermission_ThrowsException()
    {
        // Arrange
        var admin = CreateValidAdmin();

        // Act & Assert
        Assert.Throws<DomainException>(() => admin.AddPermission(""));
        Assert.Throws<DomainException>(() => admin.AddPermission("   "));
    }

    [Fact]
    public void AddPermission_DuplicatePermission_ThrowsException()
    {
        // Arrange
        var admin = CreateValidAdmin();
        admin.AddPermission("ManageUsers");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => admin.AddPermission("ManageUsers"));
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void RemovePermission_ExistingPermission_RemovesFromList()
    {
        // Arrange
        var admin = CreateValidAdmin();
        admin.AddPermission("ManageUsers");

        // Act
        admin.RemovePermission("ManageUsers");

        // Assert
        Assert.Empty(admin.Permissions);
    }

    [Fact]
    public void RemovePermission_NonExistingPermission_ThrowsException()
    {
        // Arrange
        var admin = CreateValidAdmin();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => admin.RemovePermission("ManageUsers"));
        Assert.Contains("does not exist", exception.Message);
    }

    [Fact]
    public void RemovePermission_EmptyPermission_ThrowsException()
    {
        // Arrange
        var admin = CreateValidAdmin();

        // Act & Assert
        Assert.Throws<DomainException>(() => admin.RemovePermission(""));
        Assert.Throws<DomainException>(() => admin.RemovePermission("   "));
    }

    [Fact]
    public void HasPermission_ExistingPermission_ReturnsTrue()
    {
        // Arrange
        var admin = CreateValidAdmin();
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
        var admin = CreateValidAdmin();

        // Act
        var result = admin.HasPermission("ManageUsers");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Admin_CreatedDate_IsSetAutomatically()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var admin = CreateValidAdmin();
        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(admin.CreatedDate >= beforeCreation);
        Assert.True(admin.CreatedDate <= afterCreation);
    }
}

