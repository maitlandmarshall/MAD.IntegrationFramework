using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal class MetaDataTimedIntegrationFactory : ITimedIntegrationFactory
    {
        private readonly ITimedIntegrationMetaDataService timedIntegrationMetaDataService;

        public MetaDataTimedIntegrationFactory(ITimedIntegrationMetaDataService timedIntegrationConfigurationService)
        {
            this.timedIntegrationMetaDataService = timedIntegrationConfigurationService;
        }

        public TimedIntegration Create(Type timedIntegrationType)
        {
            if (!typeof(TimedIntegration).IsAssignableFrom(timedIntegrationType))
                throw new TypeLoadException($"Parameter named {nameof(timedIntegrationType)} must be a type which inherents from {nameof(TimedIntegration)}.");

            // Create the instance
            TimedIntegration timedIntegration = Activator.CreateInstance(timedIntegrationType) as TimedIntegration;

            // Load the metadata
            this.timedIntegrationMetaDataService.Load(timedIntegration);

            return timedIntegration;
        }
    }
}
