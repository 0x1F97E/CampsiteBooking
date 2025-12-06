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
        // Use EF.Property to access the private field since SeasonName property is ignored in EF config
        return await context.SeasonalPricings
            .Select(sp => EF.Property<string>(sp, "_seasonName"))
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

        // Define stay duration categories
        var categoryDefinitions = new (string Name, int MinNights, int MaxNights)[]
        {
            ("1-2 nights", 1, 2),
            ("3-5 nights", 3, 5),
            ("6-7 nights", 6, 7),
            ("8-14 nights", 8, 14),
            ("15+ nights", 15, int.MaxValue)
        };

        // Calculate nights for each booking and group by category
        var bookingDurations = bookings
            .Select(b => (b.Period.EndDate - b.Period.StartDate).Days)
            .ToList();

        var totalBookings = bookingDurations.Count;
        var totalNights = bookingDurations.Sum();
        var averageNights = totalBookings > 0 ? (double)totalNights / totalBookings : 0;

        // Group bookings into categories
        var categories = categoryDefinitions.Select(cat =>
        {
            var count = bookingDurations.Count(nights => nights >= cat.MinNights && nights <= cat.MaxNights);
            return new LengthOfStayCategory
            {
                Category = cat.Name,
                BookingCount = count,
                Percentage = totalBookings > 0 ? (double)count / totalBookings * 100 : 0
            };
        }).ToList();

        return new LengthOfStayStats
        {
            Categories = categories,
            AverageNights = averageNights,
            TotalBookings = totalBookings
        };
    }

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
            // Load seasonal pricing data using EF.Property to access private fields
            var seasonalPricings = await context.SeasonalPricings.ToListAsync(cancellationToken);

            // Get unique season date ranges for the selected seasons (year-agnostic using month/day)
            var seasonDateRanges = seasonalPricings
                .Where(sp => filter.Seasons.Contains(sp.SeasonName))
                .Select(sp => new {
                    StartMonth = sp.StartDate.Month,
                    StartDay = sp.StartDate.Day,
                    EndMonth = sp.EndDate.Month,
                    EndDay = sp.EndDate.Day
                })
                .Distinct()
                .ToList();

            // Filter bookings where the start date falls within any of the season date ranges (year-agnostic)
            bookings = bookings.Where(b =>
            {
                var bookingMonth = b.Period.StartDate.Month;
                var bookingDay = b.Period.StartDate.Day;

                return seasonDateRanges.Any(range =>
                {
                    // Handle seasons that don't cross year boundary (e.g., June 1 - Aug 31)
                    if (range.StartMonth <= range.EndMonth)
                    {
                        return (bookingMonth > range.StartMonth || (bookingMonth == range.StartMonth && bookingDay >= range.StartDay))
                            && (bookingMonth < range.EndMonth || (bookingMonth == range.EndMonth && bookingDay <= range.EndDay));
                    }
                    // Handle seasons that cross year boundary (e.g., Nov 1 - Mar 31)
                    else
                    {
                        return (bookingMonth > range.StartMonth || (bookingMonth == range.StartMonth && bookingDay >= range.StartDay))
                            || (bookingMonth < range.EndMonth || (bookingMonth == range.EndMonth && bookingDay <= range.EndDay));
                    }
                });
            }).ToList();
        }

        return bookings;
    }
}
