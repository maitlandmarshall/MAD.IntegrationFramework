using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TimedIntegrationAttribute : Attribute
    {
        public TimeSpan Frequency { get; set; }

        public TimedIntegrationAttribute(TimeSpan frequency)
        {
            this.Frequency = frequency;
        }
    }
}
