using MAD.IntegrationFramework.Configuration;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Serilog;
using System.Globalization;

namespace MAD.IntegrationFramework.Logging
{
    internal class LoggerFactory
    {
        private const string LogTableName = "MIF_Log";

        private readonly MIFConfig config;

        public LoggerFactory(MIFConfig config)
        {
            this.config = config;
        }

        public ILogger Create()
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug();

            if (!string.IsNullOrEmpty(config.SqlConnectionString))
                loggerConfiguration.WriteTo.MSSqlServer(config.SqlConnectionString, LogTableName, autoCreateSqlTable: true);

            if (!string.IsNullOrEmpty(config.InstrumentationKey))
            {
                TelemetryConfiguration telemetryConfiguration = TelemetryConfiguration.CreateDefault();
                telemetryConfiguration.InstrumentationKey = config.InstrumentationKey;

                TelemetryProcessorChainBuilder builder = telemetryConfiguration.TelemetryProcessorChainBuilder;

                QuickPulseTelemetryProcessor quickPulseProcessor = null;
                builder.Use((next) =>
                {
                    quickPulseProcessor = new QuickPulseTelemetryProcessor(next);
                    return quickPulseProcessor;
                });
                builder.Build();

                QuickPulseTelemetryModule quickPulse = new QuickPulseTelemetryModule();
                quickPulse.Initialize(telemetryConfiguration);
                quickPulse.RegisterTelemetryProcessor(quickPulseProcessor);

                DependencyTrackingTelemetryModule depModule = new DependencyTrackingTelemetryModule();
                depModule.Initialize(telemetryConfiguration);

                loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, TelemetryConverter.Traces);
            }

            return loggerConfiguration.CreateLogger();
        }
    }
}
