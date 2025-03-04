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

    public void SendAlert(AppUser user, string msg) {
        _logger.LogDebug($"${user.UserName}: {msg}");
    }
}
