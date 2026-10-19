using System.Net;

namespace FinanceTracker.Client.Middleware
{
    public class ApiErrorHandler : DelegatingHandler
    {
        private readonly ILogger<ApiErrorHandler> _logger;
        private readonly NavigationManager _nav;

        public ApiErrorHandler(ILogger<ApiErrorHandler> logger, NavigationManager nav)
        {
            _logger = logger;
            _nav = nav;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken ct)
        {
            try
            {
                var response = await base.SendAsync(request, ct);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    _nav.NavigateTo("/login");

                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error calling {Url}", request.RequestUri);
                throw new ApiException("Can't reach the server. Check your connection.", ex);
            }
        }
    }

    public class ApiException : Exception
    {
        public string? Technical { get; init; }
        public ApiException(string message, Exception? inner = null) : base(message, inner) { }
    }
}
