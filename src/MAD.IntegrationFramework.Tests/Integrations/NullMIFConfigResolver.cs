using MAD.IntegrationFramework.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.UnitTests.Integrations
{
    internal class NullMIFConfigResolver : IMIFConfigResolver
    {
        public Type ResolveType()
        {
            return null;
        }
    }
}
