using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using GunksAlert.Cli.Entities;

namespace GunksAlert.Cli;

/// <summary>
/// Wrapper around HttpClient for sending requests to GunksAlert.Api and processing responses
/// </summary>
public static class ApiBridge {
    static readonly HttpClient _client = new HttpClient() {BaseAddress = new Uri("https://localhost:7108")};

    public static async Task<IApiResponse> CallApi(HttpMethod method, string path) {
        var request = new HttpRequestMessage(method, path);
        try {
            HttpResponseMessage response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string strContent = await response.Content.ReadAsStringAsync();
            ApiResponseContent? content = await response.Content.ReadFromJsonAsync<ApiResponseContent>();
            if (content == null) {
                return new ApiErrorResponse(request.RequestUri!, "No response details provided");
            } else {
                return new ApiSuccessResponse(request.RequestUri!, "Success", content);
            }
        } catch (HttpRequestException e) {
            return new ApiErrorResponse(request.RequestUri!, e.Message.ToString());
        }
    }
}
