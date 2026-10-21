using FinanceTracker.Client.Infrastucture.Api;
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
                Detail = detail ?? "",
                Duration = duration,
                Payload = DateTime.Now.Ticks
            });
        }

        public void Error(string summary, string? detail = "Please check your connection and try again.", Exception? ex = null, int duration = 4000)
        {
            if (ex is OperationCanceledException) return;

            var message = detail ?? "An unexpected error occurred.";

            if (ex is ApiException api)
            {
                message = api.Message;

                if (api.Kind == ApiErrorKind.Server && !string.IsNullOrEmpty(api.TraceId))
                {
                    message += $" (Ref: {api.TraceId})";
                    duration = Math.Max(duration, 8000);
                }
            }

            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = summary,
                Detail = message,
                Duration = duration,
                Payload = DateTime.Now.Ticks
            });

            Log(summary, ex);
        }

        private void Log(string summary, Exception? ex)
        {
            switch (ex)
            {
                case null:
                    break;

                // Expected outcomes (validation, conflict, not found, network): not bugs
                case ApiException { Kind: not ApiErrorKind.Server } api:
                    _logger.LogWarning(api, "{Summary}. {Technical} TraceId={TraceId}",
                        summary, api.Technical, api.TraceId);
                    break;

                // Server faults: these deserve attention
                case ApiException api:
                    _logger.LogError(api, "{Summary}. {Technical} TraceId={TraceId}",
                        summary, api.Technical, api.TraceId);
                    break;

                // Anything else is an unexpected client bug
                default:
                    _logger.LogError(ex, "{Summary}", summary);
                    break;
            }
        }

        public void Warning(string summary, string? detail = null, int duration = 5000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = summary,
                Detail = detail ?? "",
                Duration = duration,
                Payload = DateTime.Now.Ticks
            });
        }

        public void Info(string summary, string? detail = null, int duration = 4000)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Info,
                Summary = summary,
                Detail = detail ?? "",
                Duration = duration,
                Payload = DateTime.Now.Ticks
            });
        }
    }
}
