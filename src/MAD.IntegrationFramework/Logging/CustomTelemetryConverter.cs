using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAD.IntegrationFramework.Logging
{
    public sealed class CustomTelemetryConverter : TraceTelemetryConverter
    {
        public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            // If the event property is set to true, convert all the telemetry to Event telemetry
            if (logEvent.Properties.TryGetValue(ILoggerExtensions.EventPropertyName, out LogEventPropertyValue isEvent)
                && isEvent.ToString() == true.ToString())
            {
                logEvent.RemovePropertyIfPresent(ILoggerExtensions.EventPropertyName);

                EventTelemetry telemetry = new EventTelemetry(logEvent.MessageTemplate.Text)
                {
                    Timestamp = logEvent.Timestamp
                };

                this.ForwardPropertiesToTelemetryProperties(logEvent, telemetry, formatProvider);

                yield return telemetry;
            }
            else 
            {
                IEnumerable<ITelemetry> baseResult = base.Convert(logEvent, formatProvider);

                foreach (ITelemetry t in baseResult)
                {
                    yield return t;
                }
            }
        }
    }
}
