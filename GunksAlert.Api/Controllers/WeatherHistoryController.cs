using System;
using Microsoft.AspNetCore.Mvc;

using GunksAlert.Api.Models;
using GunksAlert.Api.Data;
using GunksAlert.Api.Services;
using GunksAlert.Api.Http;

namespace GunksAlert.Api.Controllers;

[ApiController]
[Route("api/weather-history")]
public class WeatherHistoryController : ControllerBase {
    private readonly GunksDbContext _context;
    private readonly ILogger<WeatherHistoryController> _logger;
    private readonly WeatherHistoryManager _weatherHistoryManager;

    public WeatherHistoryController(
        GunksDbContext context,
        WeatherHistoryManager weatherHistoryManager,
        ILogger<WeatherHistoryController> logger
    ) {
        _context = context;
        _weatherHistoryManager = weatherHistoryManager;
        _logger = logger;
    }

    [HttpGet("update/{date?}", Name = "UpdateWeatherHistory")]
    public async Task<IActionResult> UpdateWeatherHistory(string date) {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        DateOnly historyDate = string.IsNullOrEmpty(date)
            ? DateOnly.FromDateTime(DateTime.Today.AddDays(-1))
            : DateOnly.ParseExact(date, "yyyy-MM-dd");
        
        WeatherHistory? history = await _weatherHistoryManager.FetchHistory(gunks, historyDate);
        if (history == null) {
            string dateStr = historyDate.ToString("yyyy-mm-dd");
            return Problem($"Unable to fetch history for date: {dateStr}", null, 500);
        }

        ApiResponseContent content = new ApiResponseContent() {
            Status = ApiResponseContent.ResponseStatus.Success,
            Action = "Update",
            Model = typeof(WeatherHistory).Name,
            Data = [history.Id]
        };

        return Ok(content);
    }

    [HttpDelete("clear/{through?}", Name = "ClearWeatherHistory")]
    public async Task<IActionResult> ClearWeatherHistory(string through) {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        DateOnly throughDate = string.IsNullOrEmpty(through)
            ? DateOnly.FromDateTime(DateTime.Today)
            : DateOnly.ParseExact(through, "yyyy-MM-dd");

        try {
            List<int> deletedIds = await _weatherHistoryManager.ClearHistory(gunks, throughDate);
            ApiResponseContent content = new ApiResponseContent() {
                Status = ApiResponseContent.ResponseStatus.Success,
                Action = "Clear",
                Model = typeof(WeatherHistory).Name,
                Data = deletedIds.ToArray()
            };

            return Ok(content);
        } catch (Exception e) {
            return Problem(e.Message, null, 500);
        }
    }
}
