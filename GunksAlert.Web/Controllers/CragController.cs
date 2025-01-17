using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Models;
using GunksAlert.Data;
using GunksAlert.Services;

namespace GunksAlert.Controllers;

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

    [Route("/crag/weather-history/fetch/{date?}", Name = "WeatherHistoryFetch")]
    public async Task<IActionResult> FetchWeatherHistory(string date) {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        DateOnly historyDate = string.IsNullOrEmpty(date)
            ? DateOnly.FromDateTime(DateTime.Now)
            : DateOnly.ParseExact(date, "yyyy-MM-dd");
        if (await _weatherHistoryManager.FetchHistory(gunks, historyDate) == 0) {
            ViewData["history"] = "No history saved";
        } else {
            ViewData["history"] = "Got history!";
        }

        return View("WeatherHistory");
    }

    [Route("/crag/weather-history/clear/{through?}", Name = "WeatherHistoryClear")]
    public async Task<IActionResult> ClearWeatherHistory(string through) {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        DateOnly throughDate = string.IsNullOrEmpty(through)
            ? DateOnly.FromDateTime(DateTime.Now)
            : DateOnly.ParseExact(through, "yyyy-MM-dd");
        if (await _weatherHistoryManager.ClearHistory(gunks, throughDate) == 0) {
            ViewData["history"] = "No history deleted";
        } else {
            ViewData["history"] = "Removed history!";
        }

        return View("WeatherHistory");
    }

    [Route("/crag/forecast/fetch", Name = "ForecastFetch")]
    public async Task<IActionResult> FetchForecasts() {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        if (await _forecastManager.FetchForecasts(gunks) == 0) {
            ViewData["forecast"] = "No forecasts saved";
        } else {
            ViewData["forecast"] = "Got em!";
        }

        return View();
    }

    [Route("/crag/forecast/clear", Name = "ForecastClear")]
    public async Task<IActionResult> ClearForecasts() {
        Crag gunks = await _context.Crags.FindAsync(1) ?? throw new Exception("Unable to find The Gunks");
        if (await _forecastManager.ClearForecasts(gunks) == 0) {
            ViewData["forecast"] = "No forecasts deleted";
        } else {
            ViewData["forecast"] = "Removed em!";
        }

        return View();
    }
}
