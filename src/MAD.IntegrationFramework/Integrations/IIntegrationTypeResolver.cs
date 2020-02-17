using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal interface IIntegrationResolver
    {
        IEnumerable<Type> ResolveTypes();
    }
}
