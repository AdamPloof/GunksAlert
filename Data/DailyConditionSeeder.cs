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
public class DailyConditionSeeder {
    private ILogger _logger;
    private GunksDbContext _context;

    public DailyConditionSeeder(GunksDbContext context, ILogger logger) {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(string dataPath) {
        if (!_context.Database.EnsureCreated()) {
            throw new ArgumentException("Can't initalize data. Database has not been created.");
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
                _logger.LogWarning("Conditions data file is empty. No conditions were added");
                return;
            }

            foreach (DailyCondition condition in conditions) {
                if (!await _context.Set<DailyCondition>().AnyAsync(c => c.Id == condition.Id)) {
                    await _context.Set<DailyCondition>().AddAsync(condition);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"DailyConditions added. {conditions.Count} inserted.");
        } catch (Exception e) {
            _logger.LogError(e.Message);
            throw;
        }
    }
}
