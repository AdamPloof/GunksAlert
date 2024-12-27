using System;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using NuGet.Protocol;

namespace GunksAlert.Services;

/// <summary>
/// OpenWeatherBridge handles sending and parsing API requests to OpenWeatherMap's API
/// </summary>
/// <seealso href="https://openweathermap.org/api/one-call-3"/>
public class OpenWeatherBridge {
    private string _key;
    private HttpClient _openWeather;

    public OpenWeatherBridge(IConfiguration config, IHttpClientFactory clientFactory) {
        _key = config.GetValue<string>("OpenWeatherKey") ?? throw new ArgumentException("API key not set");
        _openWeather = clientFactory.CreateClient("OpenWeather");
        _openWeather.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
        _openWeather.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "weather@gunksalert.com");
        _openWeather.BaseAddress = new Uri("https://api.openweathermap.org/data/3.0/onecall");
    }

    public async Task<string?> Get(string path, Dictionary<string, string> queryParams) {
        StringBuilder query = new StringBuilder($"?appid={_key}");
        foreach (KeyValuePair<string, string> p in queryParams) {
            query.Append($"&{p.Key}={p.Value}");
        }
        
        if (_openWeather.BaseAddress == null) {
            throw new ArgumentNullException("Base address of OpenWeather client is null");
        }

        Uri uri = new Uri(_openWeather.BaseAddress, path + query.ToString());

        try {
            using HttpResponseMessage res = await _openWeather.GetAsync(uri);
            res.EnsureSuccessStatusCode();

            return await res.Content.ReadAsStringAsync();
        } catch (HttpRequestException e) {
            // TODO log the exception.
            Console.WriteLine($"Request failed: {e.Message}");
            throw;
        }
    }
}
