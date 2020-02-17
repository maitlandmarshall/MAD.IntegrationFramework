using MAD.IntegrationFramework.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal class EntryAssemblyIntegrationScopeMIFDbContextResolver : IIntegrationScopeMIFDbContextResolver
    {
        public IEnumerable<Type> ResolveTypes()
        {
            return this.ResolveTypes(Assembly.GetEntryAssembly());
        }

        public IEnumerable<Type> ResolveTypes(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(y => typeof(MIFDbContext).IsAssignableFrom(y) && !y.IsAbstract);
        }
    }
}
