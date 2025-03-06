using System.Text;

using GunksAlert.Api.Security;
using GunksAlert.Api.Models;
using GunksAlert.Api.Data;

namespace GunksAlert.Api.Services;

/// <summary>
/// AlertSender is responsible for sending alerts via the provided INotifier.
/// </summary>
public class AlertSender {
    private readonly GunksDbContext _context;
    private readonly ILogger _logger;

    public AlertSender(GunksDbContext context, ILogger<AlertSender> logger) {
        _context = context;
        _logger = logger;
    }

    public void SendAlerts(AppUser user, List<Alert> alerts) {
        IEnumerable<Alert> notifications = alerts.Where(a => a.Canceled == false);
        IEnumerable<Alert> cancellations = alerts.Where(a => a.Canceled == true);
        int notifyCount = notifications.Count();
        int cancelCount = cancellations.Count();
        if (notifyCount == 0 && cancelCount == 0) {
            // no notifications/cancellations to send
            return;
        }

        StringBuilder msg = new StringBuilder();
        if (notifyCount > 0) {
            msg.Append("Looks like some good climbing weather on the way.");
            msg.Append("The following days will be climbable:\n");
            foreach (Alert a in notifications) {
                _context.Alerts.Add(a);
                msg.Append($"- {a.Crag.Name}: {a.ForecastDate.ToString("M/d/yyyy")}\n");
            }
        }

        if (cancelCount > 0) {
            msg.Append("The weather changed for the worse.");
            msg.Append("The following days will be NOT be climbable:\n");
            foreach (Alert a in cancellations) {
                _context.Alerts.Add(a);
                msg.Append($"- {a.Crag.Name}: {a.ForecastDate.ToString("M/d/yyyy")}\n");
            }
        }

        _context.SaveChanges();
        SendAlert(user, msg.ToString());
    }

    public void SendAlert(AppUser user, string msg) {
        _logger.LogDebug($"${user.UserName}: {msg}");
    }
}
