using Autofac;
using MAD.IntegrationFramework.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IIntegrationScopeFactory integrationScopeFactory;
        private readonly IMIFConfigRepository configRepository;

        private readonly List<TimedIntegrationTimer> timedIntegrationTimers;

        public TimedIntegrationService(ILogger logger,
                                       IIntegrationResolver timedIntegrationTypesResolver,
                                       ILifetimeScope lifetimeScope,
                                       TimedIntegrationRunAfterAttributeHandler timedIntegrationRunAfterAttributeHandler,
                                       IIntegrationMetaDataMemento timedIntegrationMetaDataService,
                                       IIntegrationScopeFactory integrationScopeFactory,
                                       IMIFConfigRepository configRepository)
        {
            this.logger = logger;
            this.timedIntegrationTypesResolver = timedIntegrationTypesResolver;
            this.lifetimeScope = lifetimeScope;
            this.timedIntegrationRunAfterAttributeHandler = timedIntegrationRunAfterAttributeHandler;
            this.timedIntegrationMetaDataService = timedIntegrationMetaDataService;
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
            TimedIntegrationTimer integrationTimer = sender as TimedIntegrationTimer;
            integrationTimer.Stop();

            Activity integrationActivity = new Activity(integrationTimer.TimedIntegrationType.Name);
            integrationActivity.Start();

            try
            {
                using (ILifetimeScope scope = this.integrationScopeFactory.Create(integrationTimer.TimedIntegrationType, this.lifetimeScope))
                {
                    // Wait for another TimedIntegration to complete if this TimedIntegration has the [RunAfter] attribute.
                    await this.timedIntegrationRunAfterAttributeHandler.WaitForOthers(integrationTimer, this.timedIntegrationTimers);

                    TimedIntegration timedIntegration = scope.Resolve(integrationTimer.TimedIntegrationType) as TimedIntegration;
                    this.timedIntegrationMetaDataService.Load(timedIntegration);

                    await scope.Resolve<TimedIntegrationExecutionHandler>().Execute(timedIntegration, integrationTimer);

                    // The integration timer interval starts at "run immediately" and then is calculated by the TimedIntegration implementation
                    integrationTimer.Interval = timedIntegration.Interval.TotalMilliseconds;

                    // If the consumer edits the MIFConfig automatically save it.
                    await this.configRepository.Save(scope.Resolve<MIFConfig>());
                }
            }
            catch (Exception)
            {
                // Wait for 6 hours before starting the timer again to prevent error spam
                // TODO: Think of a better way to do this
                await Task.Delay(TimeSpan.FromHours(6));
            }
            finally
            {
                integrationActivity.Stop();
                integrationTimer.Start();
            }
        }

        internal void Stop()
        {
            foreach (TimedIntegrationTimer integrationTimer in this.timedIntegrationTimers)
            {
                if (integrationTimer.IsIntegrationRunning())
                    this.logger.Information("{Integration} has stopped", integrationTimer.TimedIntegrationType.Name);

                integrationTimer.Stop();
                integrationTimer.Dispose();
            }

            this.timedIntegrationTimers.Clear();
        }
    }
}
