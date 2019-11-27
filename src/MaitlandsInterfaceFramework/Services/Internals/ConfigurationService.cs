using MaitlandsInterfaceFramework.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MaitlandsInterfaceFramework.Services.Internals
{
    internal class ConfigurationService
    {
        private static string SettingsFilePath
        {
            get
            {
                return Path.Combine(
                        Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                        "settings.json");
            }
        }

        public static Type GetConfigurationType()
        {
            Type typeWhichInheritsFromMIFConfig = Assembly.GetEntryAssembly().GetTypes().FirstOrDefault(y => typeof(MIFConfig).IsAssignableFrom(y));

            if (typeWhichInheritsFromMIFConfig == null)
                typeWhichInheritsFromMIFConfig = typeof(MIFConfig);

            return typeWhichInheritsFromMIFConfig;
        }

        public static MIFConfig LoadConfiguration()
        {
            Type typeWhichInheritsFromMIFConfig = GetConfigurationType();

            if (!File.Exists(SettingsFilePath))
                return Activator.CreateInstance(typeWhichInheritsFromMIFConfig) as MIFConfig;

            string settingsData = File.ReadAllText(SettingsFilePath);

            return JsonConvert.DeserializeObject(settingsData, typeWhichInheritsFromMIFConfig) as MIFConfig;
        }

        private static volatile object SyncToken = new object();

        public static void SaveConfiguration(MIFConfig config)
        {
            lock (SyncToken)
            {
                string settingsData = JsonConvert.SerializeObject(config, Formatting.Indented);

                File.WriteAllText(SettingsFilePath, settingsData);

                if (MIF.Config != config)
                    MIF.Config = config;
            }
        }
    }
}
