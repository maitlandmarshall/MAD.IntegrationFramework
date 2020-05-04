using MAD.IntegrationFramework.Database;
using MAD.IntegrationFramework.Logging;
using Microsoft.ApplicationInsights;
using Serilog;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Integrations
{
    internal class TimedIntegrationExecutionHandler
    {
        private readonly ILogger logger;
        private readonly IIntegrationMetaDataMemento timedIntegrationMetaDataService;

        public TimedIntegrationExecutionHandler(ILogger logger,
                                                IIntegrationMetaDataMemento timedIntegrationMetaDataService)
        {
            this.logger = logger;
            this.timedIntegrationMetaDataService = timedIntegrationMetaDataService;
        }

        public async Task Execute(TimedIntegration timedIntegration, TimedIntegrationTimer timer)
        {
            IScheduledIntegration scheduledInterface = timedIntegration as IScheduledIntegration;

            try
            {
                DateTime lastRun = DateTime.Now;

                timer.LastStart = lastRun;

                // Every time this interface is executed, check if it's enabled.
                if (!timedIntegration.IsEnabled)
                    return;

                if (scheduledInterface != null
                    && scheduledInterface.LastRunDateTime.HasValue
                    && lastRun < scheduledInterface.NextRunDateTime)
                {
                    return;
                }

                this.logger.Event("{Integration} has started");

                await timedIntegration.Execute();

                if (scheduledInterface != null)
                    scheduledInterface.LastRunDateTime = lastRun;

                this.logger.Event("{Integration} has finished");
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "{Integration} has failed");
                throw;
            }
            finally
            {
                // Save the configuration data after each execution of the TimedIntegration
                this.timedIntegrationMetaDataService.Save(timedIntegration);

                timer.LastFinish = DateTime.Now;
            }
        }
    }
}
