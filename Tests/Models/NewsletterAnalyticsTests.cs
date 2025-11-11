using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class NewsletterAnalyticsTests
{
    private NewsletterAnalytics CreateValidAnalytics(int totalSent = 100)
    {
        return NewsletterAnalytics.Create(NewsletterId.Create(1), totalSent);
    }

    [Fact]
    public void NewsletterAnalytics_CanBeCreated_WithValidData()
    {
        var analytics = CreateValidAnalytics();
        Assert.NotNull(analytics);
        Assert.Equal(100, analytics.TotalSent);
        Assert.Equal(0, analytics.TotalOpened);
        Assert.Equal(0m, analytics.OpenRate);
    }

    [Fact]
    public void NewsletterAnalytics_Create_ThrowsException_WhenTotalSentIsZero()
    {
        Assert.Throws<DomainException>(() => CreateValidAnalytics(totalSent: 0));
    }

    [Fact]
    public void NewsletterAnalytics_RecordOpen_IncreasesOpenedAndCalculatesRate()
    {
        var analytics = CreateValidAnalytics(totalSent: 100);
        analytics.RecordOpen();
        analytics.RecordOpen();
        Assert.Equal(2, analytics.TotalOpened);
        Assert.Equal(2.0m, analytics.OpenRate);
    }

    [Fact]
    public void NewsletterAnalytics_RecordClick_IncreasesClickedAndCalculatesRate()
    {
        var analytics = CreateValidAnalytics(totalSent: 100);
        analytics.RecordClick();
        analytics.RecordClick();
        analytics.RecordClick();
        Assert.Equal(3, analytics.TotalClicked);
        Assert.Equal(3.0m, analytics.ClickRate);
    }

    [Fact]
    public void NewsletterAnalytics_RecordUnsubscribe_IncreasesUnsubscribed()
    {
        var analytics = CreateValidAnalytics();
        analytics.RecordUnsubscribe();
        Assert.Equal(1, analytics.TotalUnsubscribed);
    }

    [Fact]
    public void NewsletterAnalytics_UpdateMetrics_UpdatesAllMetrics()
    {
        var analytics = CreateValidAnalytics(totalSent: 100);
        analytics.UpdateMetrics(50, 25, 5);
        Assert.Equal(50, analytics.TotalOpened);
        Assert.Equal(25, analytics.TotalClicked);
        Assert.Equal(5, analytics.TotalUnsubscribed);
        Assert.Equal(50.0m, analytics.OpenRate);
        Assert.Equal(25.0m, analytics.ClickRate);
    }

    [Fact]
    public void NewsletterAnalytics_UpdateMetrics_ThrowsException_WhenMetricsAreNegative()
    {
        var analytics = CreateValidAnalytics();
        Assert.Throws<DomainException>(() => analytics.UpdateMetrics(-1, 0, 0));
    }

    [Fact]
    public void NewsletterAnalytics_UpdateMetrics_ThrowsException_WhenMetricsExceedTotalSent()
    {
        var analytics = CreateValidAnalytics(totalSent: 100);
        Assert.Throws<DomainException>(() => analytics.UpdateMetrics(150, 0, 0));
    }
}
