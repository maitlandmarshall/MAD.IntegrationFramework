using MAD.IntegrationFramework.Services;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MAD.IntegrationFramework.Configuration
{
    internal class DefaultMIFConfigFactory : IMIFConfigFactory
    {
        private const string SettingsFileName = "settings.json";

        private readonly IRelativeFilePathResolver relativeFilePathResolver;

        public DefaultMIFConfigFactory(IRelativeFilePathResolver relativeFilePathResolver)
        {
            this.relativeFilePathResolver = relativeFilePathResolver;
        }

        public MIFConfig Create()
        {
            Type typeWhichInheritsFromMIFConfig = this.GetMifsConfigConfigurationType();
            string settingsFilePath = this.relativeFilePathResolver.ResolvePath(SettingsFileName);

            if (!File.Exists(settingsFilePath))
            {
                return Activator.CreateInstance(typeWhichInheritsFromMIFConfig) as MIFConfig;
            }
            else
            {
                string settingsData = File.ReadAllText(settingsFilePath);
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
