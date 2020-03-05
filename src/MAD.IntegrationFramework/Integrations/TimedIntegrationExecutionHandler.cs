using MAD.IntegrationFramework.Database;
using MAD.IntegrationFramework.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Integrations
{
    internal class TimedIntegrationExecutionHandler
    {
        private readonly IMIFDbContextFactory<TimedIntegrationLogDbContext> timedIntegrationLogDbContextFactory;
        private readonly IIntegrationMetaDataService timedIntegrationMetaDataService;

        public TimedIntegrationExecutionHandler(IMIFDbContextFactory<TimedIntegrationLogDbContext> timedIntegrationLogDbContextFactory,
                                                IIntegrationMetaDataService timedIntegrationMetaDataService)
        {
            this.timedIntegrationLogDbContextFactory = timedIntegrationLogDbContextFactory;
            this.timedIntegrationMetaDataService = timedIntegrationMetaDataService;
        }

        public async Task Execute(TimedIntegration timedIntegration, TimedIntegrationTimer timer)
        {
            IScheduledIntegration scheduledInterface = timedIntegration as IScheduledIntegration;

            try
            {
                timer.LastStart = DateTime.Now;

                // Every time this interface is executed, check if it's enabled.
                if (!timedIntegration.IsEnabled)
                    return;

                if (scheduledInterface != null && scheduledInterface.LastRunDateTime.HasValue)
                {
                    DateTime now = DateTime.Now;

                    if (now < scheduledInterface.NextRunDateTime)
                        return;
                }

                using (TimedIntegrationLogDbContext dbContext = this.timedIntegrationLogDbContextFactory.Create())
                {
                    DateTime lastRun = DateTime.Now;

                    TimedIntegrationLog log = new TimedIntegrationLog
                    {
                        InterfaceName = timedIntegration.GetType().Name,
                        StartDateTime = lastRun,
                        ExecutablePath = Process.GetCurrentProcess().MainModule.FileName,
                        MachineName = Environment.MachineName
                    };

                    dbContext.TimedIntegrationLogs.Add(log);
                    await dbContext.SaveChangesAsync();

                    try
                    {
                        await timedIntegration.Execute();
                    }
                    finally
                    {
                        log.EndDateTime = DateTime.Now;
                        await dbContext.SaveChangesAsync();

                        if (scheduledInterface != null)
                            scheduledInterface.LastRunDateTime = lastRun;
                    }
                }
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
