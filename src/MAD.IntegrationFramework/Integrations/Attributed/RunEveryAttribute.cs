using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations.Attributed
{
    public class RunEveryAttribute : Attribute
    {
        public TimeSpan Interval { get; }

        public RunEveryAttribute(double seconds)
        {
            this.Interval = TimeSpan.FromSeconds(seconds);
        }
    }
}
