using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GunksAlert.Cli.Entities;

public class ApiResponseContent {
    [JsonPropertyName("status")]
    public required string Status { get; set; }

    [JsonPropertyName("action")]
    public required string Action { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("data")]
    public int[]? Data { get; set; }
}
