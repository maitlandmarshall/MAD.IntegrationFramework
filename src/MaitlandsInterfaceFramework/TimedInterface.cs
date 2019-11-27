using System;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework
{
    public abstract class TimedInterface
    {
        internal TimedInterfaceServiceTimer Timer { get; set; }

        public abstract TimeSpan Interval { get; }
        public abstract bool IsEnabled { get; }

        public abstract Task Execute();
    }

    internal class TimedInterfaceServiceTimer : System.Timers.Timer
    {
        public TimedInterface TimedInterface { get; private set; }

        public TimedInterfaceServiceTimer(TimedInterface timedInterface)
        {
            this.Interval = timedInterface.Interval.TotalMilliseconds;
            this.AutoReset = true;
            this.TimedInterface = timedInterface;
        }
    }
}
