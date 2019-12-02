using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
                TimedInterfaceServiceTimer timer = new TimedInterfaceServiceTimer(timedInterface);
                timer.Elapsed += this.Timer_Elapsed;

                timedInterface.Timer = timer;

                // Execute the interface initially, otherwise will have to wait for the first timer to elapse before anything happens.
                await this.ExecuteInterface(timedInterface);

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
            }
            finally
            {
                serviceTimer.Start();
            }
        }

        private async Task ExecuteInterface (TimedInterface timedInterface)
        {
            // Every time this interface is executed, check if it's enabled.
            if (!timedInterface.IsEnabled)
                return;

            try
            {
                // Load the configuration data associated with the TimedInterface before each execution
                ConfigurationService.LoadConfiguration(timedInterface);
                await timedInterface.Execute();
            }
            catch (Exception ex)
            {
                await ex.LogException();
            }
            finally
            {
                // Save the configuration data after each execution of the TimedInterface
                ConfigurationService.SaveConfiguration(timedInterface);
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
