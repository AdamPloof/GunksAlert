using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using GunksAlert.Api.Data;
using GunksAlert.Api.Services;
using GunksAlert.Api.Models;
using GunksAlert.Api.Http;

namespace GunksAlert.Api.Controllers;

[ApiController]
[Route("api/crag")]
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

    [HttpGet("list", Name = "CragList")]
    public async Task<IActionResult> List() {        
        return View(await _context.Crags.ToListAsync());
    }

    // TODO: token auth
    [HttpGet("refresh-weather", Name = "RefreshWeather")]
    [AllowAnonymous]
    public async  Task<IActionResult> RefreshWeatherData() {
        Crag gunks = _context.Crags.Where(c => c.Id == 1).First();
        try {
            await _weather.RefreshWeather(gunks);
            ApiResponseContent content = new ApiResponseContent() {
                Status = ApiResponseContent.ResponseStatus.Success,
                Action = "Refresh-Weather",
                Model = "All",
                Data = new int[0]
            };

            return Ok(content);
        } catch (Exception e) {
            return Problem(e.Message, null, 500);
        }
    }
}
