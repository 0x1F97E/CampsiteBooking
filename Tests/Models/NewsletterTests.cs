using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class NewsletterTests
{
    private Newsletter CreateValidNewsletter(string subject = "Summer Newsletter")
    {
        return Newsletter.Create(subject, "Content here", DateTime.UtcNow.AddDays(1));
    }

    [Fact]
    public void Newsletter_CanBeCreated_WithValidData()
    {
        var newsletter = CreateValidNewsletter();
        Assert.NotNull(newsletter);
        Assert.Equal("Summer Newsletter", newsletter.Subject);
        Assert.Equal("Draft", newsletter.Status);
    }

    [Fact]
    public void Newsletter_Create_ThrowsException_WhenSubjectIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidNewsletter(subject: ""));
    }

    [Fact]
    public void Newsletter_Create_ThrowsException_WhenSubjectIsTooLong()
    {
        var longSubject = new string('a', 201);
        Assert.Throws<DomainException>(() => CreateValidNewsletter(subject: longSubject));
    }

    [Fact]
    public void Newsletter_Schedule_ChangesStatusToScheduled()
    {
        var newsletter = CreateValidNewsletter();
        newsletter.Schedule(DateTime.UtcNow.AddDays(5));
        Assert.Equal("Scheduled", newsletter.Status);
    }

    [Fact]
    public void Newsletter_Send_ChangesStatusToSent()
    {
        var newsletter = CreateValidNewsletter();
        newsletter.Send(100);
        Assert.Equal("Sent", newsletter.Status);
        Assert.Equal(100, newsletter.RecipientCount);
        Assert.NotNull(newsletter.SentDate);
    }

    [Fact]
    public void Newsletter_Send_ThrowsException_WhenAlreadySent()
    {
        var newsletter = CreateValidNewsletter();
        newsletter.Send(100);
        Assert.Throws<DomainException>(() => newsletter.Send(50));
    }

    [Fact]
    public void Newsletter_UpdateContent_UpdatesSubjectAndContent()
    {
        var newsletter = CreateValidNewsletter();
        newsletter.UpdateContent("New Subject", "New Content");
        Assert.Equal("New Subject", newsletter.Subject);
        Assert.Equal("New Content", newsletter.Content);
    }

    [Fact]
    public void Newsletter_UpdateContent_ThrowsException_WhenSent()
    {
        var newsletter = CreateValidNewsletter();
        newsletter.Send(100);
        Assert.Throws<DomainException>(() => newsletter.UpdateContent("New", "Content"));
    }

    [Fact]
    public void Newsletter_Cancel_ChangesStatusToCancelled()
    {
        var newsletter = CreateValidNewsletter();
        newsletter.Cancel();
        Assert.Equal("Cancelled", newsletter.Status);
    }
}
