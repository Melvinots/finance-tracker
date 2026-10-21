namespace FinanceTracker.Client.Infrastucture.Api
{
    public enum ApiErrorKind 
    { 
        Validation, 
        NotFound, 
        Conflict, 
        Unauthorized, 
        Network, 
        Timeout, 
        Server 
    }
}
