using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Data;
using GunksAlert.Api.Services;

namespace GunksAlert.Api.Controllers;

public class CragController : Controller {
    private readonly GunksDbContext _context;
    private readonly ILogger<CragController> _logger;
    private readonly ForecastManager _forecastManager;
    private readonly WeatherHistoryManager _weatherHistoryManager;

    public CragController(
        GunksDbContext context,
        ForecastManager forecastManager,
        WeatherHistoryManager weatherHistoryManager,
        ILogger<CragController> logger
    ) {
        _context = context;
        _forecastManager = forecastManager;
        _weatherHistoryManager = weatherHistoryManager;
        _logger = logger;
    }

    [Route("/crag/list", Name = "CragList")]
    public async Task<IActionResult> List() {        
        return View(await _context.Crags.ToListAsync());
    }
}
