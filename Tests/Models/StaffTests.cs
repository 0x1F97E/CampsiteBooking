using CampsiteBooking.Models;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class StaffTests
{
    [Fact]
    public void Staff_InheritsFromUser()
    {
        // Arrange & Act
        var staff = new Staff
        {
            Email = Email.Create("staff@campsite.com"),
            FirstName = "Jane",
            LastName = "Staff",
            EmployeeId = "EMP001",
            CampsiteId = 1,
            HireDate = DateTime.UtcNow.AddYears(-1)
        };

        // Assert
        Assert.IsAssignableFrom<User>(staff);
        Assert.Equal("staff@campsite.com", staff.Email?.Value);
        Assert.Equal("Jane Staff", staff.FullName);
    }

    [Fact]
    public void EmployeeId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var staff = new Staff
        {
            Email = Email.Create("staff@campsite.com"),
            FirstName = "Jane",
            LastName = "Staff"
        };

        // Act
        staff.EmployeeId = "EMP001";

        // Assert
        Assert.Equal("EMP001", staff.EmployeeId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void EmployeeId_EmptyValue_ThrowsArgumentException(string invalidEmployeeId)
    {
        // Arrange
        var staff = new Staff
        {
            Email = Email.Create("staff@campsite.com"),
            FirstName = "Jane",
            LastName = "Staff"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => staff.EmployeeId = invalidEmployeeId);
    }

    [Fact]
    public void CampsiteId_ValidValue_SetsCorrectly()
    {
        // Arrange
        var staff = new Staff
        {
            Email = Email.Create("staff@campsite.com"),
            FirstName = "Jane",
            LastName = "Staff",
            EmployeeId = "EMP001"
        };

        // Act
        staff.CampsiteId = 5;

        // Assert
        Assert.Equal(5, staff.CampsiteId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CampsiteId_InvalidValue_ThrowsArgumentException(int invalidCampsiteId)
    {
        // Arrange
        var staff = new Staff
        {
            Email = Email.Create("staff@campsite.com"),
            FirstName = "Jane",
            LastName = "Staff",
            EmployeeId = "EMP001"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => staff.CampsiteId = invalidCampsiteId);
    }

    [Fact]
    public void HireDate_ValidDate_SetsCorrectly()
    {
        // Arrange
        var staff = new Staff
        {
            Email = Email.Create("staff@campsite.com"),
            FirstName = "Jane",
            LastName = "Staff",
            EmployeeId = "EMP001",
            CampsiteId = 1
        };
        var hireDate = DateTime.UtcNow.AddYears(-2);

        // Act
        staff.HireDate = hireDate;

        // Assert
        Assert.Equal(hireDate, staff.HireDate);
    }

    [Fact]
    public void HireDate_FutureDate_ThrowsArgumentException()
    {
        // Arrange
        var staff = new Staff
        {
            Email = Email.Create("staff@campsite.com"),
            FirstName = "Jane",
            LastName = "Staff",
            EmployeeId = "EMP001",
            CampsiteId = 1
        };
        var futureDate = DateTime.UtcNow.AddDays(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => staff.HireDate = futureDate);
    }

    [Fact]
    public void Department_CanBeSet()
    {
        // Arrange
        var staff = new Staff
        {
            Email = Email.Create("staff@campsite.com"),
            FirstName = "Jane",
            LastName = "Staff",
            EmployeeId = "EMP001",
            CampsiteId = 1,
            HireDate = DateTime.UtcNow.AddYears(-1)
        };

        // Act
        staff.Department = "Reception";

        // Assert
        Assert.Equal("Reception", staff.Department);
    }
}

