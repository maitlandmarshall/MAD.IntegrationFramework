using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal interface IIntegrationPathResolver
    {
        string ResolvePath(TimedIntegration timedIntegration);
    }
}
 