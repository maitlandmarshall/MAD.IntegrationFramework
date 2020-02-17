using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal interface IIntegrationScopeMIFDbContextResolver
    {
        IEnumerable<Type> ResolveTypes();
    }
}
