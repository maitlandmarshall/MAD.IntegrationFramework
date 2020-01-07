using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;

[assembly: InternalsVisibleTo("MaitlandsInterfaceFramework")]
namespace MaitlandsInterfaceFramework.Core
{
    public abstract class TimedInterface
    {
        internal TimedInterfaceServiceTimer Timer { get; set; }

        public abstract TimeSpan Interval { get; }
        public abstract bool IsEnabled { get; }

        public abstract Task Execute();
    }

    internal class TimedInterfaceServiceTimer : System.Timers.Timer, INotifyPropertyChanged
    {
        public TimedInterface TimedInterface { get; private set; }

        private DateTime? lastFinish;
        public DateTime? LastFinish
        {
            get => this.lastFinish;
            set
            {
                this.lastFinish = value;
                this.OnPropertyChanged();
            }
        }

        public DateTime? LastStart { get; set; }

        public TimedInterfaceServiceTimer(TimedInterface timedInterface)
        {
            this.Interval = timedInterface.Interval.TotalMilliseconds;
            this.AutoReset = true;
            this.TimedInterface = timedInterface;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
