using System;
using System.Net.Http;
using System.Text;
using GunksAlert.Cli.Entities;

namespace GunksAlert.Cli.Services;

/// <summary>
/// WeatherManager is responsible for calling the API endpoints used in keeping
/// Forecast and WeatherHistory entities up to date.
/// </summary>
public static class WeatherManager {
    /// <summary>
    /// Refresh all weather data. Get most recent forecasts and weather
    /// history remove duplicate histories, ensure there are a minimum of
    /// 90 days of weather history.
    /// 
    /// TODO: take in crag name/ID
    /// </summary>
    /// <param name="date"></param>
    /// <returns>true if the request was successful, otherwise false</returns>
    public static async Task<bool> RefreshWeatherData() {
        string endpoint = "api/crag/refresh-weather";
        IApiResponse response = await ApiBridge.CallApi(HttpMethod.Get, endpoint);
        LogResponse(response);

        return response is ApiSuccessResponse;
    }

    /// <summary>
    /// Update weather history for a specific date
    /// </summary>
    /// <param name="date"></param>
    /// <returns>true if the request was successful, otherwise false</returns>
    public static async Task<bool> UpdateWeatherHistory(DateTime? date = null) {
        string endpoint;
        if (date == null) {
            endpoint = "api/weather-history/update";
        } else {
            endpoint = "api/weather-history/update/" + date?.ToString("yyyy-MM-dd");
        }

        IApiResponse response = await ApiBridge.CallApi(HttpMethod.Get, endpoint);
        LogResponse(response);

        return response is ApiSuccessResponse;
    }

    /// <summary>
    /// Update weather history for a between the start and end dates, inclusive
    /// </summary>
    /// <param name="date"></param>
    /// <returns>true if the request was successful, otherwise false</returns>
    public static async Task<bool> UpdateWeatherHistory(DateTime startDate, DateTime endDate) {
        StringBuilder endpoint = new StringBuilder("api/weather-history/update-range/");
        endpoint.Append(startDate.ToString("yyyy-MM-dd"));
        endpoint.Append('/');
        endpoint.Append(endDate.ToString("yyyy-MM-dd"));

        IApiResponse response = await ApiBridge.CallApi(HttpMethod.Get, endpoint.ToString());
        LogResponse(response);

        return response is ApiSuccessResponse;
    }

    /// <summary>
    /// Update the forecast data
    /// </summary>
    /// <param name="date"></param>
    /// <returns>true if the request was successful, otherwise false</returns>
    public static async Task<bool> UpdateForecast() {
        string endpoint = "api/forecast/update";
        IApiResponse response = await ApiBridge.CallApi(HttpMethod.Get, endpoint);
        LogResponse(response);

        return response is ApiSuccessResponse;
    }

    /// <summary>
    /// Remove weather history. If through is not null only remove history up to the through date (inclusive)
    /// </summary>
    /// <param name="date"></param>
    /// <returns>true if the request was successful, otherwise false</returns>
    public static async Task<bool> ClearWeatherHistory(DateTime? through = null) {
        string endpoint;
        if (through == null) {
            endpoint = "api/weather-history/clear";
        } else {
            endpoint = "api/weather-history/clear/" + through?.ToString("yyyy-MM-dd");
        }

        IApiResponse response = await ApiBridge.CallApi(HttpMethod.Delete, endpoint);
        LogResponse(response);

        return response is ApiSuccessResponse;
    }

    /// <summary>
    /// Remove current forecast data
    /// </summary>
    /// <param name="date"></param>
    /// <returns>true if the request was successful, otherwise false</returns>
    public static async Task<bool> ClearForecasts() {
        string endpoint = "api/forecast/clear";
        IApiResponse response = await ApiBridge.CallApi(HttpMethod.Delete, endpoint);
        LogResponse(response);

        return response is ApiSuccessResponse;
    }

    private static void LogResponse(IApiResponse response) {
        string uri = response.GetRequestUri().ToString();
        string resMsg = response.GetMessage();
        StringBuilder msg = new StringBuilder($"Request: {uri}");
        if (response is ApiErrorResponse) {
            msg.Append($"; Status: Error; Reason: {resMsg}");
            Logger.Error(msg.ToString());
        } else if (response is ApiSuccessResponse) {
            ApiResponseContent? content = response.GetContent();
            if (content == null) {
                msg.Append($"; Status: Success; Details: {resMsg}");
            } else {
                string data = string.Join(", ", content.Data ?? []);
                string model = content.Model ?? "";
                msg.Append($"; Status: {content.Status}; Details: {resMsg}");
                msg.Append($"; Action: {content.Action}; Model: {model}; Data: [{data}]");
            }

            Logger.Info(msg.ToString());
        }
    }
}
