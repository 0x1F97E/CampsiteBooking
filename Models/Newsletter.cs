using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

public class Newsletter : Entity<NewsletterId>
{
    private string _subject = string.Empty;
    private string _content = string.Empty;
    private DateTime _scheduledDate;
    private DateTime? _sentDate;
    private string _status = "Draft";
    private int _recipientCount;
    private DateTime _createdDate;
    
    public string Subject => _subject;
    public string Content => _content;
    public DateTime ScheduledDate => _scheduledDate;
    public DateTime? SentDate => _sentDate;
    public string Status => _status;
    public int RecipientCount => _recipientCount;
    public DateTime CreatedDate => _createdDate;
    
    public int NewsletterId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.NewsletterId.Create(value) : ValueObjects.NewsletterId.CreateNew();
    }
    
    public static Newsletter Create(string subject, string content, DateTime scheduledDate)
    {
        if (string.IsNullOrWhiteSpace(subject))
            throw new DomainException("Subject cannot be empty");
        
        if (subject.Length > 200)
            throw new DomainException("Subject cannot exceed 200 characters");
        
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Content cannot be empty");
        
        if (scheduledDate < DateTime.UtcNow)
            throw new DomainException("Scheduled date cannot be in the past");
        
        return new Newsletter
        {
            Id = ValueObjects.NewsletterId.CreateNew(),
            _subject = subject.Trim(),
            _content = content.Trim(),
            _scheduledDate = scheduledDate,
            _status = "Draft",
            _recipientCount = 0,
            _createdDate = DateTime.UtcNow
        };
    }
    
    private Newsletter() { }
    
    public void Schedule(DateTime scheduledDate)
    {
        if (scheduledDate < DateTime.UtcNow)
            throw new DomainException("Scheduled date cannot be in the past");
        
        if (_status == "Sent")
            throw new DomainException("Cannot reschedule a sent newsletter");
        
        _scheduledDate = scheduledDate;
        _status = "Scheduled";
    }
    
    public void Send(int recipientCount)
    {
        if (_status == "Sent")
            throw new DomainException("Newsletter has already been sent");
        
        if (recipientCount <= 0)
            throw new DomainException("Recipient count must be greater than 0");
        
        _sentDate = DateTime.UtcNow;
        _recipientCount = recipientCount;
        _status = "Sent";
    }
    
    public void UpdateContent(string subject, string content)
    {
        if (_status == "Sent")
            throw new DomainException("Cannot update a sent newsletter");
        
        if (string.IsNullOrWhiteSpace(subject))
            throw new DomainException("Subject cannot be empty");
        
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Content cannot be empty");
        
        _subject = subject.Trim();
        _content = content.Trim();
    }
    
    public void Cancel()
    {
        if (_status == "Sent")
            throw new DomainException("Cannot cancel a sent newsletter");
        _status = "Cancelled";
    }
}
