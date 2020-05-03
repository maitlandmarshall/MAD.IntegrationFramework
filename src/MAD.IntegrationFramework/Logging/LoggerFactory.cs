using MAD.IntegrationFramework.Configuration;
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

        public LoggerFactory(MIFConfig config)
        {
            this.config = config;
        }

        public ILogger Create()
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.With<OperationIdEnricher>()
                .WriteTo.Console()
                .WriteTo.Debug();

            if (!string.IsNullOrEmpty(this.config.SqlConnectionString))
                loggerConfiguration.WriteTo.MSSqlServer(this.config.SqlConnectionString, LogTableName, autoCreateSqlTable: true);

            if (!string.IsNullOrEmpty(this.config.InstrumentationKey))
            {
                TelemetryConfiguration telemetryConfiguration = TelemetryConfiguration.CreateDefault();
                telemetryConfiguration.InstrumentationKey = this.config.InstrumentationKey;

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

                DependencyTrackingTelemetryModule depModule = this.BuildDependencyTrackingTelemetryModule();
                depModule.Initialize(telemetryConfiguration);

                PerformanceCollectorModule perfCounter = new PerformanceCollectorModule();
                perfCounter.Initialize(telemetryConfiguration);

                loggerConfiguration.WriteTo.ApplicationInsights(telemetryConfiguration, new ParentOperationIdTraceTelemetryConverter());
            }

            return loggerConfiguration.CreateLogger();
        }

        private DependencyTrackingTelemetryModule BuildDependencyTrackingTelemetryModule()
        {
            List<string> excludeComponentCorrelationHttpHeadersOnDomains = new List<string>
            {
                "core.windows.net",
                "core.chinacloudapi.cn",
                "core.cloudapi.de",
                "core.usgovcloudapi.net",
                "localhost",
                "127.0.0.1"
            };

            DependencyTrackingTelemetryModule depModule = new DependencyTrackingTelemetryModule();

            foreach (string excludeComponent in excludeComponentCorrelationHttpHeadersOnDomains)
                depModule.ExcludeComponentCorrelationHttpHeadersOnDomains.Add(excludeComponent);

            depModule.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.ServiceBus");
            depModule.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.EventHubs");

            return depModule;
        }
    }
}
