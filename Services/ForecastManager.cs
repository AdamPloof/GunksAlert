using System.Collections.Generic;
using GunksAlert.Models;

namespace GunksAlert.Services;

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
    private string _forecastPath;

    public ForecastManager(OpenWeatherBridge openWeather)  {
        _openWeather = openWeather;
        _forecastPath = "/";
    }

    /// <summary>
    /// Get the 8 day forecast for a crag
    /// </summary>
    /// <param name="crag"></param>
    /// <returns>The forecast as a JSON string (TODO: return Forecast)</returns>
    public async Task<string> GetForecast(Crag crag) {
        Dictionary<string, string> queryParams = new Dictionary<string, string>() {
            {"lat", crag.Latitude.ToString()},
            {"lon", crag.Longitude.ToString()},
            {"exclude", "current,minutely,hourly,alerts"},
            {"units", "imperial"},
        };

        return await _openWeather.Get(_forecastPath, queryParams) ?? throw new Exception($"Failed to fetch forecast data for crag: {crag.Name}");
    }
}
