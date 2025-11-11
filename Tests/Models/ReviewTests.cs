using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class ReviewTests
{
    private Review CreateValidReview(int rating = 5, string comment = "Great campsite!")
    {
        return Review.Create(CampsiteId.Create(1), UserId.Create(1), rating, comment, "John Doe");
    }

    [Fact]
    public void Review_CanBeCreated_WithValidData()
    {
        var review = CreateValidReview();
        Assert.NotNull(review);
        Assert.Equal(5, review.Rating);
        Assert.Equal("Great campsite!", review.Comment);
        Assert.Equal("John Doe", review.ReviewerName);
        Assert.False(review.IsApproved);
        Assert.True(review.IsVisible);
    }

    [Fact]
    public void Review_Create_ThrowsException_WhenRatingIsZero()
    {
        Assert.Throws<DomainException>(() => CreateValidReview(rating: 0));
    }

    [Fact]
    public void Review_Create_ThrowsException_WhenRatingIsAbove5()
    {
        Assert.Throws<DomainException>(() => CreateValidReview(rating: 6));
    }

    [Fact]
    public void Review_Create_ThrowsException_WhenCommentIsTooLong()
    {
        var longComment = new string('a', 2001);
        Assert.Throws<DomainException>(() => CreateValidReview(comment: longComment));
    }

    [Fact]
    public void Review_Approve_SetsIsApprovedToTrue()
    {
        var review = CreateValidReview();
        review.Approve();
        Assert.True(review.IsApproved);
    }

    [Fact]
    public void Review_Reject_SetsIsApprovedAndIsVisibleToFalse()
    {
        var review = CreateValidReview();
        review.Reject();
        Assert.False(review.IsApproved);
        Assert.False(review.IsVisible);
    }

    [Fact]
    public void Review_Hide_SetsIsVisibleToFalse()
    {
        var review = CreateValidReview();
        review.Hide();
        Assert.False(review.IsVisible);
    }

    [Fact]
    public void Review_Show_SetsIsVisibleToTrue()
    {
        var review = CreateValidReview();
        review.Hide();
        review.Show();
        Assert.True(review.IsVisible);
    }

    [Fact]
    public void Review_UpdateReview_UpdatesRatingAndComment()
    {
        var review = CreateValidReview(rating: 5);
        review.UpdateReview(4, "Updated comment");
        Assert.Equal(4, review.Rating);
        Assert.Equal("Updated comment", review.Comment);
    }

    [Fact]
    public void Review_AddAdminResponse_SetsResponse()
    {
        var review = CreateValidReview();
        review.AddAdminResponse("Thank you for your feedback!");
        Assert.Equal("Thank you for your feedback!", review.AdminResponse);
        Assert.NotNull(review.AdminResponseDate);
    }

    [Fact]
    public void Review_AddAdminResponse_ThrowsException_WhenResponseIsEmpty()
    {
        var review = CreateValidReview();
        Assert.Throws<DomainException>(() => review.AddAdminResponse(""));
    }

    [Fact]
    public void Review_AddAdminResponse_ThrowsException_WhenResponseIsTooLong()
    {
        var review = CreateValidReview();
        var longResponse = new string('a', 1001);
        Assert.Throws<DomainException>(() => review.AddAdminResponse(longResponse));
    }
}
