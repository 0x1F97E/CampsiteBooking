using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

public class NewsletterSubscription : Entity<NewsletterSubscriptionId>
{
    private Email _email = null!;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private bool _isActive;
    private DateTime _subscribedDate;
    private DateTime? _unsubscribedDate;
    private string _preferredLanguage = "da";
    
    public Email Email => _email;
    public string FirstName => _firstName;
    public string LastName => _lastName;
    public bool IsActive => _isActive;
    public DateTime SubscribedDate => _subscribedDate;
    public DateTime? UnsubscribedDate => _unsubscribedDate;
    public string PreferredLanguage => _preferredLanguage;
    
    public int NewsletterSubscriptionId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.NewsletterSubscriptionId.Create(value) : ValueObjects.NewsletterSubscriptionId.CreateNew();
    }
    
    public static NewsletterSubscription Create(Email email, string firstName, string lastName, string preferredLanguage = "da")
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty");
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty");
        
        var validLanguages = new[] { "da", "en", "de", "sv", "no" };
        if (!validLanguages.Contains(preferredLanguage))
            throw new DomainException("Preferred language must be da, en, de, sv, or no");
        
        return new NewsletterSubscription
        {
            Id = ValueObjects.NewsletterSubscriptionId.CreateNew(),
            _email = email,
            _firstName = firstName.Trim(),
            _lastName = lastName.Trim(),
            _isActive = true,
            _subscribedDate = DateTime.UtcNow,
            _preferredLanguage = preferredLanguage
        };
    }
    
    private NewsletterSubscription() { }
    
    public void Unsubscribe()
    {
        if (!_isActive)
            throw new DomainException("Subscription is already inactive");
        
        _isActive = false;
        _unsubscribedDate = DateTime.UtcNow;
    }
    
    public void Resubscribe()
    {
        if (_isActive)
            throw new DomainException("Subscription is already active");
        
        _isActive = true;
        _unsubscribedDate = null;
    }
    
    public void UpdatePreferredLanguage(string language)
    {
        var validLanguages = new[] { "da", "en", "de", "sv", "no" };
        if (!validLanguages.Contains(language))
            throw new DomainException("Preferred language must be da, en, de, sv, or no");
        
        _preferredLanguage = language;
    }
}
