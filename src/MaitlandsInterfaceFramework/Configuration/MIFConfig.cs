using MaitlandsInterfaceFramework.Services.Internals;

namespace MaitlandsInterfaceFramework.Configuration
{
    public class MIFConfig
    {
        internal const int DefaultBindingPort = 666;

        public string SqlConnectionString { get; set; }

        public int BindingPort { get; set; } = DefaultBindingPort;
        public string BindingPath { get; set; }

        public void Save()
        {
            ConfigurationService.SaveConfiguration(this);
        }
    }
}
