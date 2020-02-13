using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal interface ITimedIntegrationFactory
    {
        TimedIntegration Create(Type timedIntegrationType);
    }
}
