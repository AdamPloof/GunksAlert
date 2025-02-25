using GunksAlert.Api.Models;

namespace GunksAlert.Api.Data;

/// <summary>
/// Seed the database with a basic climbable conditions
/// </summary>
public static class ClimbableConditionSeeder {
    public static readonly string DefaultSummary = "A good day at the Gunks";

    public static async Task SeedAsync(
        GunksDbContext context
    ) {
        int conditionsCount = context.Set<ClimbableConditions>()
            .Where(c => c.Summary == DefaultSummary)
            .Count();
        if (conditionsCount > 0) {
            // Only add condition if there aren't any saved yet
            return;
        }

        ClimbableConditions conditions = new ClimbableConditions() {
            Summary = DefaultSummary,
            TempMin = 40,
            TempMax = 85,
            WindSpeed = 4,
            WindGust = 10,
            WindDegree = 200,
            Clouds = 20,
            Humidity = 20,
            Pop = 0,
            Rain = 0.0,
            Snow = 0.0
        };
        await context.Set<ClimbableConditions>().AddAsync(conditions);
        await context.SaveChangesAsync();
    }
}
