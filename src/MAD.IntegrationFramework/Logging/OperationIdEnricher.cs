using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MAD.IntegrationFramework.Logging
{
    public class OperationIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            Activity currentActivity = Activity.Current;

            if (currentActivity is null)
                return;

            logEvent.AddPropertyIfAbsent(new LogEventProperty("operationId", new ScalarValue(currentActivity.TraceId)));
        }
    }
}
