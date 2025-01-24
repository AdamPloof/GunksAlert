using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

using GunksAlert.Api.Models;
using GunksAlert.Api.Data;
using GunksAlert.Api.Services;
using GunksAlert.Api.Http;

namespace GunksAlert.Api.Controllers;

[ApiController]
[Route("api/forecast")]
public class ForecastController : ControllerBase {
    private readonly GunksDbContext _context;
    private readonly ILogger<ForecastController> _logger;
    private readonly ForecastManager _forecastManager;

    public ForecastController(
        GunksDbContext context,
        ForecastManager forecastManager,
        ILogger<ForecastController> logger
    ) {
        _context = context;
        _forecastManager = forecastManager;
        _logger = logger;
    }

    [HttpGet("update", Name = "UpdateForecast")]
    public async Task<IActionResult> UpdateForecasts() {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        Forecast[]? forecasts = await _forecastManager.UpdateForecasts(gunks);
        if (forecasts == null) {
            return Problem("Unable to fetch forecasts for upcoming dates");
        }

        int[] ids = forecasts.Select(f => f.Id).ToArray();
        ApiResponseContent content = new ApiResponseContent() {
            Status = ApiResponseContent.ResponseStatus.Success,
            Action = "Update",
            Model = typeof(Forecast).Name,
            Data = ids
        };        
        
        return Ok(content);
    }

    [HttpDelete("clear", Name = "ForecastClear")]
    public async Task<IActionResult> ClearForecasts() {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        try {
            List<int> deletedIds = await _forecastManager.ClearForecasts(gunks);
            ApiResponseContent content = new ApiResponseContent() {
                Status = ApiResponseContent.ResponseStatus.Success,
                Action = "Clear",
                Model = typeof(Forecast).Name,
                Data = deletedIds.ToArray()
            };

            return Ok(content);
        } catch (Exception e) {
            return Problem(e.Message, null, 500);
        }
    }
}
