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
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.Conditional(y => !string.IsNullOrEmpty(config.SqlConnectionString), y => y.MSSqlServer(config.SqlConnectionString, LogTableName, autoCreateSqlTable: true))
                .CreateLogger();
        }
    }
}
