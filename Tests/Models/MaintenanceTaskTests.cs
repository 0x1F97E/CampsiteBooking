using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class MaintenanceTaskTests
{
    private MaintenanceTask CreateValidTask(string priority = "Medium")
    {
        return MaintenanceTask.Create(CampsiteId.Create(1), "Fix roof", "Repair leaking roof", priority, DateTime.UtcNow.AddDays(1));
    }

    [Fact]
    public void MaintenanceTask_CanBeCreated_WithValidData()
    {
        var task = CreateValidTask();
        Assert.NotNull(task);
        Assert.Equal("Fix roof", task.Title);
        Assert.Equal("Medium", task.Priority);
        Assert.Equal("Pending", task.Status);
    }

    [Fact]
    public void MaintenanceTask_Create_ThrowsException_WhenTitleIsEmpty()
    {
        Assert.Throws<DomainException>(() => MaintenanceTask.Create(CampsiteId.Create(1), "", "Description", "Medium", DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void MaintenanceTask_Create_ThrowsException_WhenTitleIsTooLong()
    {
        var longTitle = new string('a', 201);
        Assert.Throws<DomainException>(() => MaintenanceTask.Create(CampsiteId.Create(1), longTitle, "Description", "Medium", DateTime.UtcNow.AddDays(1)));
    }

    [Fact]
    public void MaintenanceTask_Create_ThrowsException_WhenPriorityIsInvalid()
    {
        Assert.Throws<DomainException>(() => CreateValidTask(priority: "Invalid"));
    }

    [Theory]
    [InlineData("Low")]
    [InlineData("Medium")]
    [InlineData("High")]
    [InlineData("Critical")]
    public void MaintenanceTask_Create_AcceptsValidPriorities(string priority)
    {
        var task = CreateValidTask(priority: priority);
        Assert.Equal(priority, task.Priority);
    }

    [Fact]
    public void MaintenanceTask_AssignTo_ChangesStatusToAssigned()
    {
        var task = CreateValidTask();
        task.AssignTo(UserId.Create(1));
        Assert.Equal("Assigned", task.Status);
        Assert.NotNull(task.AssignedToUserId);
    }

    [Fact]
    public void MaintenanceTask_Start_ChangesStatusToInProgress()
    {
        var task = CreateValidTask();
        task.Start();
        Assert.Equal("InProgress", task.Status);
    }

    [Fact]
    public void MaintenanceTask_Complete_ChangesStatusToCompleted()
    {
        var task = CreateValidTask();
        task.Complete();
        Assert.Equal("Completed", task.Status);
        Assert.NotNull(task.CompletedDate);
    }

    [Fact]
    public void MaintenanceTask_Complete_ThrowsException_WhenAlreadyCompleted()
    {
        var task = CreateValidTask();
        task.Complete();
        Assert.Throws<DomainException>(() => task.Complete());
    }

    [Fact]
    public void MaintenanceTask_Cancel_ChangesStatusToCancelled()
    {
        var task = CreateValidTask();
        task.Cancel();
        Assert.Equal("Cancelled", task.Status);
    }

    [Fact]
    public void MaintenanceTask_Cancel_ThrowsException_WhenCompleted()
    {
        var task = CreateValidTask();
        task.Complete();
        Assert.Throws<DomainException>(() => task.Cancel());
    }

    [Fact]
    public void MaintenanceTask_UpdatePriority_UpdatesPriorityValue()
    {
        var task = CreateValidTask(priority: "Low");
        task.UpdatePriority("Critical");
        Assert.Equal("Critical", task.Priority);
    }
}
