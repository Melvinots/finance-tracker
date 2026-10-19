using FinanceTracker.Client.Middleware;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FinanceTracker.Client.Extensions
{
    public static class HttpResponseExtensions
    {
        private const string GenericMessage = "Something went wrong. Please try again later.";

        public static async Task<T> ReadOrThrowAsync<T>(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<T>();
                return result ?? throw new ApiException("The server returned an empty response.")
                {
                    Technical = $"Empty body from {response.RequestMessage?.RequestUri}"
                };
            }

            throw await BuildExceptionAsync(response);
        }

        public static async Task<(T? Result, string? Error)> TryReadAsync<T>(this HttpResponseMessage response)
        {
            try
            {
                var result = await response.ReadOrThrowAsync<T>();
                return (result, null);
            }
            catch (ApiException ex)
            {
                return (default, ex.Message);
            }
        }

        public static async Task<T?> ReadOrDefaultAsync<T>(this HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
                return default;

            return await response.ReadOrThrowAsync<T>();
        }

        public static async Task EnsureSuccessOrThrowAsync(this HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;
            throw await BuildExceptionAsync(response);
        }

        private static async Task<ApiException> BuildExceptionAsync(HttpResponseMessage response)
        {
            var problem = await TryReadProblemAsync(response);
            var serverMessage = problem?.Detail ?? problem?.Title;
            var technical = $"{response.RequestMessage?.Method} {response.RequestMessage?.RequestUri} -> {(int)response.StatusCode}";

            return response.StatusCode switch
            {
                // Wrong route: nothing the user can fix
                HttpStatusCode.NotFound when problem is null
                    => new ApiException(GenericMessage) { Technical = technical },

                HttpStatusCode.NotFound => new ApiException(serverMessage ?? "Item not found.") { Technical = technical },
                HttpStatusCode.BadRequest => new ApiException(serverMessage ?? "Please check your input and try again.") { Technical = technical },
                HttpStatusCode.Unauthorized => new ApiException(serverMessage ?? "Your session has expired. Please log in again.") { Technical = technical },
                HttpStatusCode.Conflict => new ApiException(serverMessage ?? "This conflicts with existing data.") { Technical = technical },

                // 500s and anything unexpected: never show server text or status codes
                _ => new ApiException(GenericMessage) { Technical = technical }
            };
        }

        private static async Task<ProblemDetails?> TryReadProblemAsync(HttpResponseMessage r)
        {
            try { return await r.Content.ReadFromJsonAsync<ProblemDetails>(); }
            catch { return null; }
        }
    }
}
