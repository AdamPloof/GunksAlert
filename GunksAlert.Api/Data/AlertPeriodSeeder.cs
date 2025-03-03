using System.Collections.Generic;

using GunksAlert.Api.Models;

namespace GunksAlert.Api.Data;

/// <summary>
/// Seed the database with a standard AlertPeriod
/// </summary>
public static class AlertPeriodSeeder {
    public static async Task SeedAsync(GunksDbContext context) {
        int periodCount = context.Set<AlertPeriod>().Count();
        if (periodCount > 0) {
            // Only add period if there aren't any saved yet
            return;
        }

        AlertPeriod period = new AlertPeriod() {
            StartDate = DateTimeOffset.MinValue,
            EndDate = DateTimeOffset.MaxValue
        };
        period.SetDaysOfWeek(new List<string>() {"Thursday", "Friday", "Saturday", "Sunday"});
        period.SetMonths(new List<string>() {
            "November",
            "December",
            "January",
            "February",
            "March",
            "April"
        });

        await context.Set<AlertPeriod>().AddAsync(period);
        await context.SaveChangesAsync();
    }
}
