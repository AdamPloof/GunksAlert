using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Data;
using GunksAlert.Api.Models;
using GunksAlert.Api.Services;

namespace GunksAlert.Api.Controllers;

public class AlertController : Controller {
    private readonly GunksDbContext _context;
    private readonly ConditionsChecker _conditionsChecker;
    private readonly ILogger<AlertController> _logger;

    public AlertController(
        GunksDbContext context,
        ConditionsChecker conditionsChecker,
        ILogger<AlertController> logger
    ) {
        _context = context;
        _conditionsChecker = conditionsChecker;
        _logger = logger;
    }

    [Route("/alert/check-conditions/{cragId}", Name = "CheckConditions")]
    public IActionResult CheckConditions(int cragId) {
        Crag? crag = _context.Crags.Find(cragId);
        if (crag == null) {
            ViewData["isClimbable"] = $"nope {cragId}";

            return View();
        } else {
            ViewData["isClimbable"] = $"yep {crag.Name}";
        }

        DateTimeOffset today = DateTimeOffset.Now;
        ClimbableConditions conditions = new ClimbableConditions() {
            Summary = "A good day at the Gunks",
            TempMin = 40,
            TempMax = 85,
            WindSpeed = 4,
            WindGust = 10,
            WindDegree = 200,
            Clouds = 20,
            Humidity = 20,
            Pop = 0,
            Rain = 0.0,
            Snow = 0.0
        };
        DateTimeOffset weekend = today.AddDays(5);
        ClimbabilityReport report = _conditionsChecker.ConditionsReport(
            conditions,
            DateOnly.FromDateTime(today.DateTime),
            DateOnly.FromDateTime(weekend.DateTime)
        );

        return View(report);
    }
}
