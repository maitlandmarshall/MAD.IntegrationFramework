using MAD.IntegrationFramework.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal class DefaultIntegrationFilePathResolver : IIntegrationPathResolver
    {
        private IRelativeFilePathResolver relativeFilePathResolver;

        public DefaultIntegrationFilePathResolver(IRelativeFilePathResolver relativeFilePathResolver)
        {
            this.relativeFilePathResolver = relativeFilePathResolver;
        }

        public string ResolvePath(TimedIntegration timedIntegration)
        {
            return this.relativeFilePathResolver.GetRelativeFilePath($"{timedIntegration.GetType().Name}.json");
        }
    }
}
