using MAD.IntegrationFramework.Configuration;
using MAD.IntegrationFramework.Http;
using MAD.IntegrationFramework.Integrations;
using MAD.IntegrationFramework.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework
{
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

    internal class FrameworkContainer
    {
        #region HAVE VISIBLE CONSOLE

        // P/Invoke declarations for Windows.
        [DllImport("kernel32.dll")] static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")] static extern bool IsWindowVisible(IntPtr hWnd);

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
        private readonly IExceptionLogger exceptionLogger;
        private readonly IWebHostFactory webHostFactory;
        private readonly IMIFConfigFactory mifConfigFactory;
        private readonly TimedIntegrationController timedIntegrationController;
        private IWebHost webHost;

        public FrameworkContainer(ILogger<FrameworkContainer> logger,
                                  IExceptionLogger exceptionLogger,
                                  IWebHostFactory webHostFactory,
                                  IMIFConfigFactory mifConfigFactory,
                                  TimedIntegrationController timedIntegrationController)
        {
            this.logger = logger;
            this.exceptionLogger = exceptionLogger;
            this.webHostFactory = webHostFactory;
            this.mifConfigFactory = mifConfigFactory;
            this.timedIntegrationController = timedIntegrationController;
            this.serviceCancellationToken = new CancellationTokenSource();
        }

        public async Task Start()
        {
            this.logger.LogInformation($"MIF initializing {DateTime.Now}");

            try
            {
                MIFConfig config = this.mifConfigFactory.Create();

                this.logger.LogInformation($"Binding Port: {config.BindingPort}");
                this.logger.LogInformation($"Binding Path: {config.BindingPath}");

                if (!HaveVisibleConsole())
                {
                    this.MountWindowsService();
                }
                else
                {
                    this.logger.LogInformation("Running as console");
                }

                this.logger.LogInformation("Starting Http Server");

                this.webHost = this.webHostFactory.CreateWebHost();
                await this.webHost.StartAsync(this.serviceCancellationToken.Token);

                this.logger.LogInformation("Http Server started");
                this.logger.LogInformation("Starting Timed Integration Service");

                timedIntegrationController.Start();

                this.logger.LogInformation("Timed Integration Service started");

                await Task.Delay(TimeSpan.FromMilliseconds(-1), this.serviceCancellationToken.Token);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                await this.exceptionLogger.LogException(ex);

                throw;
            }
        }

        private void MountWindowsService()
        {
            this.logger.LogInformation("Running as service");

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
}
