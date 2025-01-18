using System;
using Microsoft.AspNetCore.Mvc;

using GunksAlert.Models;
using GunksAlert.Data;
using GunksAlert.Services;

namespace GunksAlert.Controllers;

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

    [Route("fetch", Name = "ForecastFetch")]
    public async Task<IActionResult> FetchForecasts() {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        Forecast[]? forecasts = await _forecastManager.FetchForecasts(gunks);
        if (forecasts == null) {
            return Problem("Unable to fetch forecasts for upcoming dates");
        } else {
            return Ok(forecasts);
        }
    }

    [Route("clear", Name = "ForecastClear")]
    public async Task<IActionResult> ClearForecasts() {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        try {
            List<int> deletedIds = await _forecastManager.ClearForecasts(gunks);
            return Ok(deletedIds);
        } catch (Exception e) {
            return Problem(e.Message, null, 500);
        }
    }
}
