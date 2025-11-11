namespace CampsiteBooking.Models;

public class Photo
{
    public int PhotoId { get; set; }

    private int _campsiteId;
    public int CampsiteId
    {
        get => _campsiteId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("CampsiteId must be greater than 0", nameof(CampsiteId));
            _campsiteId = value;
        }
    }

    public int? AccommodationTypeId { get; set; } // Nullable - photo can be for campsite or specific accommodation type

    private string _url = string.Empty;
    public string Url
    {
        get => _url;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Url cannot be empty", nameof(Url));
            _url = value;
        }
    }

    public string Caption { get; set; } = string.Empty;
    public string AltText { get; set; } = string.Empty;

    private int _displayOrder;
    public int DisplayOrder
    {
        get => _displayOrder;
        set
        {
            if (value < 0)
                throw new ArgumentException("DisplayOrder cannot be negative", nameof(DisplayOrder));
            _displayOrder = value;
        }
    }

    public bool IsPrimary { get; set; } = false;
    public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Business methods
    public void SetAsPrimary()
    {
        IsPrimary = true;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UnsetAsPrimary()
    {
        IsPrimary = false;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateDisplayOrder(int newOrder)
    {
        if (newOrder < 0)
            throw new ArgumentException("Display order cannot be negative", nameof(newOrder));

        DisplayOrder = newOrder;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateCaption(string caption, string altText)
    {
        Caption = caption ?? string.Empty;
        AltText = altText ?? string.Empty;
        UpdatedDate = DateTime.UtcNow;
    }
}

