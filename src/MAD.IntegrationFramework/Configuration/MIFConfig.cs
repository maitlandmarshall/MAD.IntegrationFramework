using System.ComponentModel.DataAnnotations;

namespace MAD.IntegrationFramework.Configuration
{
    public class MIFConfig
    {
        internal const int DefaultBindingPort = 666;

        public string SqlConnectionString { get; set; }

        public int BindingPort { get; set; } = DefaultBindingPort;
        public string BindingPath { get; set; }

        public string InstrumentationKey { get; set; }
    }
}
