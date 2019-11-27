using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MaitlandsInterfaceFramework.Services.Internals
{
    internal class TimedInterfaceService
    {
        private List<TimedInterface> TimedInterfaces = new List<TimedInterface>();

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
                if (!timedInterface.IsEnabled)
                    continue;

                TimedInterfaceServiceTimer timer = new TimedInterfaceServiceTimer(timedInterface);
                timer.Elapsed += this.Timer_Elapsed;

                timedInterface.Timer = timer;

                // Execute the interface initially, otherwise will have to wait for the first timer to elapse before anything happens.
                try
                {
                    await timedInterface.Execute();
                }
                catch (Exception ex)
                {
                    await ex.LogException();
                }

                timer.Start();
            }
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimedInterfaceServiceTimer serviceTimer = sender as TimedInterfaceServiceTimer;
            serviceTimer.Stop();

            try
            {
                await serviceTimer.TimedInterface.Execute();
            }
            catch (Exception ex)
            {
                await ex.LogException();
            }
            finally
            {
                serviceTimer.Start();
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
