using MAD.IntegrationFramework.Configuration;
using MAD.IntegrationFramework.Http;
using MAD.IntegrationFramework.Integrations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework
{
    internal class FrameworkContainer
    {
        #region HAVE VISIBLE CONSOLE

        // P/Invoke declarations for Windows.
        [DllImport("kernel32.dll")] private static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")] private static extern bool IsWindowVisible(IntPtr hWnd);

        // Indicates if the current process is running:
        //  * on Windows: in a console window visible to the user.
        //  * on Unix-like platform: in a terminal window, which is assumed to imply
        //    user-visibility (given that hidden processes don't need terminals).
        private static bool HaveVisibleConsole()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                          IsWindowVisible(GetConsoleWindow())
                          :
                          Console.WindowHeight > 0;
        }

        #endregion

        private readonly CancellationTokenSource serviceCancellationToken;

        private readonly ILogger logger;
        private readonly IWebHostFactory webHostFactory;
        private readonly MIFConfig config;
        private readonly TimedIntegrationService TimedIntegrationService;

        private IWebHost webHost;

        public FrameworkContainer(ILogger logger,
                                  IWebHostFactory webHostFactory,
                                  MIFConfig config,
                                  TimedIntegrationService timedIntegrationService)
        {
            this.logger = logger;
            this.webHostFactory = webHostFactory;
            this.config = config;
            this.TimedIntegrationService = timedIntegrationService;
            this.serviceCancellationToken = new CancellationTokenSource();
        }

        public async Task Start()
        {
            this.logger.Information("MIF initializing");

            try
            {
                this.logger.Information("Binding Port: {bindingPort}", this.config.BindingPort);
                this.logger.Information("Binding Path: {bindingPath}", this.config.BindingPath);

                if (!HaveVisibleConsole())
                {
                    this.MountWindowsService();
                }
                else
                {
                    this.logger.Information("Running as console");
                }

                this.logger.Information("Starting Http Server");

                this.webHost = this.webHostFactory.CreateWebHost();
                await this.webHost.StartAsync(this.serviceCancellationToken.Token);

                this.logger.Information("Http Server started");
                this.logger.Information("Starting Timed Integration Service");

                this.TimedIntegrationService.Start();

                this.logger.Information("Timed Integration Service started");

                await Task.Delay(TimeSpan.FromMilliseconds(-1), this.serviceCancellationToken.Token);
            }
            catch (Exception ex)
            {
                this.logger.Fatal(ex, "MIF failed to start");

                Console.ReadLine();

                throw;
            }
        }

        private void MountWindowsService()
        {
            this.logger.Information("Running as service");

            _ = Host.CreateDefaultBuilder()
                    .UseWindowsService()
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<MIFWindowsService>();
                    })
                    .Build()
                    .RunAsync(this.serviceCancellationToken.Token);
        }

        public async Task Stop()
        {
            try
            {
                await this.webHost.StopAsync(TimeSpan.FromSeconds(60));
            }
            finally
            {
                this.serviceCancellationToken.Cancel();
                this.serviceCancellationToken.Dispose();
            }
        }
    }

    internal sealed class MIFWindowsService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await MIF.Stop();
        }
    }
}
