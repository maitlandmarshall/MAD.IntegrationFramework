using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MAD.IntegrationFramework.Logging
{
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            ServiceCollection services = new ServiceCollection();
            services.AddLogging(cfg => 
                cfg
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddConsole()
            );

            builder.Populate(services);

            builder.RegisterType<ExceptionDbLogger>().As<IExceptionLogger>();
        }
    }
}
