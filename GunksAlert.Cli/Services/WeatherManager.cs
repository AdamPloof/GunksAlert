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
    public static async Task UpdateWeatherHistory(DateTime? date = null) {
        string endpoint;
        if (date == null) {
            endpoint = "api/weather-history/update";
        } else {
            endpoint = "api/weather-history/update/" + date?.ToString("yyyy-MM-dd");
        }

        IApiResponse response = await ApiBridge.CallApi(HttpMethod.Get, endpoint);
        LogResponse(response);
    }

    public static async Task UpdateForecast() {
        string endpoint = "api/forecast/update";
        IApiResponse response = await ApiBridge.CallApi(HttpMethod.Get, endpoint);
        LogResponse(response);
    }

    public static async Task ClearWeatherHistory(DateTime? through = null) {
        string endpoint;
        if (through == null) {
            endpoint = "api/weather-history/clear";
        } else {
            endpoint = "api/weather-history/clear/" + through?.ToString("yyyy-MM-dd");
        }

        IApiResponse response = await ApiBridge.CallApi(HttpMethod.Delete, endpoint);
        LogResponse(response);
    }

    public static async Task ClearForecasts() {
        string endpoint = "api/forecast/clear";
        IApiResponse response = await ApiBridge.CallApi(HttpMethod.Delete, endpoint);
        LogResponse(response);
    }

    private static void LogResponse(IApiResponse response) {
        string uri = response.GetRequestUri().ToString();
        string resMsg = response.GetMessage();
        StringBuilder msg = new StringBuilder($"Request: {uri}");
        if (response is ApiErrorResponse) {
            msg.Append($", Status: Error, Reason: {resMsg}");
            Logger.Error(msg.ToString());
        } else if (response is ApiSuccessResponse) {
            ApiResponseContent? content = response.GetContent();
            if (content == null) {
                msg.Append($"Status: Success, Details: {resMsg}");
            } else {
                string data = string.Join(", ", content.Data ?? []);
                string model = content.Model ?? "";
                msg.Append($"Status: {content.Status}, Details: {resMsg}");
                msg.Append($", Action: {content.Action}, Model: {model}, Data: [{data}]");
            }

            Logger.Info(msg.ToString());
        }
    }
}
