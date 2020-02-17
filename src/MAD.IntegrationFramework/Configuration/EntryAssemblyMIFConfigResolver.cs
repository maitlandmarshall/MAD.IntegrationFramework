using MAD.IntegrationFramework.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MAD.IntegrationFramework.Configuration
{
    internal class EntryAssemblyMIFConfigResolver : IMIFConfigResolver
    {
        public Type ResolveType()
        {
            return Assembly
                .GetEntryAssembly()
                .GetTypes()
                .SingleOrDefault(y => typeof(MIFConfig).IsAssignableFrom(y) && !y.IsAbstract);
        }
    }
}
