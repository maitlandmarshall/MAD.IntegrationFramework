using MAD.IntegrationFramework.Integrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RunAfterAttribute : Attribute
    {
        public Type IntegrationTypeToRunAfter { get; }

        public RunAfterAttribute(Type integrationTypeToRunAfter)
        {
            if (!typeof(IIntegration).IsAssignableFrom(integrationTypeToRunAfter))
                throw new NotSupportedException($"{integrationTypeToRunAfter.Name} must implement {nameof(IIntegration)}.");

            this.IntegrationTypeToRunAfter = integrationTypeToRunAfter;
        }
    }
}
