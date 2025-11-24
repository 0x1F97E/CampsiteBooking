using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

public class Photo : Entity<PhotoId>
{
    private CampsiteId _campsiteId = null!;
    private AccommodationTypeId? _accommodationTypeId;
    private string _url = string.Empty;
    private string _caption = string.Empty;
    private string _altText = string.Empty;
    private int _displayOrder;
    private bool _isPrimary;
    private DateTime _uploadedDate;
    private DateTime _updatedDate;
    private string _category = "General"; // "Information" or "Gallery"

    public CampsiteId CampsiteId => _campsiteId;
    public AccommodationTypeId? AccommodationTypeId => _accommodationTypeId;
    public string Url => _url;
    public string Caption => _caption;
    public string AltText => _altText;
    public int DisplayOrder => _displayOrder;
    public bool IsPrimary => _isPrimary;
    public DateTime UploadedDate => _uploadedDate;
    public DateTime UpdatedDate => _updatedDate;
    public string Category => _category;
    
    public int PhotoId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.PhotoId.Create(value) : ValueObjects.PhotoId.CreateNew();
    }
    
    public static Photo Create(CampsiteId campsiteId, string url, string caption, string altText, int displayOrder, AccommodationTypeId? accommodationTypeId = null, string category = "General")
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("URL cannot be empty");

        if (displayOrder < 0)
            throw new DomainException("Display order cannot be negative");

        return new Photo
        {
            Id = ValueObjects.PhotoId.CreateNew(),
            _campsiteId = campsiteId,
            _accommodationTypeId = accommodationTypeId,
            _url = url.Trim(),
            _caption = caption?.Trim() ?? string.Empty,
            _altText = altText?.Trim() ?? string.Empty,
            _displayOrder = displayOrder,
            _isPrimary = false,
            _uploadedDate = DateTime.UtcNow,
            _updatedDate = DateTime.UtcNow,
            _category = category
        };
    }
    
    private Photo() { }
    
    public void SetAsPrimary()
    {
        _isPrimary = true;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void UnsetAsPrimary()
    {
        _isPrimary = false;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void UpdateDisplayOrder(int newOrder)
    {
        if (newOrder < 0)
            throw new DomainException("Display order cannot be negative");
        
        _displayOrder = newOrder;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void UpdateCaption(string caption, string altText)
    {
        _caption = caption?.Trim() ?? string.Empty;
        _altText = altText?.Trim() ?? string.Empty;
        _updatedDate = DateTime.UtcNow;
    }
}
