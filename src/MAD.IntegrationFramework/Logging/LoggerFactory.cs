using MAD.IntegrationFramework.Configuration;
using Serilog;

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
                loggerConfiguration.WriteTo.ApplicationInsights(config.InstrumentationKey, TelemetryConverter.Traces);

            return loggerConfiguration.CreateLogger();
        }
    }
}
