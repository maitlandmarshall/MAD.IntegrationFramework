using MaitlandsInterfaceFramework.Core.Configuration;
using MaitlandsInterfaceFramework.Database;
using MaitlandsInterfaceFramework.Factories.Internals.Database;
using MaitlandsInterfaceFramework.Services.Internals;
using MaitlandsInterfaceFramework.Services.Internals.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("MaitlandsInterfaceFramework.Tests")]
namespace MaitlandsInterfaceFramework
{
    public class MIFStartupProperties { }

    public static class MIF
    {
        internal static IServiceProvider ServiceProvider { get; private set; }

        public static MIFConfig Config => MIFConfig.Instance;

        public static void SetConfigForTesting(MIFConfig config)
        {
            MIFConfig.Instance = config;
        }

        private static void ConfigureServices()
        {
            var serviceCollection = new ServiceCollection()
                .AddLogging(logging =>
                {
                    logging.Services.AddTransient(typeof(ILogger<>), typeof(Logger<>));
                })
                .AddSingleton<MIFService>()
                .AddSingleton<AutomaticMigrationService>()
                .AddSingleton<TimedInterfaceService>()
                .AddTransient<ExceptionDbLogService>()
                .AddTransient<SqlBuilderService>()
                .AddTransient(typeof(IMIFDbContextFactory<>), typeof(MIFDbContextFactory<>));

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public static Task Start(MIFStartupProperties properties = null)
        {
            ConfigureServices();

            return ServiceProvider.GetService<MIFService>().Start();
        }

        public static Task Stop()
        {
            return ServiceProvider.GetService<MIFService>().Stop();
        }
    }
}
