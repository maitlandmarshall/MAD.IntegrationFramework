using Autofac;
using Serilog;

namespace MAD.IntegrationFramework.Logging
{
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<LoggerFactory>().AsSelf();
            builder.Register(y => y.Resolve<LoggerFactory>().Create()).AsImplementedInterfaces();
        }
    }
}
