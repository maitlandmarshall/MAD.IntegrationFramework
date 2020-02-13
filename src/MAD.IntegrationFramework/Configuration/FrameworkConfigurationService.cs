using MAD.IntegrationFramework.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MAD.IntegrationFramework.Configuration
{
    internal class FrameworkConfigurationService
    {
        private const string SettingsRelativePath = "settings.json";
        private static object syncToken = new object();

        private IRelativeFilePathResolver relativeFilePathResolver;

        public FrameworkConfigurationService(DefaultRelativeFilePathResolver relativeFilePathResolver)
        {
            this.relativeFilePathResolver = relativeFilePathResolver;
        }

        public MIFConfig LoadConfiguration()
        {
            Type typeWhichInheritsFromMIFConfig = GetMifsConfigConfigurationType();
            string settingsFilePath = this.relativeFilePathResolver.GetRelativeFilePath(SettingsRelativePath);

            if (!File.Exists(settingsFilePath))
            {
                MIFConfig.Instance = Activator.CreateInstance(typeWhichInheritsFromMIFConfig) as MIFConfig;
            }
            else
            {
                string settingsData = File.ReadAllText(settingsFilePath);
                MIFConfig.Instance = JsonConvert.DeserializeObject(settingsData, typeWhichInheritsFromMIFConfig) as MIFConfig;
            }

            return MIFConfig.Instance;
        }

        public static void SaveConfiguration(MIFConfig config)
        {
            lock (syncToken)
            {
                string settingsData = JsonConvert.SerializeObject(config, Formatting.Indented);

                File.WriteAllText(SettingsRelativePath, settingsData);

                MIFConfig.Instance = config;
            }
        }

        private static Type GetMifsConfigConfigurationType()
        {
            Type typeWhichInheritsFromMIFConfig = Assembly.GetEntryAssembly().GetTypes().FirstOrDefault(y => !y.IsAbstract && typeof(MIFConfig).IsAssignableFrom(y));

            if (typeWhichInheritsFromMIFConfig == null)
                typeWhichInheritsFromMIFConfig = typeof(MIFConfig);

            return typeWhichInheritsFromMIFConfig;
        }
    }
}
