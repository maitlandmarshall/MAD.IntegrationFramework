using Microsoft.CodeAnalysis.CSharp.Syntax;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Logging
{
    public static class ILoggerExtensions
    {
        public const string EventPropertyName = "IsEvent";

        public static void Event (this ILogger logger, string messageTemplate)
        {
            logger = logger.ForContext(EventPropertyName, true);

            logger.Information(messageTemplate);
        }
    }
}
