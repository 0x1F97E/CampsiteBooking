using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

public class NewsletterAnalytics : Entity<NewsletterAnalyticsId>
{
    private NewsletterId _newsletterId = null!;
    private int _totalSent;
    private int _totalOpened;
    private int _totalClicked;
    private int _totalUnsubscribed;
    private decimal _openRate;
    private decimal _clickRate;
    private DateTime _recordedDate;
    
    public NewsletterId NewsletterId => _newsletterId;
    public int TotalSent => _totalSent;
    public int TotalOpened => _totalOpened;
    public int TotalClicked => _totalClicked;
    public int TotalUnsubscribed => _totalUnsubscribed;
    public decimal OpenRate => _openRate;
    public decimal ClickRate => _clickRate;
    public DateTime RecordedDate => _recordedDate;
    
    public int NewsletterAnalyticsId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.NewsletterAnalyticsId.Create(value) : ValueObjects.NewsletterAnalyticsId.CreateNew();
    }
    
    public static NewsletterAnalytics Create(NewsletterId newsletterId, int totalSent)
    {
        if (totalSent <= 0)
            throw new DomainException("Total sent must be greater than 0");
        
        return new NewsletterAnalytics
        {
            Id = ValueObjects.NewsletterAnalyticsId.CreateNew(),
            _newsletterId = newsletterId,
            _totalSent = totalSent,
            _totalOpened = 0,
            _totalClicked = 0,
            _totalUnsubscribed = 0,
            _openRate = 0,
            _clickRate = 0,
            _recordedDate = DateTime.UtcNow
        };
    }
    
    private NewsletterAnalytics() { }
    
    public void RecordOpen()
    {
        _totalOpened++;
        CalculateRates();
    }
    
    public void RecordClick()
    {
        _totalClicked++;
        CalculateRates();
    }
    
    public void RecordUnsubscribe()
    {
        _totalUnsubscribed++;
    }
    
    private void CalculateRates()
    {
        if (_totalSent > 0)
        {
            _openRate = Math.Round((_totalOpened / (decimal)_totalSent) * 100, 2);
            _clickRate = Math.Round((_totalClicked / (decimal)_totalSent) * 100, 2);
        }
    }
    
    public void UpdateMetrics(int opened, int clicked, int unsubscribed)
    {
        if (opened < 0 || clicked < 0 || unsubscribed < 0)
            throw new DomainException("Metrics cannot be negative");
        
        if (opened > _totalSent || clicked > _totalSent || unsubscribed > _totalSent)
            throw new DomainException("Metrics cannot exceed total sent");
        
        _totalOpened = opened;
        _totalClicked = clicked;
        _totalUnsubscribed = unsubscribed;
        CalculateRates();
    }
}
