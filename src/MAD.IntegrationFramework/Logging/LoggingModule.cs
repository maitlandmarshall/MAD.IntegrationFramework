using Autofac;
using MAD.IntegrationFramework.Database;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Logging
{
    public class LoggingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>));

            builder.RegisterType<LoggerFactory>().As<ILoggerFactory>();
            builder.RegisterType<ExceptionDbLogger>().As<IExceptionLogger>();            
        }
    }
}
