using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class NewsletterSubscriptionTests
{
    private NewsletterSubscription CreateValidSubscription(string language = "da")
    {
        return NewsletterSubscription.Create(Email.Create("test@example.com"), "John", "Doe", language);
    }

    [Fact]
    public void NewsletterSubscription_CanBeCreated_WithValidData()
    {
        var subscription = CreateValidSubscription();
        Assert.NotNull(subscription);
        Assert.Equal("John", subscription.FirstName);
        Assert.True(subscription.IsActive);
        Assert.Equal("da", subscription.PreferredLanguage);
    }

    [Fact]
    public void NewsletterSubscription_Create_ThrowsException_WhenFirstNameIsEmpty()
    {
        Assert.Throws<DomainException>(() => NewsletterSubscription.Create(Email.Create("test@example.com"), "", "Doe", "da"));
    }

    [Fact]
    public void NewsletterSubscription_Create_ThrowsException_WhenLanguageIsInvalid()
    {
        Assert.Throws<DomainException>(() => CreateValidSubscription(language: "fr"));
    }

    [Theory]
    [InlineData("da")]
    [InlineData("en")]
    [InlineData("de")]
    [InlineData("sv")]
    [InlineData("no")]
    public void NewsletterSubscription_Create_AcceptsValidLanguages(string language)
    {
        var subscription = CreateValidSubscription(language: language);
        Assert.Equal(language, subscription.PreferredLanguage);
    }

    [Fact]
    public void NewsletterSubscription_Unsubscribe_SetsIsActiveToFalse()
    {
        var subscription = CreateValidSubscription();
        subscription.Unsubscribe();
        Assert.False(subscription.IsActive);
        Assert.NotNull(subscription.UnsubscribedDate);
    }

    [Fact]
    public void NewsletterSubscription_Unsubscribe_ThrowsException_WhenAlreadyInactive()
    {
        var subscription = CreateValidSubscription();
        subscription.Unsubscribe();
        Assert.Throws<DomainException>(() => subscription.Unsubscribe());
    }

    [Fact]
    public void NewsletterSubscription_Resubscribe_SetsIsActiveToTrue()
    {
        var subscription = CreateValidSubscription();
        subscription.Unsubscribe();
        subscription.Resubscribe();
        Assert.True(subscription.IsActive);
        Assert.Null(subscription.UnsubscribedDate);
    }

    [Fact]
    public void NewsletterSubscription_UpdatePreferredLanguage_UpdatesLanguage()
    {
        var subscription = CreateValidSubscription(language: "da");
        subscription.UpdatePreferredLanguage("en");
        Assert.Equal("en", subscription.PreferredLanguage);
    }
}
