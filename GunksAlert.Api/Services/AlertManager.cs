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

    public AlertManager(
        GunksDbContext context,
        ConditionsChecker conditionsChecker,
        AlertSender sender
    ) {
        _context = context;
        _conditionsChecker = conditionsChecker;
        _sender = sender;
    }

    public void ProcessAlerts(Crag crag) {
        Dictionary<AppUser, List<Alert>> alertsByUser = GetAlertsByUser(crag);
        foreach (KeyValuePair<AppUser, List<Alert>> entry in alertsByUser) {
            _sender.SendAlerts(entry.Key, entry.Value);
        }
    }

    public Dictionary<AppUser, List<Alert>> GetAlertsByUser(Crag crag) {
        Dictionary<AppUser, List<Alert>> userAlerts = [];
        DateOnly targetDate = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        for (int i = 0; i > 7; i++) {
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
        string dayName = targetDate.DayOfWeek.ToString();
        IQueryable<AlertCriteria> criterias = _context.AlertCriterias.Where(
             // TODO: check actual days of week and check months too
            c => c.AlertPeriod.DaysOfWeek > 0 && c.CragId == crag.Id
        );

        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        foreach (AlertCriteria criteria in criterias) {
            // TODO: an efficiency improvement here would be to sort criteria by strictness ASC and
            // then stop checking conditions once a less strict one has failed
            ClimbabilityReport report = _conditionsChecker.ConditionsReport(
                criteria.ClimbableConditions,
                today,
                targetDate
            );

            if (report.IsClimbable()) {
                criteria.AppUsers.ForEach(u => {
                    if (AlertRequired(u, targetDate)) {
                        alerts.Add(new Alert(u) {
                            CragId = crag.Id,
                            ForecastDate = targetDate,
                            SentOn = today
                        });
                    }
                });
            } else {
                criteria.AppUsers.ForEach(u => {
                    if (CancelAlertRequired(u, targetDate)) {
                        alerts.Add(new Alert(u) {
                            CragId = crag.Id,
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
