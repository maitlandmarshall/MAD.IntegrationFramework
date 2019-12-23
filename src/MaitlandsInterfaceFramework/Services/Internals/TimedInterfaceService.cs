using MaitlandsInterfaceFramework.Core;
using MaitlandsInterfaceFramework.Core.Services.Internals;
using MaitlandsInterfaceFramework.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework.Services.Internals
{
    internal partial class TimedInterfaceService
    {
        private List<TimedInterface> TimedInterfaces = new List<TimedInterface>();
        private volatile object SyncToken = new object();

        public TimedInterfaceService()
        {
            try
            {
                this.LoadInterfaces();
                this.StartInterfaces();
            }
            catch (Exception ex)
            {
                ex.LogException().Wait();
                throw;
            }
        }

        private void LoadInterfaces()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();

            // Look through the entryAssembly for all the classes which inherit from TimedInterface and create instances of them all
            foreach (Type timedInterfaceType in entryAssembly.GetTypes().Where(y => typeof(TimedInterface).IsAssignableFrom(y) && !y.IsAbstract))
            {
                this.TimedInterfaces.Add(
                    Activator.CreateInstance(timedInterfaceType) as TimedInterface
                );
            }
        }

        private async void StartInterfaces()
        {
            // Create a timer for each interface
            foreach (TimedInterface timedInterface in this.TimedInterfaces)
            {
                TimedInterfaceServiceTimer timer = new TimedInterfaceServiceTimer(timedInterface);
                timer.Elapsed += this.Timer_Elapsed;

                timedInterface.Timer = timer;

#if DEBUG
                // Execute the interface initially, otherwise will have to wait for the first timer to elapse before anything happens.
                await this.ExecuteInterface(timedInterface);
#endif

                timer.Start();
            }
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimedInterfaceServiceTimer serviceTimer = sender as TimedInterfaceServiceTimer;
            serviceTimer.Stop();

            try
            {
                await this.ExecuteInterface(serviceTimer.TimedInterface);

                if (serviceTimer.Interval != serviceTimer.TimedInterface.Interval.TotalMilliseconds)
                    serviceTimer.Interval = serviceTimer.TimedInterface.Interval.TotalMilliseconds;
            }
            catch (Exception ex)
            {
                await ex.LogException();
                serviceTimer.Interval = TimeSpan.FromHours(6).TotalMilliseconds;
            }
            finally
            {
                serviceTimer.Start();
            }
        }

        private async Task ExecuteInterface (TimedInterface timedInterface)
        {
            TimedInterfaceDbContext db = null;
            TimedInterfaceLog log = null;

            if (String.IsNullOrEmpty(MIF.Config.SqlConnectionString))
                return;

            IScheduledInterface scheduledInterface = timedInterface as IScheduledInterface;

            try
            {
                // Load the configuration data associated with the TimedInterface before each execution
                ConfigurationService.LoadConfiguration(timedInterface);

                // Every time this interface is executed, check if it's enabled.
                if (!timedInterface.IsEnabled)
                    return;

                if (scheduledInterface != null && scheduledInterface.LastRunDateTime.HasValue)
                {
                    DateTime now = DateTime.Now;

                    if (now < scheduledInterface.NextRunDateTime)
                        return;
                }

                // If there is a connection to a datbase, create a log detailing the Start / End date and times the interface ran
                if (!String.IsNullOrEmpty(MIF.Config.SqlConnectionString))
                {
                    lock (this.SyncToken)
                    {
                        db = new TimedInterfaceDbContext();
                        log = new TimedInterfaceLog
                        {
                            InterfaceName = timedInterface.GetType().Name,
                            StartDateTime = DateTime.Now,
                            ExecutablePath = Process.GetCurrentProcess().MainModule.FileName,
                            MachineName = Environment.MachineName
                        };

                        db.TimedInterfaceLogs.Add(log);
                    }

                    await db.SaveChangesAsync();
                }

                await timedInterface.Execute();

                if (scheduledInterface != null)
                    scheduledInterface.LastRunDateTime = DateTime.Now;
            }
            finally
            {
                // Save the configuration data after each execution of the TimedInterface
                ConfigurationService.SaveConfiguration(timedInterface);

                if (db != null)
                {
                    log.EndDateTime = DateTime.Now;

                    await db.SaveChangesAsync();
                    await db.DisposeAsync();
                }
            }
        }

        internal void StopInterfaces()
        {
            foreach (TimedInterface timedInterface in this.TimedInterfaces)
            {
                timedInterface.Timer.Stop();
                timedInterface.Timer.Dispose();
            }

            this.TimedInterfaces.Clear();
        }
    }
}
