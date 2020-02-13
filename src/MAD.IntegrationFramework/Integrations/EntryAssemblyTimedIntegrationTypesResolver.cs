using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal class EntryAssemblyTimedIntegrationTypesResolver : ITimedIntegrationTypesResolver
    {
        public IEnumerable<Type> ResolveTypes()
        {
            Assembly assembly = Assembly.GetEntryAssembly();

            // Look through the assembly and yield all the classes which inherit from TimedIntegration
            foreach (Type timedInterfaceType in assembly.GetTypes().Where(y => typeof(TimedIntegration).IsAssignableFrom(y) && !y.IsAbstract))
            {
                yield return timedInterfaceType;
            }
        }
    }
}
