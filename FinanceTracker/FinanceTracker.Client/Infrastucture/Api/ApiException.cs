namespace FinanceTracker.Client.Infrastucture.Api
{
    public class ApiException : Exception
    {
        public const string GenericMessage = "Something went wrong. Please try again later.";

        public ApiErrorKind Kind { get; }
        public IReadOnlyDictionary<string, string[]> Errors { get; init; } = new Dictionary<string, string[]>();
        public string? TraceId { get; init; }
        public string? Technical { get; init; }

        public ApiException(ApiErrorKind kind, string message, Exception? inner = null) : base(message, inner)
            => Kind = kind;
    }
}
