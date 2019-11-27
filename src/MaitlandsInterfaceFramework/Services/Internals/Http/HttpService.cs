using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework.Services.Internals.Http
{
    internal class HttpService
    {
        private IWebHost Server;

        public HttpService()
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

            this.Server = builder.Build();
        }

        public Task StartAsync()
        {
            return this.Server.StartAsync();
        }

        public Task StopAsync()
        {
            return this.Server.StopAsync();
        }
    }
}
