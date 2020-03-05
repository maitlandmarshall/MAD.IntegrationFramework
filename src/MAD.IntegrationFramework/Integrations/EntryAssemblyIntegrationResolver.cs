using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal class EntryAssemblyIntegrationResolver : IIntegrationResolver
    {
        public IEnumerable<Type> ResolveTypes()
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            return this.ResolveTypes(assembly);
        }

        public IEnumerable<Type> ResolveTypes (Assembly assembly)
        {
            // Look through the assembly and yield all the classes which inherit from TimedIntegration
            foreach (Type timedIntegrationType in assembly.GetTypes().Where(y => typeof(TimedIntegration).IsAssignableFrom(y) && !y.IsAbstract))
            {
                yield return timedIntegrationType;
            }
        }
    }
}
