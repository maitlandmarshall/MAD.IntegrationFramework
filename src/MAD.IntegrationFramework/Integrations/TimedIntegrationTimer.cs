using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal class TimedIntegrationTimer : System.Timers.Timer, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Type TimedIntegrationType { get; private set; }

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

        public TimedIntegrationTimer(Type timedIntegrationType)
        {
            this.TimedIntegrationType = timedIntegrationType;
            this.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
            this.AutoReset = true;
        }

        public bool IsIntegrationRunning()
        {
            // It has never finished || it has finished before, but has started again and is still running
            return !this.LastFinish.HasValue || this.LastStart > this.LastFinish;
        }

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
