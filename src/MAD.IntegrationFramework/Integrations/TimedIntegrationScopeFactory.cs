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
        private readonly IMIFDbContextFactory mifDbContextFactory;
        private readonly MIFConfig config;

        public TimedIntegrationScopeFactory(IIntegrationScopeMIFDbContextResolver integrationScopeMIFDbContextResolver,
                                            IMIFDbContextFactory mifDbContextFactory,
                                            MIFConfig config)
        {
            this.integrationMIFDbContextResolver = integrationScopeMIFDbContextResolver;
            this.mifDbContextFactory = mifDbContextFactory;
            this.config = config;
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
                builder.Register<object>(scope => this.mifDbContextFactory.Create(mifDbContextType)).As(mifDbContextType);
            }

            if (this.config.GetType() == typeof(MIFConfig))
                return;

            builder.Register<object>(scope => this.config).SingleInstance().As(config.GetType());
        }

        
    }
}
