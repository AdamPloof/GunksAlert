using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using GunksAlert.Api.Data;
using GunksAlert.Api.Models;
using GunksAlert.Api.Security;

namespace GunksAlert.Api.Services;

/// <summary>
/// AlertManager is responsibile for checking upcoming days for climbable conditions and
/// notifying users if days are going to be climbable and un-notifying anyone how was alerted
/// but the forecast has changed for the worse.
/// </summary>
/// <remarks>
/// An overview of the steps involved:
/// 
/// - Get the climbability report for the next 7 days
/// - For each day
///     - Get all alert criteria where that day is valid
///     - Compare forecast to conditions of each criteria
///     - If day is climbable, send alert to the user(s) associated with
///        the criteria (if we haven't already)
///     - If day is not climbable, check for users who were notified for that day
///       and un-notify
/// </remarks>
public class AlertManager {
    /// <summary>
    /// A list of who to notify and who to un-notify for a specific day
    /// </summary>
    public struct AlertReport {
        public DateOnly TargetDate;
        public List<AppUser> UsersToNotify;
        public List<AppUser> UsersToCancelNotify;

        public AlertReport(DateOnly targetDate) {
            TargetDate = targetDate;
            UsersToNotify = [];
            UsersToCancelNotify = [];
        }
    }

    /// <summary>
    /// List the days that a user needs to be notified/un-notified of. This
    /// is used to allow us to send a single SMS message with the dates involved
    /// rather than sending multiple messages for each date separately.
    /// </summary>
    public struct AlertSet {
        public required AppUser User;
        public List<DateOnly> NotifyDays;
        public List<DateOnly> CancelNotifyDays;
    }

    private GunksDbContext _context;
    private ConditionsChecker _conditionsChecker;
    private AlertSender _sender;
    private ILogger _logger;

    public AlertManager(
        GunksDbContext context,
        ConditionsChecker conditionsChecker,
        AlertSender sender,
        ILogger<AlertManager> logger
    ) {
        _context = context;
        _conditionsChecker = conditionsChecker;
        _sender = sender;
        _logger = logger;
    }

    public void ProcessAlerts(Crag crag) {
        _logger.LogDebug("Processing alerts");
        Dictionary<AppUser, List<Alert>> alertsByUser = GetAlertsByUser(crag);
        foreach (KeyValuePair<AppUser, List<Alert>> entry in alertsByUser) {
            _sender.SendAlerts(entry.Key, entry.Value);
            _logger.LogDebug(
                $"Alert sent for: {entry.Key.UserName} for {entry.Value[0].ForecastDate.ToString("yyyy-mm-dd")}"
            );
        }
    }

    public Dictionary<AppUser, List<Alert>> GetAlertsByUser(Crag crag) {
        Dictionary<AppUser, List<Alert>> userAlerts = [];
        DateOnly targetDate = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        for (int i = 0; i < 7; i++) {
            List<Alert> alerts = GetAlerts(targetDate, crag);
            foreach (Alert alert in alerts) {
                if (!userAlerts.ContainsKey(alert.User)) {
                    userAlerts.Add(alert.User, new List<Alert>());
                }

                userAlerts[alert.User].Add(alert);
            }

            targetDate = targetDate.AddDays(1);
        }

        return userAlerts;
    }

    public List<Alert> GetAlerts(DateOnly targetDate, Crag crag) {
        List<Alert> alerts = [];
        int dayBit = 1 << (int)targetDate.DayOfWeek;
        int monthBit = 1 << targetDate.Month - 1;

        List<AlertCriteria> criterias = _context.AlertCriterias.Where(c => 
                c.CragId == crag.Id
                && (c.AlertPeriod.DaysOfWeek & dayBit) != 0
                && (c.AlertPeriod.Months & monthBit) != 0
        ).ToList();

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        foreach (AlertCriteria criteria in criterias) {
            // TODO: an efficiency improvement here would be to sort criteria by strictness ASC and
            // then stop checking conditions once a less strict one has failed
            _context.Entry(criteria)
                .Reference(c => c.ClimbableConditions)
                .Load();
            ConditionsReport report = _conditionsChecker.CheckConditions(
                criteria.Crag,
                criteria.ClimbableConditions,
                today,
                targetDate
            );

            if (report.IsClimbable()) {
                _logger.LogDebug($"{targetDate.ToString("yyyy-MM-dd")} is climbable, preparing alerts.");
                criteria.AppUsers.ForEach(u => {
                    if (AlertRequired(u, targetDate)) {
                        alerts.Add(new Alert() {
                            User = u,
                            UserId = u.Id,
                            CragId = crag.Id,
                            Crag = crag,
                            ForecastDate = targetDate,
                            SentOn = today
                        });
                    }
                });
            } else {
                _logger.LogDebug($"{targetDate.ToString("yyyy-MM-dd")} is not climbable, no alerts will be sent.");
                criteria.AppUsers.ForEach(u => {
                    if (CancelAlertRequired(u, targetDate)) {
                        alerts.Add(new Alert() {
                            User = u,
                            UserId = u.Id,
                            CragId = crag.Id,
                            Crag = crag,
                            ForecastDate = targetDate,
                            SentOn = today,
                            Canceled = false
                        });
                    }
                });
            }
        }

        return alerts;
    }

    /// <summary>
    /// Confirm that we haven't already sent an alert for this user on the target date or, if
    /// we have, that it has been cancelled.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="targetDate"></param>
    /// <returns></returns>
    private bool AlertRequired(AppUser user, DateOnly targetDate) {
        IQueryable<Alert> priorAlerts = _context.Alerts
            .Where(a => a.ForecastDate == targetDate && a.User == user)
            .OrderByDescending(a => a.SentOn);
        if (priorAlerts.Count() == 0) {
            return true;
        }

        return priorAlerts.First().Canceled == true;
    }

    /// <summary>
    /// Confirm that we haven't already cancelled prior alerts sent for this day or, if
    /// we have, that they have been re-alerted.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="targetDate"></param>
    /// <returns></returns>
    private bool CancelAlertRequired(AppUser user, DateOnly targetDate) {
        IQueryable<Alert> priorAlerts = _context.Alerts
            .Where(a => a.ForecastDate == targetDate && a.User == user)
            .OrderByDescending(a => a.SentOn);
        if (priorAlerts.Count() == 0) {
            return false;
        }

        return priorAlerts.First().Canceled == false;
    }
}
