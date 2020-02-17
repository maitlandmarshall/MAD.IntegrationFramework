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

[assembly: InternalsVisibleTo("MAD.IntegrationFramework.IntegrationTests")]
[assembly: InternalsVisibleTo("MAD.IntegrationFramework.UnitTests")]
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
            builder.RegisterModule<FrameworkModule>();

            RootDependencyInjectionContainer = builder.Build();
        }

        public static async Task Start(MIFStartupProperties properties = null)
        {
            ConfigureServices();

            using (rootScope = RootDependencyInjectionContainer.BeginLifetimeScope())
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
