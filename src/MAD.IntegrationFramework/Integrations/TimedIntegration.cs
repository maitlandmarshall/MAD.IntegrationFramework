using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

[assembly: InternalsVisibleTo("MAD.IntegrationFramework")]
namespace MAD.IntegrationFramework.Integrations
{
    public abstract class TimedIntegration
    {
        internal TimedIntegrationTimer Timer { get; set; }

        public abstract TimeSpan Interval { get; }
        public abstract bool IsEnabled { get; }

        public abstract Task Execute();
    }
}
