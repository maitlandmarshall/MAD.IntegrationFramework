using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal interface ITimedIntegrationTypesResolver
    {
        IEnumerable<Type> ResolveTypes();
    }
}
