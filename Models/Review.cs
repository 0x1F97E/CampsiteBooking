namespace CampsiteBooking.Models;

public class Review
{
    public int ReviewId { get; set; }

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

    private int _userId;
    public int UserId
    {
        get => _userId;
        set
        {
            if (value <= 0)
                throw new ArgumentException("UserId must be greater than 0", nameof(UserId));
            _userId = value;
        }
    }

    public int? BookingId { get; set; } // Optional - link to specific booking

    private int _rating;
    public int Rating
    {
        get => _rating;
        set
        {
            if (value < 1 || value > 5)
                throw new ArgumentException("Rating must be between 1 and 5", nameof(Rating));
            _rating = value;
        }
    }

    private string _comment = string.Empty;
    public string Comment
    {
        get => _comment;
        set
        {
            if (value != null && value.Length > 2000)
                throw new ArgumentException("Comment cannot exceed 2000 characters", nameof(Comment));
            _comment = value ?? string.Empty;
        }
    }

    public string ReviewerName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; } = false;
    public bool IsVisible { get; set; } = true;
    public string AdminResponse { get; set; } = string.Empty;
    public DateTime? AdminResponseDate { get; set; }

    // Business methods
    public void Approve()
    {
        IsApproved = true;
        UpdatedDate = DateTime.UtcNow;
    }

    public void Reject()
    {
        IsApproved = false;
        IsVisible = false;
        UpdatedDate = DateTime.UtcNow;
    }

    public void Hide()
    {
        IsVisible = false;
        UpdatedDate = DateTime.UtcNow;
    }

    public void Show()
    {
        IsVisible = true;
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateReview(int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));

        if (comment != null && comment.Length > 2000)
            throw new ArgumentException("Comment cannot exceed 2000 characters", nameof(comment));

        Rating = rating;
        Comment = comment ?? string.Empty;
        UpdatedDate = DateTime.UtcNow;
    }

    public void AddAdminResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            throw new ArgumentException("Admin response cannot be empty", nameof(response));

        if (response.Length > 1000)
            throw new ArgumentException("Admin response cannot exceed 1000 characters", nameof(response));

        AdminResponse = response;
        AdminResponseDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
    }
}

