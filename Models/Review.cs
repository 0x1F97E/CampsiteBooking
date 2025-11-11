using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

public class Review : Entity<ReviewId>
{
    private CampsiteId _campsiteId = null!;
    private UserId _userId = null!;
    private BookingId? _bookingId;
    private int _rating;
    private string _comment = string.Empty;
    private string _reviewerName = string.Empty;
    private DateTime _createdDate;
    private DateTime _updatedDate;
    private bool _isApproved;
    private bool _isVisible;
    private string _adminResponse = string.Empty;
    private DateTime? _adminResponseDate;
    
    public CampsiteId CampsiteId => _campsiteId;
    public UserId UserId => _userId;
    public BookingId? BookingId => _bookingId;
    public int Rating => _rating;
    public string Comment => _comment;
    public string ReviewerName => _reviewerName;
    public DateTime CreatedDate => _createdDate;
    public DateTime UpdatedDate => _updatedDate;
    public bool IsApproved => _isApproved;
    public bool IsVisible => _isVisible;
    public string AdminResponse => _adminResponse;
    public DateTime? AdminResponseDate => _adminResponseDate;
    
    public int ReviewId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.ReviewId.Create(value) : ValueObjects.ReviewId.CreateNew();
    }
    
    public static Review Create(CampsiteId campsiteId, UserId userId, int rating, string comment, string reviewerName, BookingId? bookingId = null)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5");
        
        if (comment != null && comment.Length > 2000)
            throw new DomainException("Comment cannot exceed 2000 characters");
        
        if (string.IsNullOrWhiteSpace(reviewerName))
            throw new DomainException("Reviewer name cannot be empty");
        
        return new Review
        {
            Id = ValueObjects.ReviewId.CreateNew(),
            _campsiteId = campsiteId,
            _userId = userId,
            _bookingId = bookingId,
            _rating = rating,
            _comment = comment?.Trim() ?? string.Empty,
            _reviewerName = reviewerName.Trim(),
            _createdDate = DateTime.UtcNow,
            _updatedDate = DateTime.UtcNow,
            _isApproved = false,
            _isVisible = true
        };
    }
    
    private Review() { }
    
    public void Approve()
    {
        _isApproved = true;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void Reject()
    {
        _isApproved = false;
        _isVisible = false;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void Hide()
    {
        _isVisible = false;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void Show()
    {
        _isVisible = true;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void UpdateReview(int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5");
        
        if (comment != null && comment.Length > 2000)
            throw new DomainException("Comment cannot exceed 2000 characters");
        
        _rating = rating;
        _comment = comment?.Trim() ?? string.Empty;
        _updatedDate = DateTime.UtcNow;
    }
    
    public void AddAdminResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            throw new DomainException("Admin response cannot be empty");
        
        if (response.Length > 1000)
            throw new DomainException("Admin response cannot exceed 1000 characters");
        
        _adminResponse = response.Trim();
        _adminResponseDate = DateTime.UtcNow;
        _updatedDate = DateTime.UtcNow;
    }
}
