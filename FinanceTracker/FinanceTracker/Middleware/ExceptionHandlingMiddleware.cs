using FinanceTracker.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // The client went away. Nothing to send, nothing to log as an error.
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "{Type} on {Path}", ex.GetType().Name, context.Request.Path);

                var (status, title) = ex switch
                {
                    NotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
                    ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
                    AuthFailedException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                    _ => (StatusCodes.Status400BadRequest, "Validation Failed")
                };

                await WriteProblem(context, status, title, ex.Message, (ex as ValidationFailedException)?.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);
                await WriteProblem(context, StatusCodes.Status500InternalServerError,
                    "Internal Server Error", "An unexpected error occurred.");
            }
        }

        private static Task WriteProblem(HttpContext context, int status, string title, string detail,
            IReadOnlyDictionary<string, string[]>? errors = null)
        {
            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };
            problem.Extensions["traceId"] = context.TraceIdentifier;
            if (errors is not null) problem.Extensions["errors"] = errors;

            context.Response.StatusCode = status;
            return context.Response.WriteAsJsonAsync(problem, options: null, contentType: "application/problem+json");
        }
    }
}