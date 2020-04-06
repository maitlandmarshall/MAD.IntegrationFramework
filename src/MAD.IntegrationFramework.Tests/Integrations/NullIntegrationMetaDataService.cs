using MAD.IntegrationFramework.Integrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Tests.Integrations
{
    internal class NullIntegrationMetaDataService : IIntegrationMetaDataMemento
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
