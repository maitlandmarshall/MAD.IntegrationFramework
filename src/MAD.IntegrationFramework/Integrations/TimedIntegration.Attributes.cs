using MAD.IntegrationFramework.Integrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RunAfterAttribute : Attribute
    {
        public Type IntegrationTypeToRunAfter { get; }

        public RunAfterAttribute(Type interfaceTypeToRunAfter)
        {
            if (!typeof(TimedIntegration).IsAssignableFrom(interfaceTypeToRunAfter))
                throw new NotSupportedException($"{interfaceTypeToRunAfter.Name} must derive from {nameof(TimedIntegration)}.");

            this.IntegrationTypeToRunAfter = interfaceTypeToRunAfter;
        }
    }
}
