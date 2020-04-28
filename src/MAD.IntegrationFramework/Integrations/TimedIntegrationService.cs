using Autofac;
using MAD.IntegrationFramework.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Integrations
{
    internal class TimedIntegrationService
    {
        private readonly object syncToken = new object();

        private readonly ILogger logger;
        private readonly IIntegrationResolver timedIntegrationTypesResolver;
        private readonly ILifetimeScope lifetimeScope;
        private readonly TimedIntegrationRunAfterAttributeHandler timedIntegrationRunAfterAttributeHandler;
        private readonly IIntegrationMetaDataMemento timedIntegrationMetaDataService;
        private readonly TimedIntegrationExecutionHandler timedIntegrationExecutionHandler;
        private readonly IIntegrationScopeFactory integrationScopeFactory;
        private readonly IMIFConfigRepository configRepository;

        private readonly List<TimedIntegrationTimer> timedIntegrationTimers;

        public TimedIntegrationService(ILogger logger,
                                       IIntegrationResolver timedIntegrationTypesResolver,
                                       ILifetimeScope lifetimeScope,
                                       TimedIntegrationRunAfterAttributeHandler timedIntegrationRunAfterAttributeHandler,
                                       IIntegrationMetaDataMemento timedIntegrationMetaDataService,
                                       TimedIntegrationExecutionHandler timedIntegrationExecutionHandler,
                                       IIntegrationScopeFactory integrationScopeFactory,
                                       IMIFConfigRepository configRepository)
        {
            this.logger = logger;
            this.timedIntegrationTypesResolver = timedIntegrationTypesResolver;
            this.lifetimeScope = lifetimeScope;
            this.timedIntegrationRunAfterAttributeHandler = timedIntegrationRunAfterAttributeHandler;
            this.timedIntegrationMetaDataService = timedIntegrationMetaDataService;
            this.timedIntegrationExecutionHandler = timedIntegrationExecutionHandler;
            this.integrationScopeFactory = integrationScopeFactory;
            this.configRepository = configRepository;
            this.timedIntegrationTimers = new List<TimedIntegrationTimer>();
        }

        public void Start()
        {
            IEnumerable<Type> integrationTypes = this.timedIntegrationTypesResolver.ResolveTypes();

            // Create a timer for each interface
            foreach (Type timedIntegrationType in integrationTypes)
            {
                TimedIntegrationTimer timedIntegrationTimer = new TimedIntegrationTimer(timedIntegrationType);
                timedIntegrationTimer.Elapsed += this.TimedIntegrationTimer_Elapsed;

                this.timedIntegrationTimers.Add(timedIntegrationTimer);
            }

            foreach (TimedIntegrationTimer timedIntegrationTimer in this.timedIntegrationTimers)
            {
                timedIntegrationTimer.Start();
            }
        }

        private async void TimedIntegrationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TimedIntegrationTimer serviceTimer = sender as TimedIntegrationTimer;
            serviceTimer.Stop();

            try
            {
                using (ILifetimeScope scope = this.integrationScopeFactory.Create(serviceTimer.TimedIntegrationType, this.lifetimeScope))
                {
                    // Wait for another TimedIntegration to complete if this TimedIntegration has the [RunAfter] attribute.
                    await this.timedIntegrationRunAfterAttributeHandler.WaitForOthers(serviceTimer, this.timedIntegrationTimers);

                    TimedIntegration timedIntegration = scope.Resolve(serviceTimer.TimedIntegrationType) as TimedIntegration;
                    this.timedIntegrationMetaDataService.Load(timedIntegration);

                    await this.timedIntegrationExecutionHandler.Execute(timedIntegration, serviceTimer);

                    // The service timer interval starts at "run immediately" and then is calculated by the TimedIntegration implementation
                    serviceTimer.Interval = timedIntegration.Interval.TotalMilliseconds;

                    // If the consumer edits the MIFConfig automatically save it.
                    await this.configRepository.Save(scope.Resolve<MIFConfig>());
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "{Integration} has failed", serviceTimer.TimedIntegrationType.Name);

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
