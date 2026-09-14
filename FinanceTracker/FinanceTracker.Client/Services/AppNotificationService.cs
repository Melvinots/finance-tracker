using Radzen;

namespace FinanceTracker.Client.Services
{
    public class AppNotificationService
    {
        private readonly NotificationService _notificationService;
        private readonly ILogger<AppNotificationService> _logger;

        public AppNotificationService(NotificationService notificationService, ILogger<AppNotificationService> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public void Success(string summary, string? detail = null, int duration = 3000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }

        public void Error(string summary, 
            string? detail = "Please check your connection and try again.", Exception? ex = null, int duration = 4000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });

            if (ex is not null)
            {
                _logger.LogError(ex, "{Summary}", summary);
            }
        }

        public void Warning(string summary, string? detail = null, int duration = 5000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }

        public void Info(string summary, string? detail = null, int duration = 4000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Info,
                Summary = summary,
                Detail = detail,
                Duration = duration
            });
        }
    }
}
