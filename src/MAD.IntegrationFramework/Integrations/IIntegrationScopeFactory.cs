using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal interface IIntegrationScopeFactory
    {
        ILifetimeScope Create(Type timedIntegrationType, ILifetimeScope context);
    }
}
