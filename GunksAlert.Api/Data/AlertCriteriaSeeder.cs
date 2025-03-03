using System.Collections.Generic;

using GunksAlert.Api.Models;
using GunksAlert.Api.Security;

namespace GunksAlert.Api.Data;

/// <summary>
/// Seed the database with a starting AlertCriteria and assigns it to the
/// admin user
/// </summary>
public static class AlertCriteriaSeeder {
    public static async Task SeedAsync(GunksDbContext context) {
        int conditionsCount = context.Set<ClimbableConditions>().Count();
        int periodCount = context.Set<AlertPeriod>().Count();
        int criteriaCount = context.Set<AlertCriteria>().Count();
        if (conditionsCount == 0 || periodCount == 0 || criteriaCount > 0) {
            // Only add criteria if there aren't any saved yet and there
            // are periods/conditions to associate it with
            return;
        }

        ClimbableConditions conditions = context.Set<ClimbableConditions>().Where(
            c => c.Summary == ClimbableConditionSeeder.DefaultSummary
        ).First();
        AlertPeriod period = context.Set<AlertPeriod>().Where(
            p => p.Months == 3087 && p.DaysOfWeek == 113
        ).First();
        Crag gunks = context.Set<Crag>().Where(c => c.Name == "Gunks").First();
        AlertCriteria criteria = new AlertCriteria() {
            Crag = gunks,
            ClimbableConditions = conditions,
            AlertPeriod = period
        };
        AppUser admin = context.Set<AppUser>().Where(u => u.NormalizedUserName == "GUNKSADMIN").First();
        admin.AddCriteria(criteria);
        await context.SaveChangesAsync();
    }
}
