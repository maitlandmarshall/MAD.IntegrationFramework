using MAD.IntegrationFramework.Core;
using MAD.IntegrationFramework.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ITimedIntegrationFactory timedIntegrationFactory;
        private readonly ITimedIntegrationTypesResolver timedIntegrationTypesResolver;
        private List<TimedIntegrationTimer> timedIntegrationTimers;

        public TimedIntegrationController(ILogger<TimedIntegrationController> logger,
                                          IExceptionLogger exceptionLogger,
                                          IServiceScopeFactory serviceScopeFactory,
                                          ITimedIntegrationFactory timedIntegrationFactory,
                                          ITimedIntegrationTypesResolver timedIntegrationTypesResolver
                                          )
        {
            this.logger = logger;
            this.exceptionLogger = exceptionLogger;
            this.serviceScopeFactory = serviceScopeFactory;
            this.timedIntegrationFactory = timedIntegrationFactory;
            this.timedIntegrationTypesResolver = timedIntegrationTypesResolver;
            this.timedIntegrationTimers = new List<TimedIntegrationTimer>();
        }

        public void Start (IEnumerable<Type> integrationTypes)
        {
            // Create a timer for each interface
            foreach (Type timedIntegrationType in integrationTypes)
            {
                TimedIntegrationTimer timedIntegrationTimer = new TimedIntegrationTimer(timedIntegrationType);
                timedIntegrationTimer.Elapsed += this.TimedIntegrationTimer_Elapsed;

                this.timedIntegrationTimers.Add(timedIntegrationTimer);
            }

            foreach (TimedIntegrationTimer timedInterfaceTimer in this.timedIntegrationTimers)
            {
                timedInterfaceTimer.Start();
            }
        }

        private async void TimedIntegrationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimedIntegrationTimer serviceTimer = sender as TimedIntegrationTimer;
            serviceTimer.Stop();

            try
            {
                using (IServiceScope scope = this.serviceScopeFactory.CreateScope())
                {
                    // Wait for another TimedIntegration to complete if this TimedIntegration has the [RunAfter] attribute.
                    await scope.ServiceProvider
                        .GetRequiredService<TimedIntegrationRunAfterAttributeHandler>()
                        .WaitForOthers(serviceTimer, this.timedIntegrationTimers);

                    TimedIntegration timedIntegration = scope.ServiceProvider.GetRequiredService(serviceTimer.TimedIntegrationType) as TimedIntegration;
                    TimedIntegrationExecutionHandler timedIntegrationExecuteHandler = scope.ServiceProvider.GetRequiredService<TimedIntegrationExecutionHandler>();

                    await timedIntegrationExecuteHandler.Execute(timedIntegration);
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                await this.exceptionLogger.LogException(ex, serviceTimer.TimedIntegrationType.GetType().Name);

                // Wait for 6 hours before starting the timer again to prevent error spam
                // TODO: Think of a better way to do this
                await Task.Delay(TimeSpan.FromHours(6));
            }
            finally
            {
                serviceTimer.Start();
            }
        }

        internal void Stop()
        {
            foreach (TimedIntegrationTimer timedIntegrationTimers in this.timedIntegrationTimers)
            {
                timedIntegrationTimers.Stop();
                timedIntegrationTimers.Dispose();
            }

            this.timedIntegrationTimers.Clear();
        }
    }
}
