using Microsoft.EntityFrameworkCore;
using CampsiteBooking.Data;
using CampsiteBooking.Models;

namespace CampsiteBooking.Services;

public class DashboardService : IDashboardService
{
    private readonly IDbContextFactory<CampsiteBookingDbContext> _contextFactory;
    private static readonly string[] MonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

    public DashboardService(IDbContextFactory<CampsiteBookingDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<int>> GetAvailableYearsAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        // Load all bookings and extract years client-side (Period is mapped via private field)
        var bookings = await context.Bookings.ToListAsync(cancellationToken);
        return bookings
            .Select(b => b.Period.StartDate.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToList();
    }

    public async Task<List<string>> GetAvailableSeasonsAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        return await context.SeasonalPricings
            .Select(sp => sp.SeasonName)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CampsiteInfo>> GetCampsitesAsync(CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        // Load all campsites and filter client-side (IsActive is mapped via private field)
        var campsites = await context.Campsites.ToListAsync(cancellationToken);
        return campsites
            .Where(c => c.IsActive)
            .Select(c => new CampsiteInfo { Id = c.Id.Value, Name = c.Name })
            .ToList();
    }

    public async Task<BookingStats> GetBookingStatsAsync(DashboardFilter filter, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var bookings = await GetFilteredBookingsAsync(context, filter, cancellationToken);

        var monthlyData = bookings
            .GroupBy(b => new { b.Period.StartDate.Year, b.Period.StartDate.Month })
            .Select(g => new BookingMonthData
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                BookingCount = g.Count(),
                MonthName = MonthNames[g.Key.Month - 1]
            })
            .OrderBy(d => d.Year).ThenBy(d => d.Month)
            .ToList();

        return new BookingStats
        {
            MonthlyData = monthlyData,
            TotalBookings = monthlyData.Sum(d => d.BookingCount)
        };
    }

    public async Task<RevenueStats> GetRevenueStatsAsync(DashboardFilter filter, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var bookings = await GetFilteredBookingsAsync(context, filter, cancellationToken);

        var monthlyData = bookings
            .GroupBy(b => new { b.Period.StartDate.Year, b.Period.StartDate.Month })
            .Select(g => new RevenueMonthData
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Revenue = g.Sum(b => b.TotalPrice.Amount),
                MonthName = MonthNames[g.Key.Month - 1]
            })
            .OrderBy(d => d.Year).ThenBy(d => d.Month)
            .ToList();

        return new RevenueStats
        {
            MonthlyData = monthlyData,
            TotalRevenue = monthlyData.Sum(d => d.Revenue)
        };
    }

    public async Task<LengthOfStayStats> GetLengthOfStayStatsAsync(DashboardFilter filter, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var bookings = await GetFilteredBookingsAsync(context, filter, cancellationToken);

        var stays = bookings.Select(b => (b.Period.EndDate - b.Period.StartDate).Days).ToList();

        var distribution = stays
            .GroupBy(nights => GetLengthCategory(nights))
            .Select(g => new LengthOfStayData { Category = g.Key, Count = g.Count() })
            .OrderBy(d => GetCategoryOrder(d.Category))
            .ToList();

        return new LengthOfStayStats
        {
            Distribution = distribution,
            AverageNights = stays.Any() ? stays.Average() : 0
        };
    }

    private static string GetLengthCategory(int nights) => nights switch
    {
        <= 2 => "1-2 nights",
        <= 5 => "3-5 nights",
        <= 7 => "6-7 nights",
        <= 14 => "8-14 nights",
        _ => "15+ nights"
    };

    private static int GetCategoryOrder(string cat) => cat switch
    {
        "1-2 nights" => 1, "3-5 nights" => 2, "6-7 nights" => 3, "8-14 nights" => 4, _ => 5
    };

    public async Task<CountryStats> GetCountryStatsAsync(DashboardFilter filter, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var bookings = await GetFilteredBookingsAsync(context, filter, cancellationToken);
        var guests = await context.Guests.ToListAsync(cancellationToken);

        // Join bookings with guests client-side
        var countryData = bookings
            .Join(guests, b => b.GuestId.Value, g => g.UserId, (b, g) => g.Country)
            .Where(country => !string.IsNullOrEmpty(country))
            .GroupBy(country => country)
            .Select(g => new { Country = g.Key, Count = g.Count() })
            .OrderByDescending(c => c.Count)
            .Take(10)
            .ToList();

        var total = countryData.Sum(c => c.Count);
        return new CountryStats
        {
            Distribution = countryData.Select(c => new CountryData
            {
                Country = c.Country ?? "Unknown",
                Count = c.Count,
                Percentage = total > 0 ? (double)c.Count / total * 100 : 0
            }).ToList(),
            TotalGuests = total
        };
    }

    public async Task<NewUserStats> GetNewUserStatsAsync(DashboardFilter filter, CancellationToken cancellationToken = default)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
        var guests = await context.Guests.ToListAsync(cancellationToken);

        // Filter by years client-side (JoinedDate may be mapped via private field)
        if (filter.Years.Any())
            guests = guests.Where(g => filter.Years.Contains(g.JoinedDate.Year)).ToList();

        var monthlyData = guests
            .GroupBy(g => new { g.JoinedDate.Year, g.JoinedDate.Month })
            .Select(g => new NewUserMonthData
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                NewUserCount = g.Count(),
                MonthName = MonthNames[g.Key.Month - 1]
            })
            .OrderBy(d => d.Year).ThenBy(d => d.Month)
            .ToList();

        return new NewUserStats
        {
            MonthlyData = monthlyData,
            TotalNewUsers = monthlyData.Sum(d => d.NewUserCount)
        };
    }

    private async Task<List<Booking>> GetFilteredBookingsAsync(CampsiteBookingDbContext context, DashboardFilter filter, CancellationToken cancellationToken)
    {
        // Load all bookings first, then filter client-side (properties mapped via private fields)
        var bookings = await context.Bookings.ToListAsync(cancellationToken);

        if (filter.CampsiteIds.Any())
            bookings = bookings.Where(b => filter.CampsiteIds.Contains(b.CampsiteId.Value)).ToList();

        if (filter.Years.Any())
            bookings = bookings.Where(b => filter.Years.Contains(b.Period.StartDate.Year)).ToList();

        if (filter.Seasons.Any())
        {
            var seasonDates = await context.SeasonalPricings
                .Where(sp => filter.Seasons.Contains(sp.SeasonName))
                .Select(sp => new { sp.StartDate, sp.EndDate })
                .ToListAsync(cancellationToken);

            bookings = bookings.Where(b => seasonDates.Any(sd =>
                b.Period.StartDate >= sd.StartDate && b.Period.StartDate <= sd.EndDate)).ToList();
        }

        return bookings;
    }
}
