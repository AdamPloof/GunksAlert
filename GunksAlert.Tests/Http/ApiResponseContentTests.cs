using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Xunit;

using GunksAlert.Api.Http;
using GunksAlert.Api.Models;

namespace GunksAlert.Tests.Http;

public class ApiResponseContentTests {
    [Fact]
    public void ResponseIsSerialized() {
        var content = new ApiResponseContent() {
            Status = ApiResponseContent.ResponseStatus.Success,
            Action = "Test",
            Model = typeof(Forecast).Name,
            Data = new int[] {42, 43, 44}
        };
        string jsonContent = JsonSerializer.Serialize<ApiResponseContent>(content);
        Assert.Contains("\"status\":\"Success\"", jsonContent);
        Assert.Contains("\"action\":\"Test\"", jsonContent);
        Assert.Contains("\"model\":\"Forecast\"", jsonContent);
        Assert.Contains("\"data\":[42,43,44]", jsonContent);
    }

    [Fact]
    public void ResponseIsDeserialized() {
        string jsonContent = """
            {
                "status": "Error",
                "action": "Test",
                "model": "WeatherHistory",
                "data": [84, 83, 81]
            }
        """;
        ApiResponseContent? content = JsonSerializer.Deserialize<ApiResponseContent>(jsonContent);

        Assert.NotNull(content);
        Assert.Equal(ApiResponseContent.ResponseStatus.Error, content.Status);
        Assert.Equal("Test", content.Action);
        Assert.Equal("WeatherHistory", content.Model);
        Assert.NotNull(content.Data);
        Assert.Equal(3, content.Data.Length);
        int[] expectedIds = [84, 83, 81];
        foreach (int id in expectedIds) {
            Assert.Contains(id, content.Data);
        }
    }
}
