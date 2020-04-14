using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations.Attributed
{
    internal class AttributedIntegrationService : IIntegrationService
    {
        private readonly ILogger<AttributedIntegrationService> logger;

        public AttributedIntegrationService (ILogger<AttributedIntegrationService> logger)
        {
            this.logger = logger;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
