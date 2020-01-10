using MaitlandsInterfaceFramework.Core.Configuration;
using MaitlandsInterfaceFramework.Core.Services.Internals;
using MaitlandsInterfaceFramework.Services.Internals.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework.Services.Internals
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

    internal class MIFService
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

        internal TimedInterfaceService TimedInterfaceService { get; private set; }
        internal HttpService HttpServer { get; private set; }
        internal CancellationTokenSource ServiceCancellationToken { get; private set; }

        private ILogger Logger;
        private ExceptionDbLogService ExceptionDbLogger;

        public MIFService(ILogger<MIFService> logger, ExceptionDbLogService exceptionDbLogger, TimedInterfaceService timedInterfaceService)
        {
            this.Logger = logger;
            this.ExceptionDbLogger = exceptionDbLogger;
            this.TimedInterfaceService = timedInterfaceService;
        }

        public async Task Start()
        {
            this.Logger.LogInformation($"MIF initializing {DateTime.Now}");

            try
            {
                MIFConfig config = ConfigurationService.LoadConfiguration();

                this.Logger.LogInformation($"Binding Port: {config.BindingPort}");
                this.Logger.LogInformation($"Binding Path: {config.BindingPath}");

                this.ServiceCancellationToken = new CancellationTokenSource();

                if (!HaveVisibleConsole())
                {
                    this.Logger.LogInformation("Running as service");

                    _ = Host.CreateDefaultBuilder()
                            .UseWindowsService()
                            .ConfigureServices((hostContext, services) =>
                            {
                                services.AddHostedService<MIFWindowsService>();
                            })
                            .Build()
                            .RunAsync(this.ServiceCancellationToken.Token);
                }
                else
                {
                    this.Logger.LogInformation("Running as console");
                }

                this.Logger.LogInformation("Starting Http Server");

                this.HttpServer = new HttpService();
                await this.HttpServer.StartAsync();

                this.Logger.LogInformation("Http Server started");
                this.Logger.LogInformation("Starting Timed Interface Service");

                this.TimedInterfaceService.LoadInterfaces();
                this.TimedInterfaceService.StartInterfaces();

                this.Logger.LogInformation("Timed Tnterface Service started");

                await Task.Delay(TimeSpan.FromMilliseconds(-1), this.ServiceCancellationToken.Token);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, ex.Message);
                await this.ExceptionDbLogger.LogException(ex);

                throw;
            }
        }

        public async Task Stop()
        {
            try
            {
                await this.HttpServer.StopAsync();
                this.TimedInterfaceService.StopInterfaces();
            }
            finally
            {
                this.ServiceCancellationToken.Cancel();
                this.ServiceCancellationToken.Dispose();
            }
        }
    }
}
