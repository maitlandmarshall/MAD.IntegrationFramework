using MAD.IntegrationFramework.Core;
using MAD.IntegrationFramework.Core.Services.Internals;
using MAD.IntegrationFramework.Database;
using MAD.IntegrationFramework.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Integrations
{
    internal class TimedIntegrationController
    {
        private object syncToken = new object();

        private readonly ILogger<TimedIntegrationController> logger;
        private readonly IExceptionLogger exceptionLogger;
        private readonly IntegrationConfigurationService integrationConfigurationService;

        private List<TimedIntegrationTimer> timedIntegrationTimers;

        public TimedIntegrationController(ILogger<TimedIntegrationController> logger, IExceptionLogger exceptionLogger, IntegrationConfigurationService integrationConfigurationService)
        {
            this.logger = logger;
            this.exceptionLogger = exceptionLogger;
            this.integrationConfigurationService = integrationConfigurationService;

            this.timedIntegrationTimers = new List<TimedIntegrationTimer>();
        }

        private IEnumerable<Type> GetTimedIntegrationTypesInAssembly(Assembly assembly)
        {
            // Look through the assembly and yield all the classes which inherit from TimedIntegration
            foreach (Type timedInterfaceType in assembly.GetTypes().Where(y => typeof(TimedIntegration).IsAssignableFrom(y) && !y.IsAbstract))
            {
                yield return timedInterfaceType;
            }
        }

        internal async Task StartAsync()
        {
            IEnumerable<Type> timedIntegrationTypesInEntryAssembly = this.GetTimedIntegrationTypesInAssembly(Assembly.GetEntryAssembly());

            // Create a timer for each interface
            foreach (Type timedIntegrationType in timedIntegrationTypesInEntryAssembly)
            {
                TimedIntegrationTimer timedIntegrationTimer = new TimedIntegrationTimer(timedIntegrationType);
                timedIntegrationTimer.Elapsed += this.TimedIntegrationTimer_Elapsed;

                this.timedIntegrationTimers.Add(timedIntegrationTimer);
            }

            foreach (TimedIntegrationTimer timedInterfaceTimer in this.timedIntegrationTimers)
            {
#if DEBUG
                await this.HandleRunAfterAttributeForTimedInterface(timedInterfaceTimer.TimedIntegrationType);

                // Execute the interface initially, otherwise will have to wait for the first timer to elapse before anything happens.
                await this.ExecuteInterface(timedInterfaceTimer.TimedIntegrationType);
#endif

                timedInterfaceTimer.Start();
            }
        }

        private async void TimedIntegrationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimedIntegrationTimer serviceTimer = sender as TimedIntegrationTimer;
            serviceTimer.Stop();

            try
            {
                await this.HandleRunAfterAttributeForTimedInterface(serviceTimer.TimedIntegrationType);
                await this.ExecuteInterface(serviceTimer.TimedIntegrationType);

                if (serviceTimer.Interval != serviceTimer.TimedIntegrationType.Interval.TotalMilliseconds)
                    serviceTimer.Interval = serviceTimer.TimedIntegrationType.Interval.TotalMilliseconds;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                await this.exceptionLogger.LogException(ex, serviceTimer.TimedIntegrationType.GetType().Name);

                serviceTimer.Interval = TimeSpan.FromHours(6).TotalMilliseconds;
            }
            finally
            {
                serviceTimer.Start();
            }
        }

        private async Task HandleRunAfterAttributeForTimedIntegration(Type timedIntegrationType)
        {
            RunAfterAttribute runAfterAttribute = timedIntegrationType.GetCustomAttribute<RunAfterAttribute>();

            if (runAfterAttribute == null)
                throw new MissingAttributeException(typeof(RunAfterAttribute));

            Type integrationTypeToRunAfter = this.timedIntegrationTimers.FirstOrDefault(y => y.TimedIntegrationType == runAfterAttribute.IntegrationTypeToRunAfter).TimedIntegrationType;

            if (integrationTypeToRunAfter == null)
                throw new Exception($"{timedIntegrationType.GetType().Name} cannot run after {runAfterAttribute.IntegrationTypeToRunAfter.Name} as it can't be found");

            // If the interface to run after hasn't completed a first run, or it has started again but hasn't finished
            if (!integrationTypeToRunAfter.LastFinish.HasValue || integrationTypeToRunAfter.LastStart > integrationTypeToRunAfter.LastFinish)
            {
                TaskCompletionSource<bool> waitForFinishTaskCompletionSource = new TaskCompletionSource<bool>();
                interfaceToRunAfterTimer.PropertyChanged += InterfaceToRunAfterTimer_PropertyChanged;

                void InterfaceToRunAfterTimer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    // When the LastFinish property is updated, set the CompletionSource result so program flow may continue;
                    if (e.PropertyName == nameof(interfaceToRunAfterTimer.LastFinish))
                    {
                        interfaceToRunAfterTimer.PropertyChanged -= InterfaceToRunAfterTimer_PropertyChanged;
                        waitForFinishTaskCompletionSource.SetResult(true);
                    }
                        
                }

                // Block the flow of the function until the LastFinish property has been updated
                await waitForFinishTaskCompletionSource.Task;
            }
        }

        private async Task ExecuteInterface (TimedIntegration timedInterface)
        {
            TimedInterfaceDbContext db = null;
            TimedInterfaceLog log = null;

            if (String.IsNullOrEmpty(MIF.Config.SqlConnectionString))
                return;

            IScheduledIntegration scheduledInterface = timedInterface as IScheduledIntegration;

            try
            {
                timedInterface.Timer.LastStart = DateTime.Now;

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
                    lock (this.syncToken)
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

                DateTime lastRun = DateTime.Now;

                await timedInterface.Execute();

                if (scheduledInterface != null)
                    scheduledInterface.LastRunDateTime = lastRun;
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

                timedInterface.Timer.LastFinish = DateTime.Now;
            }
        }

        internal void StopInterfaces()
        {
            foreach (TimedIntegration timedInterface in this.TimedInterfaces)
            {
                timedInterface.Timer.Stop();
                timedInterface.Timer.Dispose();
            }

            this.TimedInterfaces.Clear();
        }
    }
}
