using MAD.IntegrationFramework.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Serilog;
using System.Collections.Generic;

namespace MAD.IntegrationFramework.Logging
{
    internal class LoggerFactory
    {
        private const string LogTableName = "MIF_Log";

        private readonly MIFConfig config;
        private readonly TelemetryClientFactory telemetryClientFactory;

        public LoggerFactory(MIFConfig config, TelemetryClientFactory telemetryClientFactory)
        {
            this.config = config;
            this.telemetryClientFactory = telemetryClientFactory;
        }

        public ILogger Create()
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.With<OperationIdEnricher>()
                .WriteTo.Console()
                .WriteTo.Debug();

            if (!string.IsNullOrEmpty(this.config.SqlConnectionString))
                loggerConfiguration.WriteTo.MSSqlServer(this.config.SqlConnectionString, LogTableName, autoCreateSqlTable: true);

            if (!string.IsNullOrEmpty(this.config.InstrumentationKey))
            {
                TelemetryClient telemetryClient = this.telemetryClientFactory.Create(this.config.InstrumentationKey);
                loggerConfiguration.WriteTo.ApplicationInsights(telemetryClient, new CustomTelemetryConverter());
            }

            return loggerConfiguration.CreateLogger();
        }

        
    }
}
