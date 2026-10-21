namespace FinanceTracker.Client.Infrastucture.Api
{
    public class ProblemDetailsDto
    {
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public int? Status { get; set; }
        public string? TraceId { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}
