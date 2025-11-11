using CampsiteBooking.Models;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class ReviewTests
{
    [Fact]
    public void CampsiteId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var review = new Review();

        // Act
        review.CampsiteId = 5;

        // Assert
        Assert.Equal(5, review.CampsiteId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CampsiteId_InvalidValue_ThrowsArgumentException(int invalidId)
    {
        // Arrange
        var review = new Review();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => review.CampsiteId = invalidId);
    }

    [Fact]
    public void UserId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var review = new Review { CampsiteId = 1 };

        // Act
        review.UserId = 10;

        // Assert
        Assert.Equal(10, review.UserId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UserId_InvalidValue_ThrowsArgumentException(int invalidId)
    {
        // Arrange
        var review = new Review { CampsiteId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => review.UserId = invalidId);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Rating_ValidValue_SetsCorrectly(int validRating)
    {
        // Arrange
        var review = new Review { CampsiteId = 1, UserId = 1 };

        // Act
        review.Rating = validRating;

        // Assert
        Assert.Equal(validRating, review.Rating);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Rating_InvalidValue_ThrowsArgumentException(int invalidRating)
    {
        // Arrange
        var review = new Review { CampsiteId = 1, UserId = 1 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => review.Rating = invalidRating);
    }

    [Fact]
    public void Comment_ValidValue_SetsCorrectly()
    {
        // Arrange
        var review = new Review { CampsiteId = 1, UserId = 1, Rating = 5 };

        // Act
        review.Comment = "Great campsite!";

        // Assert
        Assert.Equal("Great campsite!", review.Comment);
    }

    [Fact]
    public void Comment_ExceedsMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var review = new Review { CampsiteId = 1, UserId = 1, Rating = 5 };
        var longComment = new string('a', 2001);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => review.Comment = longComment);
    }

    [Fact]
    public void Comment_NullValue_SetsEmptyString()
    {
        // Arrange
        var review = new Review { CampsiteId = 1, UserId = 1, Rating = 5 };

        // Act
        review.Comment = null;

        // Assert
        Assert.Equal(string.Empty, review.Comment);
    }

    [Fact]
    public void IsApproved_DefaultValue_IsFalse()
    {
        // Arrange & Act
        var review = new Review { CampsiteId = 1, UserId = 1, Rating = 5 };

        // Assert
        Assert.False(review.IsApproved);
    }

    [Fact]
    public void IsVisible_DefaultValue_IsTrue()
    {
        // Arrange & Act
        var review = new Review { CampsiteId = 1, UserId = 1, Rating = 5 };

        // Assert
        Assert.True(review.IsVisible);
    }

    [Fact]
    public void Approve_SetsIsApprovedToTrue()
    {
        // Arrange
        var review = new Review 
        { 
            CampsiteId = 1, 
            UserId = 1, 
            Rating = 5,
            IsApproved = false
        };

        // Act
        review.Approve();

        // Assert
        Assert.True(review.IsApproved);
    }

    [Fact]
    public void Reject_SetsIsApprovedAndIsVisibleToFalse()
    {
        // Arrange
        var review = new Review
        {
            CampsiteId = 1,
            UserId = 1,
            Rating = 5,
            IsApproved = true,
            IsVisible = true
        };

        // Act
        review.Reject();

        // Assert
        Assert.False(review.IsApproved);
        Assert.False(review.IsVisible);
    }

    [Fact]
    public void Hide_SetsIsVisibleToFalse()
    {
        // Arrange
        var review = new Review
        {
            CampsiteId = 1,
            UserId = 1,
            Rating = 5,
            IsVisible = true
        };

        // Act
        review.Hide();

        // Assert
        Assert.False(review.IsVisible);
    }

    [Fact]
    public void Show_SetsIsVisibleToTrue()
    {
        // Arrange
        var review = new Review
        {
            CampsiteId = 1,
            UserId = 1,
            Rating = 5,
            IsVisible = false
        };

        // Act
        review.Show();

        // Assert
        Assert.True(review.IsVisible);
    }

    [Fact]
    public void UpdateReview_ValidData_UpdatesRatingAndComment()
    {
        // Arrange
        var review = new Review
        {
            CampsiteId = 1,
            UserId = 1,
            Rating = 3,
            Comment = "OK"
        };

        // Act
        review.UpdateReview(5, "Excellent campsite!");

        // Assert
        Assert.Equal(5, review.Rating);
        Assert.Equal("Excellent campsite!", review.Comment);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void UpdateReview_InvalidRating_ThrowsArgumentException(int invalidRating)
    {
        // Arrange
        var review = new Review
        {
            CampsiteId = 1,
            UserId = 1,
            Rating = 3
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => review.UpdateReview(invalidRating, "Comment"));
    }

    [Fact]
    public void UpdateReview_CommentTooLong_ThrowsArgumentException()
    {
        // Arrange
        var review = new Review
        {
            CampsiteId = 1,
            UserId = 1,
            Rating = 3
        };
        var longComment = new string('a', 2001);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => review.UpdateReview(5, longComment));
    }

    [Fact]
    public void AddAdminResponse_ValidResponse_SetsResponseAndDate()
    {
        // Arrange
        var review = new Review
        {
            CampsiteId = 1,
            UserId = 1,
            Rating = 5
        };

        // Act
        review.AddAdminResponse("Thank you for your feedback!");

        // Assert
        Assert.Equal("Thank you for your feedback!", review.AdminResponse);
        Assert.NotNull(review.AdminResponseDate);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddAdminResponse_EmptyResponse_ThrowsArgumentException(string invalidResponse)
    {
        // Arrange
        var review = new Review
        {
            CampsiteId = 1,
            UserId = 1,
            Rating = 5
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => review.AddAdminResponse(invalidResponse));
    }

    [Fact]
    public void AddAdminResponse_ResponseTooLong_ThrowsArgumentException()
    {
        // Arrange
        var review = new Review
        {
            CampsiteId = 1,
            UserId = 1,
            Rating = 5
        };
        var longResponse = new string('a', 1001);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => review.AddAdminResponse(longResponse));
    }
}

