using Autofac;
using MAD.IntegrationFramework.Configuration;
using MAD.IntegrationFramework.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal class TimedIntegrationScopeFactory : IIntegrationScopeFactory
    {
        private readonly IIntegrationScopeMIFDbContextResolver integrationMIFDbContextResolver;
        private readonly IMIFConfigResolver mifConfigResolver;
        private readonly IMIFDbContextFactory mifDbContextFactory;
        private readonly IMIFConfigFactory mifConfigFactory;

        public TimedIntegrationScopeFactory(IIntegrationScopeMIFDbContextResolver integrationScopeMIFDbContextResolver,
                                            IMIFConfigResolver mifConfigResolver,
                                            IMIFDbContextFactory mifDbContextFactory,
                                            IMIFConfigFactory mifConfigFactory)
        {
            this.integrationMIFDbContextResolver = integrationScopeMIFDbContextResolver;
            this.mifConfigResolver = mifConfigResolver;
            this.mifDbContextFactory = mifDbContextFactory;
            this.mifConfigFactory = mifConfigFactory;
        }

        public ILifetimeScope Create(Type timedIntegrationType, ILifetimeScope context)
        {
            ILifetimeScope scope = context.BeginLifetimeScope(containerBuilder => this.RegisterDependencies(timedIntegrationType, containerBuilder));

            return scope;
        }

        private void RegisterDependencies(Type timedIntegrationType, ContainerBuilder builder)
        {
            // Register the TimedIntegration component for the scope
            builder.RegisterType(timedIntegrationType).InstancePerLifetimeScope().AsSelf();

            // Register the derived MIFDbContexts which will be used by the TimedIntegration types
            foreach (Type mifDbContextType in this.integrationMIFDbContextResolver.ResolveTypes())
            {
                builder.Register(scope => this.mifDbContextFactory.Create(mifDbContextType)).InstancePerDependency().As(mifDbContextType);
            }

            // Register the derived MIFConfig component which will be used by the TimedIntegration types
            Type mifConfigType = this.mifConfigResolver.ResolveType();
            builder.Register(scope => this.mifConfigFactory.Create()).As(mifConfigType);
        }

        
    }
}
