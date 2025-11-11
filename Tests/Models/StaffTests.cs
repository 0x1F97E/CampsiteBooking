using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class StaffTests
{
    // Helper method to create a valid staff for testing
    private Staff CreateValidStaff(
        string email = "staff@campsite.com",
        string firstName = "Jane",
        string lastName = "Staff",
        string employeeId = "EMP001",
        int campsiteId = 1,
        DateTime? hireDate = null,
        string department = "",
        string phone = "",
        string country = "")
    {
        return Staff.Create(
            Email.Create(email),
            firstName,
            lastName,
            employeeId,
            CampsiteId.Create(campsiteId),
            hireDate ?? DateTime.UtcNow.AddYears(-1),
            department,
            phone,
            country
        );
    }

    [Fact]
    public void Staff_InheritsFromUser()
    {
        // Arrange & Act
        var staff = CreateValidStaff();

        // Assert
        Assert.IsAssignableFrom<User>(staff);
        Assert.Equal("staff@campsite.com", staff.Email.Value);
        Assert.Equal("Jane Staff", staff.FullName);
    }

    [Fact]
    public void Staff_EmployeeId_IsSetCorrectly()
    {
        // Arrange & Act
        var staff = CreateValidStaff(employeeId: "EMP001");

        // Assert
        Assert.Equal("EMP001", staff.EmployeeId);
    }

    [Fact]
    public void Staff_Create_ThrowsException_WhenEmployeeIdIsEmpty()
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => CreateValidStaff(employeeId: ""));
        Assert.Throws<DomainException>(() => CreateValidStaff(employeeId: "   "));
    }

    [Fact]
    public void Staff_Create_ThrowsException_WhenHireDateIsInFuture()
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => CreateValidStaff(hireDate: DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void Staff_UpdateEmployeeId_UpdatesValue()
    {
        // Arrange
        var staff = CreateValidStaff();

        // Act
        staff.UpdateEmployeeId("EMP002");

        // Assert
        Assert.Equal("EMP002", staff.EmployeeId);
    }

    [Fact]
    public void Staff_UpdateEmployeeId_ThrowsException_WhenEmpty()
    {
        // Arrange
        var staff = CreateValidStaff();

        // Act & Assert
        Assert.Throws<DomainException>(() => staff.UpdateEmployeeId(""));
        Assert.Throws<DomainException>(() => staff.UpdateEmployeeId("   "));
    }

    [Fact]
    public void Staff_UpdateDepartment_UpdatesValue()
    {
        // Arrange
        var staff = CreateValidStaff();

        // Act
        staff.UpdateDepartment("Maintenance");

        // Assert
        Assert.Equal("Maintenance", staff.Department);
    }

    [Fact]
    public void Staff_TransferToCampsite_UpdatesCampsiteId()
    {
        // Arrange
        var staff = CreateValidStaff(campsiteId: 1);
        var newCampsiteId = CampsiteId.Create(2);

        // Act
        staff.TransferToCampsite(newCampsiteId);

        // Assert
        Assert.Equal(2, staff.CampsiteId.Value);
    }

    [Fact]
    public void Staff_HireDate_IsSetCorrectly()
    {
        // Arrange
        var hireDate = DateTime.UtcNow.AddYears(-2);

        // Act
        var staff = CreateValidStaff(hireDate: hireDate);

        // Assert
        Assert.Equal(hireDate, staff.HireDate);
    }

    [Fact]
    public void Staff_Department_CanBeEmpty()
    {
        // Arrange & Act
        var staff = CreateValidStaff(department: "");

        // Assert
        Assert.Equal("", staff.Department);
    }
}

