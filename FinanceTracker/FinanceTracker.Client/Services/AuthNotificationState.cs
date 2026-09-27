namespace FinanceTracker.Client.Services
{
    public class AuthNotificationState
    {
        public bool WasReactivated { get; set; }

        public bool ConsumeReactivationFlag()
        {
            if (!WasReactivated) return false;
            WasReactivated = false;
            return true;
        }
    }
}
