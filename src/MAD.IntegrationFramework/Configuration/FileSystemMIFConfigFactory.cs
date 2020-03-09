using MAD.IntegrationFramework.Services;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MAD.IntegrationFramework.Configuration
{
    internal class FileSystemMIFConfigFactory : IMIFConfigFactory
    {
        private const string SettingsFileName = "settings.json";

        private readonly IRelativeFilePathResolver relativeFilePathResolver;
        private readonly IMIFConfigResolver mifConfigTypeResolver;

        public FileSystemMIFConfigFactory(IRelativeFilePathResolver relativeFilePathResolver,
                                          IMIFConfigResolver mifConfigTypeResolver)
        {
            this.relativeFilePathResolver = relativeFilePathResolver;
            this.mifConfigTypeResolver = mifConfigTypeResolver;
        }

        public MIFConfig Create()
        {
            Type typeWhichInheritsFromMIFConfig = this.mifConfigTypeResolver.ResolveType();

            // If there isn't a type from the type resolver, instantiate a default MIFConfig
            if (typeWhichInheritsFromMIFConfig == null)
                typeWhichInheritsFromMIFConfig = typeof(MIFConfig);

            string settingsFilePath = this.relativeFilePathResolver.ResolvePath(SettingsFileName);

            if (!File.Exists(settingsFilePath))
            {
                return this.CreateConfig(typeWhichInheritsFromMIFConfig);
            }
            else
            {
                string settingsData = File.ReadAllText(settingsFilePath);
                return this.CreateConfig(typeWhichInheritsFromMIFConfig, settingsData);
            }
        }

        private MIFConfig CreateConfig(Type typeWhichInheritsFromMIFConfig, string settingsData = null)
        {
            if (string.IsNullOrEmpty(settingsData))
            {
                return Activator.CreateInstance(typeWhichInheritsFromMIFConfig) as MIFConfig;
            }

            return JsonConvert.DeserializeObject(settingsData, typeWhichInheritsFromMIFConfig) as MIFConfig;
        }
    }
}
