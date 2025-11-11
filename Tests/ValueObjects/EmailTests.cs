using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("first+last@company.org")]
    public void Create_ValidEmail_CreatesEmailObject(string validEmail)
    {
        // Act
        var email = Email.Create(validEmail);

        // Assert
        Assert.Equal(validEmail.ToLowerInvariant(), email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyEmail_ThrowsArgumentException(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user @example.com")]
    [InlineData("user@.com")]
    public void Create_InvalidFormat_ThrowsArgumentException(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
    }

    [Fact]
    public void Create_UppercaseEmail_NormalizesToLowercase()
    {
        // Act
        var email = Email.Create("TEST@EXAMPLE.COM");

        // Assert
        Assert.Equal("test@example.com", email.Value);
    }

    [Fact]
    public void Equals_SameEmail_ReturnsTrue()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");

        // Act & Assert
        Assert.Equal(email1, email2);
        Assert.True(email1 == email2);
    }

    [Fact]
    public void Equals_DifferentEmail_ReturnsFalse()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com");
        var email2 = Email.Create("test2@example.com");

        // Act & Assert
        Assert.NotEqual(email1, email2);
        Assert.True(email1 != email2);
    }

    [Fact]
    public void ToString_ReturnsEmailValue()
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act
        var result = email.ToString();

        // Assert
        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public void ImplicitConversion_ToString_Works()
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act
        string emailString = email;

        // Assert
        Assert.Equal("test@example.com", emailString);
    }
}

