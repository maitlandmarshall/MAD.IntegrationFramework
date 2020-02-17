using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Http
{
    internal class HttpModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<DefaultWebHostFactory>().As<IWebHostFactory>();
        }
    }
}
