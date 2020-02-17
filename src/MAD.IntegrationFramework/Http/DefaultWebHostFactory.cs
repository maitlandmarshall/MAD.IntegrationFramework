using MAD.IntegrationFramework.Configuration;
using MAD.IntegrationFramework.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Http
{
    internal class DefaultWebHostFactory : IWebHostFactory
    {
        private readonly IMIFConfigFactory mifConfigFactory;

        public DefaultWebHostFactory(IMIFConfigFactory mifConfigFactory)
        {
            this.mifConfigFactory = mifConfigFactory;
        }

        public IWebHost CreateWebHost()
        {
            MIFConfig config = this.mifConfigFactory.Create();

            IWebHostBuilder builder = WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseIISIntegration();

            if (config.BindingPort == 80 || config.BindingPort == 443)
            {
                builder.UseHttpSys(options =>
                {
                    options.UrlPrefixes.Add($"http://*:{config.BindingPort}/{config.BindingPath}");
                });
            }
            else
            {
                builder.UseKestrel(options =>
                {
                    options.ListenAnyIP(config.BindingPort);
                });
            }

            return builder.Build();
        }
    }
}
