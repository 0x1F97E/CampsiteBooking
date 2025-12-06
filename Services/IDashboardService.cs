using CampsiteBooking.Models;

namespace CampsiteBooking.Services;

/// <summary>
/// Service interface for dashboard data aggregation
/// </summary>
public interface IDashboardService
{
    Task<List<int>> GetAvailableYearsAsync(CancellationToken cancellationToken = default);
    Task<List<string>> GetAvailableSeasonsAsync(CancellationToken cancellationToken = default);
    Task<List<CampsiteInfo>> GetCampsitesAsync(CancellationToken cancellationToken = default);
    Task<BookingStats> GetBookingStatsAsync(DashboardFilter filter, CancellationToken cancellationToken = default);
    Task<RevenueStats> GetRevenueStatsAsync(DashboardFilter filter, CancellationToken cancellationToken = default);
    Task<LengthOfStayStats> GetLengthOfStayStatsAsync(DashboardFilter filter, CancellationToken cancellationToken = default);
    Task<CountryStats> GetCountryStatsAsync(DashboardFilter filter, CancellationToken cancellationToken = default);
}

public class DashboardFilter
{
    public List<int> CampsiteIds { get; set; } = new();
    public List<int> Years { get; set; } = new();
    public List<string> Seasons { get; set; } = new();
}

public class CampsiteInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class MonthlyDataPoint
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
}

public class BookingStats
{
    public List<BookingMonthData> MonthlyData { get; set; } = new();
    public int TotalBookings { get; set; }
}

public class BookingMonthData : MonthlyDataPoint
{
    public int BookingCount { get; set; }
}

public class RevenueStats
{
    public List<RevenueMonthData> MonthlyData { get; set; } = new();
    public decimal TotalRevenue { get; set; }
}

public class RevenueMonthData : MonthlyDataPoint
{
    public decimal Revenue { get; set; }
}

public class LengthOfStayStats
{
    public List<LengthOfStayCategory> Categories { get; set; } = new();
    public double AverageNights { get; set; }
    public int TotalBookings { get; set; }
}

public class LengthOfStayCategory
{
    public string Category { get; set; } = string.Empty;
    public int BookingCount { get; set; }
    public double Percentage { get; set; }
}

public class CountryStats
{
    public List<CountryData> Distribution { get; set; } = new();
    public int TotalGuests { get; set; }
}

public class CountryData
{
    public string Country { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}



