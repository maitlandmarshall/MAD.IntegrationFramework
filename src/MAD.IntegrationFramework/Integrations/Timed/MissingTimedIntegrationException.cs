using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    public class MissingTimedIntegrationException: Exception
    {
        public MissingTimedIntegrationException(Type timedIntegration) : base($"{timedIntegration.Name} is missing.") { }
    }
}
