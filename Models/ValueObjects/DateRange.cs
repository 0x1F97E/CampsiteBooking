namespace CampsiteBooking.Models.ValueObjects;

/// <summary>
/// Value Object representing a date range (e.g., booking period).
/// Ensures start date is always before end date.
/// </summary>
public class DateRange : ValueObject
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    private DateRange(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be after start date", nameof(endDate));

        StartDate = startDate.Date; // Store only date part
        EndDate = endDate.Date;
    }

    public static DateRange Create(DateTime startDate, DateTime endDate)
    {
        return new DateRange(startDate, endDate);
    }

    public int GetNumberOfDays()
    {
        return (EndDate - StartDate).Days;
    }

    public int GetNumberOfNights()
    {
        return GetNumberOfDays();
    }

    public bool Overlaps(DateRange other)
    {
        return StartDate < other.EndDate && EndDate > other.StartDate;
    }

    public bool Contains(DateTime date)
    {
        var checkDate = date.Date;
        return checkDate >= StartDate && checkDate < EndDate;
    }

    public bool IsInFuture()
    {
        return StartDate > DateTime.UtcNow.Date;
    }

    public bool IsInPast()
    {
        return EndDate < DateTime.UtcNow.Date;
    }

    public bool IsActive()
    {
        var today = DateTime.UtcNow.Date;
        return StartDate <= today && EndDate > today;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    public override string ToString()
    {
        return $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd} ({GetNumberOfNights()} nights)";
    }
}

