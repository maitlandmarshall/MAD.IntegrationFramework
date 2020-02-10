using MAD.IntegrationFramework.Configuration;
using MAD.IntegrationFramework.Database;
using MAD.IntegrationFramework.Http;
using MAD.IntegrationFramework.Integrations;
using MAD.IntegrationFramework.Logging;
using MAD.IntegrationFramework.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
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
                .AddSingleton<TimedIntegrationController>()
                .AddSingleton<IWebHostFactory, DefaultWebHostFactory>()
                .AddTransient<ExceptionDbLogger>()
                .AddTransient<SqlStatementBuilder>()
                .AddTransient<EmbeddedResourceService>()
                .AddTransient<IntegrationConfigurationService>()
                .AddTransient<FrameworkConfigurationService>()
                .AddTransient(typeof(IMIFDbContextFactory<>), typeof(MIFDbContextFactory<>))

                .AddTransient<IRelativeFilePathResolver, DefaultRelativeFilePathResolver>()
                .AddTransient<IIntegrationPathResolver, DefaultIntegrationFilePathResolver>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public static Task Start(MIFStartupProperties properties = null)
        {
            ConfigureServices();

            return ServiceProvider.GetService<FrameworkContainer>().Start();
        }

        public static Task Stop()
        {
            return ServiceProvider.GetService<FrameworkContainer>().Stop();
        }
    }
}
