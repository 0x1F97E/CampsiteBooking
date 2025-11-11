using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.ValueObjects;

public class DateRangeTests
{
    [Fact]
    public void Create_ValidDates_CreatesDateRange()
    {
        // Arrange
        var start = new DateTime(2025, 6, 1);
        var end = new DateTime(2025, 6, 10);

        // Act
        var dateRange = DateRange.Create(start, end);

        // Assert
        Assert.Equal(start.Date, dateRange.StartDate);
        Assert.Equal(end.Date, dateRange.EndDate);
    }

    [Fact]
    public void Create_EndBeforeStart_ThrowsArgumentException()
    {
        // Arrange
        var start = new DateTime(2025, 6, 10);
        var end = new DateTime(2025, 6, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => DateRange.Create(start, end));
    }

    [Fact]
    public void Create_EndEqualsStart_ThrowsArgumentException()
    {
        // Arrange
        var date = new DateTime(2025, 6, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => DateRange.Create(date, date));
    }

    [Fact]
    public void GetNumberOfDays_ReturnsCorrectCount()
    {
        // Arrange
        var start = new DateTime(2025, 6, 1);
        var end = new DateTime(2025, 6, 10);
        var dateRange = DateRange.Create(start, end);

        // Act
        var days = dateRange.GetNumberOfDays();

        // Assert
        Assert.Equal(9, days);
    }

    [Fact]
    public void GetNumberOfNights_ReturnsCorrectCount()
    {
        // Arrange
        var start = new DateTime(2025, 6, 1);
        var end = new DateTime(2025, 6, 10);
        var dateRange = DateRange.Create(start, end);

        // Act
        var nights = dateRange.GetNumberOfNights();

        // Assert
        Assert.Equal(9, nights);
    }

    [Fact]
    public void Overlaps_OverlappingRanges_ReturnsTrue()
    {
        // Arrange
        var range1 = DateRange.Create(new DateTime(2025, 6, 1), new DateTime(2025, 6, 10));
        var range2 = DateRange.Create(new DateTime(2025, 6, 5), new DateTime(2025, 6, 15));

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Overlaps_NonOverlappingRanges_ReturnsFalse()
    {
        // Arrange
        var range1 = DateRange.Create(new DateTime(2025, 6, 1), new DateTime(2025, 6, 10));
        var range2 = DateRange.Create(new DateTime(2025, 6, 11), new DateTime(2025, 6, 20));

        // Act
        var result = range1.Overlaps(range2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Contains_DateInRange_ReturnsTrue()
    {
        // Arrange
        var dateRange = DateRange.Create(new DateTime(2025, 6, 1), new DateTime(2025, 6, 10));
        var date = new DateTime(2025, 6, 5);

        // Act
        var result = dateRange.Contains(date);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_DateOutsideRange_ReturnsFalse()
    {
        // Arrange
        var dateRange = DateRange.Create(new DateTime(2025, 6, 1), new DateTime(2025, 6, 10));
        var date = new DateTime(2025, 6, 15);

        // Act
        var result = dateRange.Contains(date);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInFuture_FutureRange_ReturnsTrue()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(10);
        var end = DateTime.UtcNow.AddDays(20);
        var dateRange = DateRange.Create(start, end);

        // Act
        var result = dateRange.IsInFuture();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInPast_PastRange_ReturnsTrue()
    {
        // Arrange
        var start = DateTime.UtcNow.AddDays(-20);
        var end = DateTime.UtcNow.AddDays(-10);
        var dateRange = DateRange.Create(start, end);

        // Act
        var result = dateRange.IsInPast();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_SameRange_ReturnsTrue()
    {
        // Arrange
        var start = new DateTime(2025, 6, 1);
        var end = new DateTime(2025, 6, 10);
        var range1 = DateRange.Create(start, end);
        var range2 = DateRange.Create(start, end);

        // Act & Assert
        Assert.Equal(range1, range2);
        Assert.True(range1 == range2);
    }

    [Fact]
    public void Equals_DifferentRange_ReturnsFalse()
    {
        // Arrange
        var range1 = DateRange.Create(new DateTime(2025, 6, 1), new DateTime(2025, 6, 10));
        var range2 = DateRange.Create(new DateTime(2025, 6, 5), new DateTime(2025, 6, 15));

        // Act & Assert
        Assert.NotEqual(range1, range2);
        Assert.True(range1 != range2);
    }
}

