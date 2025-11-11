using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;

namespace CampsiteBooking.Models;

/// <summary>
/// Staff user entity - limited access for campsite operations
/// </summary>
public class Staff : User
{
    private string _employeeId = string.Empty;
    private string _department = string.Empty;
    private CampsiteId _campsiteId = null!;
    private DateTime _hireDate;

    public int StaffId => UserId; // Staff uses same ID as User

    public string EmployeeId => _employeeId;
    public string Department => _department;
    public CampsiteId CampsiteId => _campsiteId;
    public DateTime HireDate => _hireDate;

    // ============================================================================
    // FACTORY METHOD
    // ============================================================================

    public static Staff Create(
        Email email,
        string firstName,
        string lastName,
        string employeeId,
        CampsiteId campsiteId,
        DateTime hireDate,
        string department = "",
        string phone = "",
        string country = "")
    {
        // Validate business rules
        if (string.IsNullOrWhiteSpace(employeeId))
            throw new DomainException("Employee ID cannot be empty");

        if (hireDate > DateTime.UtcNow)
            throw new DomainException("Hire date cannot be in the future");

        var staff = new Staff(email, firstName, lastName, phone, country)
        {
            _employeeId = employeeId.Trim(),
            _campsiteId = campsiteId,
            _hireDate = hireDate,
            _department = department?.Trim() ?? string.Empty
        };

        return staff;
    }

    // ============================================================================
    // CONSTRUCTORS
    // ============================================================================

    /// <summary>
    /// Protected constructor for EF Core
    /// </summary>
    private Staff()
    {
    }

    /// <summary>
    /// Internal constructor using base User constructor
    /// </summary>
    private Staff(Email email, string firstName, string lastName, string phone, string country)
        : base(email, firstName, lastName, phone, country)
    {
    }

    // ============================================================================
    // BUSINESS METHODS
    // ============================================================================

    /// <summary>
    /// Update employee ID
    /// </summary>
    public void UpdateEmployeeId(string employeeId)
    {
        if (string.IsNullOrWhiteSpace(employeeId))
            throw new DomainException("Employee ID cannot be empty");

        _employeeId = employeeId.Trim();
    }

    /// <summary>
    /// Update department
    /// </summary>
    public void UpdateDepartment(string department)
    {
        _department = department?.Trim() ?? string.Empty;
    }

    /// <summary>
    /// Transfer to another campsite
    /// </summary>
    public void TransferToCampsite(CampsiteId newCampsiteId)
    {
        _campsiteId = newCampsiteId;
    }
}

