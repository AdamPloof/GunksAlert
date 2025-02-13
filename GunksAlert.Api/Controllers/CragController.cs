using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Data;
using GunksAlert.Api.Services;
using GunksAlert.Api.Models;

namespace GunksAlert.Api.Controllers;

public class CragController : Controller {
    private readonly GunksDbContext _context;
    private readonly ILogger<CragController> _logger;
    private readonly WeatherManager _weather;

    public CragController(
        GunksDbContext context,
        WeatherManager weather,
        ILogger<CragController> logger
    ) {
        _context = context;
        _weather = weather;
        _logger = logger;
    }

    [Route("/crag/list", Name = "CragList")]
    public async Task<IActionResult> List() {        
        return View(await _context.Crags.ToListAsync());
    }

    [Route("/crag/refresh-weather")]
    public IActionResult RefreshWeatherData() {
        Crag gunks = _context.Crags.Where(c => c.Id == 1).First();
        List<DateOnly> missingHistory = _weather.MissingHistoryDates(gunks);

        return View();
    }
}
