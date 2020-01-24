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
        public IWebHost CreateWebHost()
        {
            IWebHostBuilder builder = WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseIISIntegration();

            if (MIF.Config.BindingPort == 80 || MIF.Config.BindingPort == 443)
            {
                builder.UseHttpSys(config =>
                {
                    config.UrlPrefixes.Add($"http://*:{MIF.Config.BindingPort}/{MIF.Config.BindingPath}");
                });
            }
            else
            {
                builder.UseKestrel(config =>
                {
                    config.ListenAnyIP(MIF.Config.BindingPort);
                });
            }

            return builder.Build();
        }
    }
}
