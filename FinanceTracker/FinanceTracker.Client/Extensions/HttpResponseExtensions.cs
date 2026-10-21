using FinanceTracker.Client.Infrastucture.Api;
using System.Net;
using System.Text.Json;

namespace FinanceTracker.Client.Extensions
{
    public static class HttpResponseExtensions
    {
        public static async Task<T> ReadOrThrowAsync<T>(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw await BuildExceptionAsync(response);

            var result = await response.Content.ReadFromJsonAsync<T>();
            return result ?? throw new ApiException(ApiErrorKind.Server, ApiException.GenericMessage)
            {
                Technical = $"Empty body from {response.RequestMessage?.RequestUri}"
            };
        }

        public static async Task<T?> ReadOrDefaultAsync<T>(this HttpResponseMessage response)
            => response.StatusCode == HttpStatusCode.NotFound ? default : await response.ReadOrThrowAsync<T>();

        public static async Task EnsureSuccessOrThrowAsync(this HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw await BuildExceptionAsync(response);
        }

        public static async Task<(T? Result, string? Error)> TryReadAsync<T>(this HttpResponseMessage response)
        {
            try { return (await response.ReadOrThrowAsync<T>(), null); }
            catch (ApiException ex) { return (default, ex.Message); }
        }

        private static async Task<ApiException> BuildExceptionAsync(HttpResponseMessage response)
        {
            var problem = await TryReadProblemAsync(response);
            var errors = problem?.Errors ?? new Dictionary<string, string[]>();
            var detail = errors.Values.FirstOrDefault()?.FirstOrDefault() ?? problem?.Detail;
            var technical = $"{response.RequestMessage?.Method} {response.RequestMessage?.RequestUri} -> {(int)response.StatusCode}";

            var (kind, message) = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => (ApiErrorKind.Validation, detail ?? "Please check your input and try again."),
                HttpStatusCode.NotFound when problem is null => (ApiErrorKind.Server, ApiException.GenericMessage),
                HttpStatusCode.NotFound => (ApiErrorKind.NotFound, detail ?? "Item not found."),
                HttpStatusCode.Conflict => (ApiErrorKind.Conflict, detail ?? "This conflicts with existing data."),
                HttpStatusCode.Unauthorized => (ApiErrorKind.Unauthorized, detail ?? "Your session has expired. Please log in again."),
                _ => (ApiErrorKind.Server, ApiException.GenericMessage)
            };

            return new ApiException(kind, message)
            {
                Errors = errors,
                TraceId = problem?.TraceId,
                Technical = technical
            };
        }

        private static async Task<ProblemDetailsDto?> TryReadProblemAsync(HttpResponseMessage r)
        {
            if (r.Content.Headers.ContentType?.MediaType is not ("application/problem+json" or "application/json"))
                return null;

            try { return await r.Content.ReadFromJsonAsync<ProblemDetailsDto>(); }
            catch (JsonException) { return null; }
        }
    }
}
