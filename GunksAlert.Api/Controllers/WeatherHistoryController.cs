using System;
using Microsoft.AspNetCore.Mvc;

using GunksAlert.Models;
using GunksAlert.Data;
using GunksAlert.Services;

namespace GunksAlert.Controllers;

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
        } else {
            return Ok(history);
        }
    }

    [HttpDelete("clear/{through?}", Name = "ClearWeatherHistory")]
    public async Task<IActionResult> ClearWeatherHistory(string through) {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        DateOnly throughDate = string.IsNullOrEmpty(through)
            ? DateOnly.FromDateTime(DateTime.Today)
            : DateOnly.ParseExact(through, "yyyy-MM-dd");

        try {
            List<int> deletedIds = await _weatherHistoryManager.ClearHistory(gunks, throughDate);

            return Ok(deletedIds);
        } catch (Exception e) {
            return Problem(e.Message, null, 500);
        }
    }
}
