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
    private static readonly int MaxApiCallCount = 100;

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

    [HttpGet("update-range/{startDate}/{endDate}", Name = "UpdateWeatherHistoryRange")]
    public async Task<IActionResult> UpdateWeatherHistoryRange(string startDate, string endDate) {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        if (!DateOnly.TryParseExact(startDate, "yyyy-MM-dd", out DateOnly start)) {
            return Problem($"Start date must be in yyyy-MM-dd format. Got: {startDate}");
        }

        if (!DateOnly.TryParseExact(endDate, "yyyy-MM-dd", out DateOnly end)) {
            return Problem($"End date must be in yyyy-MM-dd format. Got: {endDate}");
        }

        if (start > end) {
            return Problem($"Start date {startDate} must be before end date {endDate}");
        }

        List<WeatherHistory> histories = new();
        DateOnly historyDate = start;
        int callCount = 0;
        while (historyDate <= end) {
            if (callCount > MaxApiCallCount) {
                // Just in case the loop runs wild, prevent making too many API calls
                break;
            }

            // TODO: should probably roll back histories that were successfully added
            WeatherHistory? history = await _weatherHistoryManager.FetchHistory(gunks, historyDate);
            if (history == null) {
                string dateStr = historyDate.ToString("yyyy-mm-dd");
                return Problem($"Unable to fetch history for date: {dateStr}", null, 500);
            }

            histories.Add(history);
            historyDate = historyDate.AddDays(1);
            callCount++;
        }
        
        ApiResponseContent content = new ApiResponseContent() {
            Status = ApiResponseContent.ResponseStatus.Success,
            Action = "Update",
            Model = typeof(WeatherHistory).Name,
            Data = [.. histories.Select(h => h.Id)]
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
