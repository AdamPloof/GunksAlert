using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using GunksAlert.Models;
using GunksAlert.Data;
using Microsoft.EntityFrameworkCore;
using GunksAlert.Services;

namespace GunksAlert.Controllers;

public class CragController : Controller {
    private readonly GunksDbContext _context;
    private readonly ILogger<CragController> _logger;
    private readonly ForecastManager _forecastManager;

    public CragController(
        GunksDbContext context,
        ForecastManager forecastManager,
        ILogger<CragController> logger
    ) {
        _context = context;
        _forecastManager = forecastManager;
        _logger = logger;
    }

    [Route("/crag/list", Name = "Crag List")]
    public async Task<IActionResult> List() {        
        return View(await _context.Crags.ToListAsync());
    }

    [Route("/crag/forecast", Name = "Crag Forecast")]
    public async Task<IActionResult> Forecast() {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        if (await _forecastManager.FetchForecasts(gunks) == 0) {
            ViewData["forecast"] = "No forecasts saved";
        } else {
            ViewData["forecast"] = "Got em!";
        }

        return View();
    }
}
