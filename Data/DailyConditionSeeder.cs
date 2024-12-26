using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using GunksAlert.Models;

namespace GunksAlert.Data;

/// <summary>
/// Seed the database with the default DailyCondition entities
/// </summary>
/// <seealso href="https://openweathermap.org/weather-conditions" />>
public static class DailyConditionSeeder {
    public static async Task SeedAsync(
        string dataPath,
        GunksDbContext context,
        ILogger logger
    ) {
        if (context.DailyConditions.Any()) {
            return; // DailyConditions already seeded
        }

        if (string.IsNullOrWhiteSpace(dataPath)) {
            throw new ArgumentException("Path to Conditions data must not be empty", nameof(dataPath));
        }

        try {
            if (!File.Exists(dataPath)) {
                throw new FileNotFoundException($"WeatherConditions data source {dataPath} does not exist");
            }

            string conditionsJson = await File.ReadAllTextAsync(dataPath);
            List<DailyCondition>? conditions = JsonSerializer.Deserialize<List<DailyCondition>>(
                conditionsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (conditions == null || conditions.Count == 0) {
                logger.LogWarning("Conditions data file is empty. No conditions were added");
                return;
            }

            foreach (DailyCondition condition in conditions) {
                if (!await context.Set<DailyCondition>().AnyAsync(c => c.Id == condition.Id)) {
                    await context.Set<DailyCondition>().AddAsync(condition);
                }
            }

            await context.SaveChangesAsync();
            logger.LogInformation($"DailyConditions added. {conditions.Count} inserted.");
        } catch (Exception e) {
            logger.LogError(e.Message);
            throw;
        }
    }
}
