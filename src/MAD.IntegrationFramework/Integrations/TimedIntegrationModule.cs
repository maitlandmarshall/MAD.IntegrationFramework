using Autofac;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal class TimedIntegrationModule : Autofac.Module
    {
        private readonly ITimedIntegrationTypesResolver timedIntegrationTypesResolver;
        private readonly ITimedIntegrationFactory timedIntegrationFactory;

        public TimedIntegrationModule(ITimedIntegrationTypesResolver timedIntegrationTypesResolver,
                                      ITimedIntegrationFactory timedIntegrationFactory)
        {
            this.timedIntegrationTypesResolver = timedIntegrationTypesResolver;
            this.timedIntegrationFactory = timedIntegrationFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            IEnumerable<Type> timedIntegrationTypes = this.timedIntegrationTypesResolver.ResolveTypes();

            foreach (Type t in timedIntegrationTypes)
            {
                builder
                    .Register(provider => this.timedIntegrationFactory.Create(t))
                    .InstancePerLifetimeScope()
                    .AsSelf();
            }
        }
    }
}
