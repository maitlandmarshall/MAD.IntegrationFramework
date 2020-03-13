using MAD.IntegrationFramework.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MAD.IntegrationFramework.Http
{
    internal class DefaultWebHostFactory : IWebHostFactory
    {
        private readonly MIFConfig config;

        public DefaultWebHostFactory(MIFConfig config)
        {
            this.config = config;
        }

        public IWebHost CreateWebHost()
        {
            IWebHostBuilder builder = WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseIISIntegration();

            if (this.config.BindingPort == 80 || this.config.BindingPort == 443)
            {
                builder.UseHttpSys(options =>
                {
                    options.UrlPrefixes.Add($"http://*:{this.config.BindingPort}/{this.config.BindingPath}");
                });
            }
            else
            {
                builder.UseKestrel(options =>
                {
                    options.ListenAnyIP(this.config.BindingPort);
                });
            }

            return builder.Build();
        }
    }
}
