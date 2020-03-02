using MaitlandsInterfaceFramework.Core.Services.Internals;
using MaitlandsInterfaceFramework.Core.Configuration;
using MaitlandsInterfaceFramework.Services.Internals;
using MaitlandsInterfaceFramework.Services.Internals.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static MaitlandsInterfaceFramework.Services.Internals.LogService;

[assembly: InternalsVisibleTo("MaitlandsInterfaceFramework.Tests")]
namespace MaitlandsInterfaceFramework
{
    public class MIFStartupProperties { }

    public static class MIF
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

        internal static TimedInterfaceService TimedInterfaceService { get; private set; }
        internal static HttpService HttpServer { get; private set; }
        internal static CancellationTokenSource ServiceCancellationToken { get; private set; }
        
        public static MIFConfig Config
        {
            get => MIFConfig.Instance;
        }

        public static void SetConfigForTesting(MIFConfig config)
        {
            MIFConfig.Instance = config;
        }

        public static void Start(MIFStartupProperties properties = null)
        {
            ServiceCancellationToken = new CancellationTokenSource();

            WriteToLog($"MIF initializing {DateTime.Now}");

            try
            {
                ConfigurationService.LoadConfiguration();

                WriteToLog($"Binding Port: {Config.BindingPort}");
                WriteToLog($"Binding Path: {Config.BindingPath}");

                if (!HaveVisibleConsole())
                {
                    WriteToLog("Running as service");

                    Host.CreateDefaultBuilder()
                        .UseWindowsService()
                        .ConfigureServices((hostContext, services) =>
                        {
                            services.AddHostedService<MIFService>();
                        })
                        .Build()
                        .RunAsync(ServiceCancellationToken.Token);
                }
                else
                {
                    WriteToLog("Running as console");
                }

                WriteToLog("Starting Http Server");

                HttpServer = new HttpService();
                HttpServer.StartAsync().Wait();

                WriteToLog("Http Server started");
                WriteToLog("Starting Timed Interface Service");

                TimedInterfaceService = new TimedInterfaceService();

                WriteToLog("Timed Tnterface Service started");

                Task.Delay(TimeSpan.FromMilliseconds(-1), ServiceCancellationToken.Token).Wait();
            }
            catch (Exception ex)
            {
                WriteToLog(ex.ToString());

                throw;
            }
        }

        public async static Task Stop()
        {
            try
            {
                await HttpServer.StopAsync();
                TimedInterfaceService.StopInterfaces();
            }
            finally
            {
                ServiceCancellationToken.Cancel();
                ServiceCancellationToken.Dispose();
            }
        }
    }

    internal sealed class MIFService : IHostedService
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
