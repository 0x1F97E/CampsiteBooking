using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

public class MaintenanceTask : Entity<MaintenanceTaskId>
{
    private CampsiteId _campsiteId = null!;
    private AccommodationSpotId? _accommodationSpotId;
    private string _title = string.Empty;
    private string _description = string.Empty;
    private string _priority = "Medium";
    private string _status = "Pending";
    private DateTime _scheduledDate;
    private DateTime? _completedDate;
    private UserId? _assignedToUserId;
    private DateTime _createdDate;
    
    public CampsiteId CampsiteId => _campsiteId;
    public AccommodationSpotId? AccommodationSpotId => _accommodationSpotId;
    public string Title => _title;
    public string Description => _description;
    public string Priority => _priority;
    public string Status => _status;
    public DateTime ScheduledDate => _scheduledDate;
    public DateTime? CompletedDate => _completedDate;
    public UserId? AssignedToUserId => _assignedToUserId;
    public DateTime CreatedDate => _createdDate;
    
    public int MaintenanceTaskId
    {
        get => Id?.Value ?? 0;
        private set => Id = value > 0 ? ValueObjects.MaintenanceTaskId.Create(value) : ValueObjects.MaintenanceTaskId.CreateNew();
    }
    
    public static MaintenanceTask Create(CampsiteId campsiteId, string title, string description, string priority, DateTime scheduledDate, AccommodationSpotId? accommodationSpotId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");
        
        if (title.Length > 200)
            throw new DomainException("Title cannot exceed 200 characters");
        
        var validPriorities = new[] { "Low", "Medium", "High", "Critical" };
        if (!validPriorities.Contains(priority))
            throw new DomainException("Priority must be Low, Medium, High, or Critical");
        
        return new MaintenanceTask
        {
            Id = ValueObjects.MaintenanceTaskId.CreateNew(),
            _campsiteId = campsiteId,
            _accommodationSpotId = accommodationSpotId,
            _title = title.Trim(),
            _description = description?.Trim() ?? string.Empty,
            _priority = priority,
            _status = "Pending",
            _scheduledDate = scheduledDate,
            _createdDate = DateTime.UtcNow
        };
    }
    
    private MaintenanceTask() { }
    
    public void AssignTo(UserId userId)
    {
        if (_status == "Completed")
            throw new DomainException("Cannot assign a completed task");
        
        _assignedToUserId = userId;
        if (_status == "Pending")
            _status = "Assigned";
    }
    
    public void Start()
    {
        if (_status == "Completed")
            throw new DomainException("Cannot start a completed task");
        
        _status = "InProgress";
    }
    
    public void Complete()
    {
        if (_status == "Completed")
            throw new DomainException("Task is already completed");
        
        _status = "Completed";
        _completedDate = DateTime.UtcNow;
    }
    
    public void Cancel()
    {
        if (_status == "Completed")
            throw new DomainException("Cannot cancel a completed task");
        
        _status = "Cancelled";
    }
    
    public void UpdatePriority(string priority)
    {
        var validPriorities = new[] { "Low", "Medium", "High", "Critical" };
        if (!validPriorities.Contains(priority))
            throw new DomainException("Priority must be Low, Medium, High, or Critical");
        
        _priority = priority;
    }
}
