using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal interface ITimedIntegrationMetaDataService
    {
        void Save(TimedIntegration timedInterface);
        void Load(TimedIntegration timedIntegration);
    }
}
