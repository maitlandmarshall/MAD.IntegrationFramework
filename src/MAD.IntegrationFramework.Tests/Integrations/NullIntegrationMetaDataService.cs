using MAD.IntegrationFramework.Integrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.UnitTests.Integrations
{
    internal class NullIntegrationMetaDataService : IIntegrationMetaDataService
    {
        public void Load(TimedIntegration timedIntegration)
        {
            return;
        }

        public void Save(TimedIntegration timedInterface)
        {
            return;
        }
    }
}
