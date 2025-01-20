using System;
using System.Net.Http;
using System.Text;
using GunksAlert.Cli.Entities;

namespace GunksAlert.Cli.Services;

/// <summary>
/// WeatherManager is responsible for calling the API endpoints used in keeping
/// Forecast and WeatherHistory entities up to date.
/// </summary>
public class WeatherManager {
    private ApiBridge _api;

    public WeatherManager(ApiBridge api) {
        _api = api;
    }

    public async void UpdateWeatherHistory() {
        string endpoint = "api/weather-history/update";
        IApiResponse response = await _api.CallApi(HttpMethod.Get, endpoint);
        LogResponse(response);
    }

    public async void UpdateForecast() {
        string endpoint = "api/forecast/update";
        IApiResponse response = await _api.CallApi(HttpMethod.Get, endpoint);
        LogResponse(response);
    }

    public async void ClearWeatherHistory() {
        string endpoint = "api/weather-history/clear";
        IApiResponse response = await _api.CallApi(HttpMethod.Delete, endpoint);
        LogResponse(response);
    }

    public async void ClearForecasts() {
        string endpoint = "api/forecast/clear";
        IApiResponse response = await _api.CallApi(HttpMethod.Delete, endpoint);
        LogResponse(response);
    }

    private void LogResponse(IApiResponse response) {
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
