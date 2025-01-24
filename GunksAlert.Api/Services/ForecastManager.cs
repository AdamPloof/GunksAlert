using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Models;
using GunksAlert.Api.Data;
using GunksAlert.Api.Http;

namespace GunksAlert.Api.Services;

/// <summary>
/// ForecastManager is responsible for fetching forecast data, creating new
/// forcast entities and cleaning up old forecasts.
/// </summary>
/// <remarks>
/// Sample forecast API call:
/// https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&exclude={part}&appid={API key}
/// </remarks>
/// <seealso href="https://openweathermap.org/api/one-call-3#current"/>
public class ForecastManager {
    private OpenWeatherBridge _openWeather;
    private readonly GunksDbContext _context;
    private string _forecastPath;

    public ForecastManager(OpenWeatherBridge openWeather, GunksDbContext context) {
        _openWeather = openWeather;
        _context = context;
        _forecastPath = "/data/3.0/onecall";
    }

    /// <summary>
    /// Get the 8 day forecast for a crag
    /// </summary>
    /// <param name="crag"></param>
    /// <returns>The number of daily forecasts fetched</returns>
    public async Task<Forecast[]?> UpdateForecasts(Crag crag) {
        Dictionary<string, string> queryParams = new Dictionary<string, string>() {
            {"lat", crag.Latitude.ToString()},
            {"lon", crag.Longitude.ToString()},
            {"exclude", "current,minutely,hourly,alerts"},
            {"units", "imperial"},
        };

        string res = await _openWeather.Get(_forecastPath, queryParams) ?? throw new Exception($"Failed to fetch forecast data for crag: {crag.Name}");
        JsonNode root = JsonNode.Parse(res)!;
        JsonNode forecastsNode = root!["daily"]!;
        Forecast[]? forecasts = JsonSerializer.Deserialize<Forecast[]>(forecastsNode);

        if (forecasts == null) {
            return forecasts;
        }

        foreach (Forecast forecast in forecasts) {
            forecast.CragId = crag.Id;
            _context.Forecasts.Add(forecast);
        }

        _context.SaveChanges();

        return forecasts;
    }

    /// <summary>
    /// Delete all forecasts currently in the database
    /// </summary>
    /// <returns>The number of rows deleted</returns>
    public async Task<List<int>> ClearForecasts(Crag crag) {
        IQueryable<Forecast> forecasts = _context.Forecasts;
        List<int> ids = forecasts.Select(f => f.Id).ToList();
        int deleteCount = await _context.Forecasts.ExecuteDeleteAsync();

        if (deleteCount != ids.Count) {
            throw new Exception("Unable to delete all forecasts");
        }

        return ids;
    }
}
