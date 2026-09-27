using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.DTOs.Auth
{
    public record LoginResult(bool Success, string? Error, bool WasReactivated);
}
