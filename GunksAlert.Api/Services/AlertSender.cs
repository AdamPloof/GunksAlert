using System.Text;

using GunksAlert.Api.Security;
using GunksAlert.Api.Models;

namespace GunksAlert.Api.Services;

/// <summary>
/// AlertSender is responsible for sending alerts via the provided INotifier.
/// </summary>
public class AlertSender {
    private readonly ILogger _logger;

    public AlertSender(ILogger<AlertSender> logger) {
        _logger = logger;
    }

    public void SendAlerts(AppUser user, List<Alert> alerts) {
        IEnumerable<Alert> notifications = alerts.Where(a => a.Canceled == false);
        IEnumerable<Alert> cancellations = alerts.Where(a => a.Canceled == true);
        int notifyCount = notifications.Count();
        int cancelCount = cancellations.Count();
        if (notifyCount == 0 && cancelCount == 0) {
            // not notifications/cancellations to send
            return;
        }

        StringBuilder msg = new StringBuilder();
        if (notifyCount > 0) {
            // TODO: store crag in the alert so we know what crag to let people know is going to be good
            msg.Append("Looks like some good weather coming to {TODO: CRAG NAME}.");
            msg.Append("The following days will be climbable:\n");
            foreach (Alert a in notifications) {
                msg.Append($"- {a.ForecastDate.ToString("M/d/yyyy")}\n");
            }
        }

        if (cancelCount > 0) {
            // TODO: store crag in the alert so we know what crag to let people know is going to be good
            msg.Append("Looks like the weather changed for the worse at {TODO: CRAG NAME}.");
            msg.Append("The following days will be NOT be climbable:\n");
            foreach (Alert a in cancellations) {
                msg.Append($"- {a.ForecastDate.ToString("M/d/yyyy")}\n");
            }
        }
    }

    public void SendAlert(AppUser user, string msg) {
        _logger.LogDebug($"${user.UserName}: {msg}");
    }
}
