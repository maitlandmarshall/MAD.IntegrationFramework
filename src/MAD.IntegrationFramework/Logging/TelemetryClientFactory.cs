using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Logging
{
    internal class TelemetryClientFactory
    {
        public TelemetryClient Create()
        {
            return this.Create(null);
        }

        public TelemetryClient Create(string instrumentationKey)
        {
            TelemetryConfiguration telemetryConfiguration = TelemetryConfiguration.CreateDefault();
            telemetryConfiguration.InstrumentationKey = instrumentationKey;

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

            return new TelemetryClient(telemetryConfiguration);
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
