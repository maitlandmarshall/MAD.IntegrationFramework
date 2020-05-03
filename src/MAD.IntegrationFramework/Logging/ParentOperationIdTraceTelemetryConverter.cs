using Microsoft.ApplicationInsights.Channel;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Logging
{
    public sealed class ParentOperationIdTraceTelemetryConverter : TraceTelemetryConverter
    {
        const string ParentOperationIdProperty = "parentOperationId";

        public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            IEnumerable<ITelemetry> telemetry = base.Convert(logEvent, formatProvider);

            if (logEvent.Properties.TryGetValue(ParentOperationIdProperty, out LogEventPropertyValue parentOperationId))
            {
                foreach (ITelemetry t in telemetry)
                {
                    t.Context.Operation.ParentId = parentOperationId.ToString().Trim('\"');
                }
            }

            return telemetry;
        }
    }
}
