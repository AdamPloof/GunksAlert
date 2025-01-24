using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GunksAlert.Api.Http;

public class ApiResponseContent {
    public enum ResponseStatus {
        Incomplete,
        Success,
        Error
    }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ResponseStatus Status { get; set; }

    [JsonPropertyName("action")]
    public required string Action { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("data")]
    public int[]? Data { get; set; }
}
