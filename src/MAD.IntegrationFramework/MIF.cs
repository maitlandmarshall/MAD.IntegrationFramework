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
                .AddSingleton<AutomaticMigrationProvider>()
                .AddSingleton<TimedInterfaceController>()
                .AddSingleton<IWebHostFactory, DefaultWebHostFactory>()
                .AddTransient<ExceptionDbLogger>()
                .AddTransient<SqlBuilder>()
                .AddTransient(typeof(IMIFDbContextFactory<>), typeof(MIFDbContextFactory<>));

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
