namespace CampsiteBooking.Models;

/// <summary>
/// Staff user entity - limited access for campsite operations
/// </summary>
public class Staff : User
{
    public int StaffId { get; set; }
    
    private string _employeeId = string.Empty;
    public string EmployeeId
    {
        get => _employeeId;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("EmployeeId cannot be empty", nameof(EmployeeId));
            _employeeId = value;
        }
    }
    
    public string Department { get; set; } = string.Empty;
    
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
    
    private DateTime _hireDate;
    public DateTime HireDate
    {
        get => _hireDate;
        set
        {
            if (value > DateTime.UtcNow)
                throw new ArgumentException("HireDate cannot be in the future", nameof(HireDate));
            _hireDate = value;
        }
    }
}

