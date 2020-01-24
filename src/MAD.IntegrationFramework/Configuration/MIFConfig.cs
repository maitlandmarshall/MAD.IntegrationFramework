namespace MAD.IntegrationFramework.Configuration
{
    public class MIFConfig
    {
        public static MIFConfig Instance { get; internal set; }

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
