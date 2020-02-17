using MAD.IntegrationFramework.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Integrations
{
    internal class TimedIntegrationExecutionHandler
    {
        private readonly IDbContextLogger<TimedIntegrationLogDbContext, TimedIntegrationLog> timedIntegrationLogger;
        private readonly IIntegrationMetaDataService timedIntegrationMetaDataService;

        public TimedIntegrationExecutionHandler(IDbContextLogger<TimedIntegrationLogDbContext, TimedIntegrationLog> timedIntegrationLogger,
                                                IIntegrationMetaDataService timedIntegrationMetaDataService)
        {
            this.timedIntegrationLogger = timedIntegrationLogger;
            this.timedIntegrationMetaDataService = timedIntegrationMetaDataService;
        }

        public async Task Execute(TimedIntegration timedIntegration)
        {
            IScheduledIntegration scheduledInterface = timedIntegration as IScheduledIntegration;

            try
            {
                timedIntegration.Timer.LastStart = DateTime.Now;

                // Every time this interface is executed, check if it's enabled.
                if (!timedIntegration.IsEnabled)
                    return;

                if (scheduledInterface != null && scheduledInterface.LastRunDateTime.HasValue)
                {
                    DateTime now = DateTime.Now;

                    if (now < scheduledInterface.NextRunDateTime)
                        return;
                }

                using IDisposable _ = await this.timedIntegrationLogger.Step(
                    log: new TimedIntegrationLog
                    {
                        InterfaceName = timedIntegration.GetType().Name,
                        StartDateTime = DateTime.Now,
                        ExecutablePath = Process.GetCurrentProcess().MainModule.FileName,
                        MachineName = Environment.MachineName
                    },
                    finish: (log) => log.EndDateTime = DateTime.Now
                );

                DateTime lastRun = DateTime.Now;

                await timedIntegration.Execute();

                if (scheduledInterface != null)
                    scheduledInterface.LastRunDateTime = lastRun;
            }
            finally
            {
                // Save the configuration data after each execution of the TimedInterface
                this.timedIntegrationMetaDataService.Save(timedIntegration);

                timedIntegration.Timer.LastFinish = DateTime.Now;
            }
        }
    }
}
