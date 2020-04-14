using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MAD.IntegrationFramework.Integrations.Attributed
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class DailyAttribute : Attribute
    {
        public int StartSecond { get; set; }
        public int StartMinute { get; set; }
        public int StartHour { get; set; }
    }
}
