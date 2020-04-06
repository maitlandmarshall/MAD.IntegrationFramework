using Autofac;
using MAD.IntegrationFramework.Configuration;
using MAD.IntegrationFramework.Database;
using MAD.IntegrationFramework.Http;
using MAD.IntegrationFramework.Integrations;
using MAD.IntegrationFramework.Logging;
using MAD.IntegrationFramework.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework
{
    internal class FrameworkModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterModule<LoggingModule>();
            builder.RegisterModule<ConfigModule>();
            builder.RegisterModule<DatabaseModule>();
            builder.RegisterModule<HttpModule>();
            builder.RegisterModule<IntegrationsModule>();

            builder.RegisterType<FrameworkContainer>().InstancePerLifetimeScope().AsSelf();
        }
    }
}
