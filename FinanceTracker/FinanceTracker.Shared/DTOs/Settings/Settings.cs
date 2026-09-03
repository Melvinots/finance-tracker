using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.DTOs.Settings
{
    public class SettingsDto
    {
        public string Currency { get; set; } = "PHP";
        public string Appearance { get; set; } = "light";
    }
}
