using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Models;
using GunksAlert.Api.Data;

namespace GunksAlert.Api.Services;

/// <summary>
/// WeatherHistoryManager is responsible for fetching weather history data, constructing
/// new `WeatherHistory` entities, and cleaning up outdated history.
/// </summary>
public class WeatherHistoryManager {
    private OpenWeatherBridge _openWeather;
    private readonly GunksDbContext _context;
    private string _historyPath;

    public WeatherHistoryManager(OpenWeatherBridge openWeather, GunksDbContext context) {
        _openWeather = openWeather;
        _context = context;
        _historyPath = "/data/3.0/onecall/day_summary";
    }

    /// <summary>
    /// Get the weather history for a specific data.
    /// </summary>
    /// <param name="crag"></param>
    /// <returns>The number of daily histories fetched</returns>
    public async Task<WeatherHistory?> FetchHistory(Crag crag, DateOnly date) {
        Dictionary<string, string> queryParams = new Dictionary<string, string>() {
            {"lat", crag.Latitude.ToString()},
            {"lon", crag.Longitude.ToString()},
            {"date", date.ToString("yyyy-MM-dd")},
            {"units", "imperial"},
        };

        string res = await _openWeather.Get(_historyPath, queryParams) ?? throw new Exception($"Failed to fetch history data for crag: {crag.Name}");
        JsonNode root = JsonNode.Parse(res)!;
        WeatherHistory? history = JsonSerializer.Deserialize<WeatherHistory>(root);
        if (history == null) {
            return history;
        }

        history.CragId = crag.Id;
        _context.WeatherHistories.Add(history);
        _context.SaveChanges();

        return history;
    }

    /// <summary>
    /// Delete all histories currently in the database
    /// </summary>
    /// <param name="crag"></param>
    /// <param name="through">The date before which history should be deleted</param>
    /// <returns>The Ids of the histories deleted</returns>
    public async Task<List<int>> ClearHistory(Crag crag, DateOnly through) {
        IQueryable<WeatherHistory> histories = _context.WeatherHistories.Where(
            h => h.Date < through);
        List<int> ids = histories.Select(h => h.Id).ToList();

        int deleteCount = await histories.ExecuteDeleteAsync();
        if (deleteCount == ids.Count) {
            return ids;
        } else {
            // TODO: log more details about this error
            throw new Exception("Not all histories were deleted successfully");
        }
    }
}
