using FinanceTracker.Client.Infrastucture.Api;
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
                throw new ApiException(ApiErrorKind.Network, "Can't reach the server. Check your connection.", ex)
                { Technical = $"{request.Method} {request.RequestUri}" };
            }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
            {
                throw new ApiException(ApiErrorKind.Timeout, "The server took too long to respond.", ex)
                { Technical = $"{request.Method} {request.RequestUri}" };
            }
        }
    }
}
