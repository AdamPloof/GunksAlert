using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Data;
using GunksAlert.Api.Models;

namespace GunksAlert.Api.Services;

/// <summary>
/// WeatherManager is responsible for making sure all weather data is up
/// to date and in good order. It ensures that:
/// - The most recent forecasts are loaded
/// - At least 90 days of weather history exists
/// - Weather history is complete through yesterday
/// - There is are duplicate weather history or forecasts entities
/// </summary>
public class WeatherManager {
    private static readonly string ClearDuplicatesProcedure = "delete_duplicate_weather_data";
    private static readonly int MinForecastDays = 5;

    private GunksDbContext _context;
    private WeatherHistoryManager _historyManager;
    private ForecastManager _forecastManager;

    public WeatherManager(
        GunksDbContext context,
        WeatherHistoryManager historyManager,
        ForecastManager forecastManager
    ) {
        _context = context;
        _historyManager = historyManager;
        _forecastManager = forecastManager;
    }

    public async Task RefreshWeather(Crag crag) {
        await GetMostRecentForecast(crag);
        await FillHistory(crag);
        await ClearDuplicates(crag.Id);
    }

    /// <summary>
    /// Check:
    /// - Most recent forecasts are loaded
    /// - At least 90 days of history is present
    /// - History exists through yesterday
    /// - No duplicate forecasts or weather history exist
    /// </summary>
    /// <returns></returns>
    public bool WeatherDataIsComplete(Crag crag) {
        bool isComplete = true;
        List<DateOnly> missingHistory = MissingHistoryDates(crag);
        if (missingHistory.Count() > 0) {
            isComplete = false;
        }

        DateTimeOffset today = DateTimeOffset.Now;
        Forecast latestForecast = _context.Forecasts.Where(f => f.Date > today).Last();
        if (latestForecast.Date < today.AddDays(MinForecastDays)) {
            isComplete = false;
        }

        return isComplete;
    }

    public List<DateOnly> MissingHistoryDates(Crag crag) {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        DateOnly startHistory = today.AddDays(-90);
        List<DateOnly> history = _context.WeatherHistories
            .Where(h =>
                h.Date >= startHistory && h.CragId == crag.Id
            )
            .Select(h => h.Date)
            .OrderBy(d => d)
            .ToList();

        var missingHistory = new List<DateOnly>();
        DateOnly historyPtr = startHistory;
        while (historyPtr < today) {
            if (!history.Contains(historyPtr)) {
                DateOnly missing = historyPtr;
                missingHistory.Add(missing);
            }
            historyPtr = historyPtr.AddDays(1);
        }

        return missingHistory;
    }

    /// <summary>
    /// Check for gaps in the history of the past 90 days and fetch history for those days
    /// </summary>
    private async Task FillHistory(Crag crag) {
        List<DateOnly> missingHistory = MissingHistoryDates(crag);
        foreach (DateOnly historyDate in missingHistory) {
            WeatherHistory? history = await _historyManager.FetchHistory(crag, historyDate);
            if (history == null) {
                throw new Exception($"Could not retrieve history for {historyDate.ToString("yyyy-mm-dd")}");
            }
        }
        
    }

    private async Task GetMostRecentForecast(Crag crag) {
        Forecast[]? _ = await _forecastManager.UpdateForecasts(crag);
    }

    /// <summary>
    /// Calls a stored procedure that will remove all duplicate weather history and
    /// forecast rows (for the same date). The most recent row of the duplicate
    /// will be preserved.
    /// </summary>
    private async Task ClearDuplicates(int cragId) {
        string dedupQuery = $"CALL {ClearDuplicatesProcedure}({cragId})";
        await _context.Database.ExecuteSqlRawAsync(dedupQuery);
    }
}
