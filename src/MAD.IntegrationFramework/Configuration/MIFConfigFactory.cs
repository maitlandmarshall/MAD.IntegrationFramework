using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MAD.IntegrationFramework.Configuration
{
    internal class MIFConfigFactory
    {
        internal string SettingsDirectory => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        private string SettingsFilePath => Path.Combine(this.SettingsDirectory, "settings.json");

        public MIFConfig Load()
        {
            Type typeWhichInheritsFromMIFConfig = this.GetMifsConfigConfigurationType();

            if (!File.Exists(this.SettingsFilePath))
            {
                return Activator.CreateInstance(typeWhichInheritsFromMIFConfig) as MIFConfig;
            }
            else
            {
                string settingsData = File.ReadAllText(SettingsFilePath);
                return JsonConvert.DeserializeObject(settingsData, typeWhichInheritsFromMIFConfig) as MIFConfig;
            }
        }

        private Type GetMifsConfigConfigurationType()
        {
            Type typeWhichInheritsFromMIFConfig = Assembly.GetEntryAssembly().GetTypes().FirstOrDefault(y => typeof(MIFConfig).IsAssignableFrom(y));

            if (typeWhichInheritsFromMIFConfig == null)
                typeWhichInheritsFromMIFConfig = typeof(MIFConfig);

            return typeWhichInheritsFromMIFConfig;
        }
    }
}
