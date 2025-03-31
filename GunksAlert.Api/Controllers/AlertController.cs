using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Data;
using GunksAlert.Api.Models;
using GunksAlert.Api.Services;
using GunksAlert.Api.ViewModels;

namespace GunksAlert.Api.Controllers;

public class AlertController : Controller {
    private readonly GunksDbContext _context;
    private readonly ConditionsChecker _conditionsChecker;
    private readonly AlertManager _alertManager;
    private readonly ILogger<AlertController> _logger;

    public AlertController(
        GunksDbContext context,
        ConditionsChecker conditionsChecker,
        AlertManager alertManager,
        ILogger<AlertController> logger
    ) {
        _context = context;
        _conditionsChecker = conditionsChecker;
        _alertManager = alertManager;
        _logger = logger;
    }

    [Route("alert/sign-up", Name="AlertSignup")]
    public IActionResult Signup(AlertSignupViewModel model) {
        if (!ModelState.IsValid) {
            return View(model);
        }

        // TODO: convert response to AlertCriteria

        return View(model);
    }

    [Route("alert/process-alerts/{cragId}")]
    public IActionResult ProcessAlerts(int cragId) {
        Crag? crag = _context.Crags.Find(cragId);
        if (crag == null) {
            ViewData["status"] = "Not processed";
            return View();
        }

        _alertManager.ProcessAlerts(crag);
        ViewData["status"] = "Processed";
        
        return View();
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
        ConditionsReport report = _conditionsChecker.CheckConditions(
            crag,
            conditions,
            DateOnly.FromDateTime(today.DateTime),
            DateOnly.FromDateTime(weekend.DateTime)
        );

        return View(report);
    }
}
