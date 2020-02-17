using Autofac;
using Autofac.Extensions.DependencyInjection;
using MAD.IntegrationFramework.Configuration;
using MAD.IntegrationFramework.Database;
using MAD.IntegrationFramework.Http;
using MAD.IntegrationFramework.Integrations;
using MAD.IntegrationFramework.Logging;
using MAD.IntegrationFramework.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("MAD.IntegrationFramework.Tests")]
namespace MAD.IntegrationFramework
{
    public class MIFStartupProperties { }

    public static class MIF
    {
        internal static IContainer RootDependencyInjectionContainer { get; private set; }
        private static ILifetimeScope rootScope;

        private static void ConfigureServices()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder
                .RegisterType(typeof(Logger<>))
                .As(typeof(ILogger<>));

            builder
                .RegisterType<FrameworkContainer>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<AutomaticMigrationService>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<DefaultWebHostFactory>()
                .As<IWebHostFactory>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ExceptionDbLogger>()
                .AsSelf();

            builder
                .RegisterType<SqlStatementBuilder>()
                .AsSelf();

            builder
                .RegisterType<EmbeddedResourceService>()
                .AsSelf();

            builder
                .RegisterType<FileSystemTimedIntegrationMetaDataService>()
                .AsSelf();

            builder
                .RegisterType<FrameworkConfigurationService>()
                .AsSelf();

            builder
                .RegisterType(typeof(MIFDbContextFactory<>))
                .As(typeof(IMIFDbContextFactory<>))
                .AsSelf();

            builder
                .RegisterType<DefaultMIFConfigFactory>()
                .As<IMIFConfigFactory>();

            builder
                .RegisterType<DefaultRelativeFilePathResolver>()
                .As<IRelativeFilePathResolver>();

            builder
                .RegisterType<MetaDataTimedIntegrationFactory>()
                .As<ITimedIntegrationFactory>();

            builder.RegisterType<TimedIntegrationController>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<TimedIntegrationRunAfterAttributeHandler>()
                .AsSelf()
                .InstancePerLifetimeScope();

            RootDependencyInjectionContainer = builder.Build();
        }

        public static async Task Start(MIFStartupProperties properties = null)
        {
            ConfigureServices();

            using (ILifetimeScope rootScope = RootDependencyInjectionContainer.BeginLifetimeScope())
            {
                await rootScope.Resolve<FrameworkContainer>().Start();
            }
        }

        public static Task Stop()
        {
            return rootScope.Resolve<FrameworkContainer>().Stop();
        }
    }
}
