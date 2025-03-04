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

    public void ProcessAlerts() {
        Dictionary<AppUser, AlertSet> alertQueue = GetAlertQueue();
        foreach (KeyValuePair<AppUser, AlertSet> entry in alertQueue) {
            SendAlerts(entry.Key, entry.Value);
        }
    }

    /// <summary>
    /// Get a dictionary of users and the days that require a notification and days that
    /// require a cancelled notifcation.
    /// </summary>
    /// <returns></returns>
    public Dictionary<AppUser, AlertSet> GetAlertQueue() {
        List<AlertReport> reports = GetAlertReports();
        Dictionary<AppUser, AlertSet> alertQueue = [];
        foreach (AlertReport report in reports) {
            // Rather than notifying each user for every day separately, group notifications
            // can cancellations first so that we can send a single message with all dates.
            foreach (AppUser user in report.UsersToNotify) {
                if (!AlertRequired(user, report.TargetDate)) {
                    continue;
                }

                if (!alertQueue.ContainsKey(user)) {
                    alertQueue.Add(user, new AlertSet() {User = user});
                }

                alertQueue[user].NotifyDays.Add(report.TargetDate);
            }

            foreach (AppUser user in report.UsersToCancelNotify) {
                if (!CancelAlertRequired(user, report.TargetDate)) {
                    continue;
                }

                if (!alertQueue.ContainsKey(user)) {
                    alertQueue.Add(user, new AlertSet() {User = user});
                }

                alertQueue[user].CancelNotifyDays.Add(report.TargetDate);
            }
        }

        return alertQueue;
    }

    public List<AlertReport> GetAlertReports() {
        List<AlertReport> reports = [];
        DateOnly targetDate = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
        for (int i = 0; i > 7; i++) {
            reports.Add(GetNotificationReport(targetDate));
            targetDate = targetDate.AddDays(1);
        }

        return reports;
    }

    public AlertReport GetNotificationReport(DateOnly targetDate) {
        AlertReport alerts = new AlertReport(targetDate);
        string dayName = targetDate.DayOfWeek.ToString();
        IQueryable<AlertCriteria> criterias = _context.AlertCriterias.Where(
             // TODO: check actual days of week and check months too
            c => c.AlertPeriod.DaysOfWeek > 0
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
                criteria.AppUsers.ForEach(u => alerts.UsersToNotify.Add(u));
            } else {
                criteria.AppUsers.ForEach(u => alerts.UsersToCancelNotify.Add(u));
            }
        }

        return alerts;
    }

    public void SendAlerts(AppUser user, AlertSet alerts) {
        int notifyCount = alerts.NotifyDays.Count();
        int cancelCount = alerts.CancelNotifyDays.Count();
        if (notifyCount == 0 && cancelCount == 0) {
            // not notifications/cancellations to send
            return;
        }

        StringBuilder msg = new StringBuilder();
        if (notifyCount > 0) {
            // TODO: store crag in the alert so we know what crag to let people know is going to be good
            msg.Append("Looks like some good weather coming to {TODO: CRAG NAME}.");
            msg.Append("The following days will be climbable:\n");
            foreach (DateOnly d in alerts.NotifyDays) {
                msg.Append($"- {d.ToString("M/d/yyyy")}\n");
            }
        }

        if (cancelCount > 0) {
            // TODO: store crag in the alert so we know what crag to let people know is going to be good
            msg.Append("Looks like the weather changed for the worse at {TODO: CRAG NAME}.");
            msg.Append("The following days will be NOT be climbable:\n");
            foreach (DateOnly d in alerts.CancelNotifyDays) {
                msg.Append($"- {d.ToString("M/d/yyyy")}\n");
            }
        }

        _sender.SendAlert(user, msg.ToString());
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
