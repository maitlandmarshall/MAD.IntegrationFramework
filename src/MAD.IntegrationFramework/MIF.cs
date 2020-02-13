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
        internal static IServiceProvider ServiceProvider { get; private set; }

        private static void ConfigureServices()
        {
            IServiceCollection serviceCollection = new ServiceCollection()
                .AddLogging(logging =>
                {
                    logging.Services.AddTransient(typeof(ILogger<>), typeof(Logger<>));
                })
                .AddSingleton<FrameworkContainer>()
                .AddSingleton<AutomaticMigrationService>()
                .AddSingleton<IWebHostFactory, DefaultWebHostFactory>()
                .AddTransient<ExceptionDbLogger>()
                .AddTransient<SqlStatementBuilder>()
                .AddTransient<EmbeddedResourceService>()
                .AddTransient<FileSystemTimedIntegrationMetaDataService>()
                .AddTransient<FrameworkConfigurationService>()
                .AddTransient(typeof(IMIFDbContextFactory<>), typeof(MIFDbContextFactory<>))
                .AddTransient<IMIFConfigFactory, DefaultMIFConfigFactory>()
                .AddTransient<IRelativeFilePathResolver, DefaultRelativeFilePathResolver>()
                .AddTransient<ITimedIntegrationFactory, MetaDataTimedIntegrationFactory>()

                .AddScoped<TimedIntegrationController>()
                .AddScoped<TimedIntegrationRunAfterAttributeHandler>();

            IEnumerable<Type> timedIntegrationTypes = new EntryAssemblyTimedIntegrationTypesResolver().ResolveTypes();

            foreach (Type t in timedIntegrationTypes)
            {
                serviceCollection.AddScoped(t, provider => provider.GetRequiredService<ITimedIntegrationFactory>().Create(t));
            }

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public static Task Start(MIFStartupProperties properties = null)
        {
            ConfigureServices();

            return ServiceProvider.GetRequiredService<FrameworkContainer>().Start();
        }

        public static Task Stop()
        {
            return ServiceProvider.GetRequiredService<FrameworkContainer>().Stop();
        }
    }
}
