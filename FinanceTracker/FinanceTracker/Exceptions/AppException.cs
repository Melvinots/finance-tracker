namespace FinanceTracker.Exceptions
{
    public abstract class AppException(string message) : Exception(message);

    public sealed class NotFoundException(string message) : AppException(message);
    public sealed class ConflictException(string message) : AppException(message);
    public sealed class AuthFailedException(string message = "Invalid email or password.") : AppException(message);

    public sealed class ValidationFailedException : AppException
    {
        public IReadOnlyDictionary<string, string[]>? Errors { get; }

        public ValidationFailedException(string message, string? field = null) : base(message)
        {
            if (field is not null)
                Errors = new Dictionary<string, string[]> { [field] = [message] };
        }
    }
}
