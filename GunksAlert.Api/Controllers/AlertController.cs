using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using GunksAlert.Api.Data;
using GunksAlert.Api.Models;
using GunksAlert.Api.Services;
using GunksAlert.Api.ViewModels;
using GunksAlert.Api.Security;

namespace GunksAlert.Api.Controllers;

public class AlertController : Controller {
    private readonly GunksDbContext _context;
    private readonly ConditionsChecker _conditionsChecker;
    private readonly AlertManager _alertManager;
    private readonly UserManager<AppUser> _userManager;

    private readonly ILogger<AlertController> _logger;

    public AlertController(
        GunksDbContext context,
        ConditionsChecker conditionsChecker,
        AlertManager alertManager,
        UserManager<AppUser> userManager,
        ILogger<AlertController> logger
    ) {
        _context = context;
        _conditionsChecker = conditionsChecker;
        _alertManager = alertManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet("alert/sign-up", Name="AlertSignup")]
    public IActionResult Signup() {
        return View();
    }

    [HttpPost("alert/sign-up", Name="AlertSignupSubmit")]
    public async Task<IActionResult> Signup(AlertSignupViewModel model) {
        if (!ModelState.IsValid) {
            return View(model);
        }

        AppUser? user = await _userManager.GetUserAsync(User);
        if (user == null) {
            // This shouldn't ever happen
            return Unauthorized();
        }

        AlertPeriod period = new AlertPeriod();
        // Not allowing the user to set start/end dates for now
        period.StartDate = DateTimeOffset.MinValue;
        period.EndDate = DateTimeOffset.MaxValue;
        period.SetMonths(model.GetMonthNames());
        period.SetDaysOfWeek(model.GetWeekdayNames());
        _context.AlertPeriods.Add(period);

        Crag crag = _context.Crags.Where(c => c.Name == "Gunks").First();
        ClimbableConditions conditions = _context.ClimbableConditions.First();
        AlertCriteria criteria = new AlertCriteria() {
            Crag = crag,
            AlertPeriod = period,
            ClimbableConditions = conditions
        };
        user.AddCriteria(criteria);

        _context.SaveChanges();

        return RedirectToAction("Index", "Home");
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
