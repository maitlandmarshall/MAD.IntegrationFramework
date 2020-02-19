using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework
{
    public abstract class MIFStartup
    {
        public abstract void ConfigureServices(ContainerBuilder containerBuilder);
    }
}
