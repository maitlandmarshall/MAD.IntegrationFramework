using Autofac;
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
        private readonly ITimedIntegrationTypesResolver timedIntegrationTypesResolver;
        private readonly ILifetimeScope lifetimeScope;
        private readonly TimedIntegrationRunAfterAttributeHandler timedIntegrationRunAfterAttributeHandler;
        private readonly ITimedIntegrationMetaDataService timedIntegrationMetaDataService;
        private readonly TimedIntegrationExecutionHandler timedIntegrationExecutionHandler;
        private List<TimedIntegrationTimer> timedIntegrationTimers;

        public TimedIntegrationController(ILogger<TimedIntegrationController> logger,
                                          IExceptionLogger exceptionLogger,
                                          ITimedIntegrationTypesResolver timedIntegrationTypesResolver,
                                          ILifetimeScope lifetimeScope,
                                          TimedIntegrationRunAfterAttributeHandler timedIntegrationRunAfterAttributeHandler,
                                          ITimedIntegrationMetaDataService timedIntegrationMetaDataService,
                                          TimedIntegrationExecutionHandler timedIntegrationExecutionHandler
                                          )
        {
            this.logger = logger;
            this.exceptionLogger = exceptionLogger;
            this.timedIntegrationTypesResolver = timedIntegrationTypesResolver;
            this.lifetimeScope = lifetimeScope;
            this.timedIntegrationRunAfterAttributeHandler = timedIntegrationRunAfterAttributeHandler;
            this.timedIntegrationMetaDataService = timedIntegrationMetaDataService;
            this.timedIntegrationExecutionHandler = timedIntegrationExecutionHandler;

            this.timedIntegrationTimers = new List<TimedIntegrationTimer>();
        }

        public void Start ()
        {
            IEnumerable<Type> integrationTypes = this.timedIntegrationTypesResolver.ResolveTypes();

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
                using (ILifetimeScope scope = this.lifetimeScope.BeginLifetimeScope(y => y.RegisterType(serviceTimer.TimedIntegrationType).InstancePerLifetimeScope().AsSelf()))
                {
                    // Wait for another TimedIntegration to complete if this TimedIntegration has the [RunAfter] attribute.
                    await this.timedIntegrationRunAfterAttributeHandler.WaitForOthers(serviceTimer, this.timedIntegrationTimers);

                    TimedIntegration timedIntegration = scope.Resolve(serviceTimer.TimedIntegrationType) as TimedIntegration;
                    this.timedIntegrationMetaDataService.Load(timedIntegration);

                    await this.timedIntegrationExecutionHandler.Execute(timedIntegration);
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
