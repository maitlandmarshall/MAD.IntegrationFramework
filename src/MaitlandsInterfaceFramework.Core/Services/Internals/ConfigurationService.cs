using MaitlandsInterfaceFramework.Core;
using MaitlandsInterfaceFramework.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MaitlandsInterfaceFramework.Core.Services.Internals
{
    internal class ConfigurationService
    {
        #region PROPERTIES

        internal static string SettingsDirectory
        {
            get
            {
                return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }
        }

        private static string SettingsFilePath
        {
            get
            {
                return Path.Combine(SettingsDirectory, "settings.json");
            }
        }

        private static volatile object SyncToken = new object();

        #endregion

        #region LOAD CONFIGURATION

        public static void LoadConfiguration()
        {
            Type typeWhichInheritsFromMIFConfig = GetMifsConfigConfigurationType();

            if (!File.Exists(SettingsFilePath))
            {
                MIFConfig.Instance = Activator.CreateInstance(typeWhichInheritsFromMIFConfig) as MIFConfig;
            }
            else
            {
                string settingsData = File.ReadAllText(SettingsFilePath);
                MIFConfig.Instance = JsonConvert.DeserializeObject(settingsData, typeWhichInheritsFromMIFConfig) as MIFConfig;
            }
        }

        public static void LoadConfiguration (TimedInterface timedInterface)
        {
            IEnumerable<MemberInfo> savableMembers = GetSavableMembersFromTimedInterface(timedInterface);

            if (!savableMembers.Any())
                return;

            string timedInterfaceSettingsFilePath = GetTimedInterfaceSettingsFilePath(timedInterface);

            if (!File.Exists(timedInterfaceSettingsFilePath))
                return;

            string timedInterfaceSettingData = File.ReadAllText(timedInterfaceSettingsFilePath);

            if (String.IsNullOrEmpty(timedInterfaceSettingData))
                return;

            Dictionary<string, object> propertiesToLoad = JsonConvert.DeserializeObject<Dictionary<string, object>>(timedInterfaceSettingData);

            foreach (MemberInfo member in savableMembers)
            {
                string memberKey = member.Name;
                object memberValue = propertiesToLoad[memberKey];

                switch (member)
                {
                    case PropertyInfo pi:
                        pi.SetValue(timedInterface, memberValue);
                        break;
                    case FieldInfo fi:
                        fi.SetValue(timedInterface, memberValue);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        #endregion

        #region SAVE CONFIGURATION

        public static void SaveConfiguration(MIFConfig config)
        {
            lock (SyncToken)
            {
                string settingsData = JsonConvert.SerializeObject(config, Formatting.Indented);

                File.WriteAllText(SettingsFilePath, settingsData);

                MIFConfig.Instance = config;
            }
        }

        public static void SaveConfiguration(TimedInterface timedInterface)
        {
            lock (SyncToken)
            {
                Type timedInterfaceType = timedInterface.GetType();

                IEnumerable<MemberInfo> savableMembers = GetSavableMembersFromTimedInterface(timedInterface);

                if (!savableMembers.Any())
                    return;

                Dictionary<string, object> propertiesToSave = new Dictionary<string, object>();

                foreach (MemberInfo member in savableMembers)
                {
                    string memberKey = member.Name;
                    object memberValue;            
                    
                    switch (member)
                    {
                        case PropertyInfo pi:
                            memberValue = pi.GetValue(timedInterface, null);
                            break;
                        case FieldInfo fi:
                            memberValue = fi.GetValue(timedInterface);
                            break;
                        default:
                            throw new NotSupportedException();
                    }

                    propertiesToSave[memberKey] = memberValue;
                }

                string timedInterfaceSettingsData = JsonConvert.SerializeObject(propertiesToSave);

                File.WriteAllText(GetTimedInterfaceSettingsFilePath(timedInterface), timedInterfaceSettingsData);
            }
        }

        #endregion

        #region HELPERS

        private static IEnumerable<MemberInfo> GetSavableMembersFromTimedInterface(TimedInterface timedInterface)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            IEnumerable<MemberInfo> savableMembers = timedInterface
                .GetType()
                .GetMembers(bindingFlags)
                .Where(y => y.GetCustomAttribute<SavableAttribute>(true) != null);

            IEnumerable<MemberInfo> savableInterfaces = timedInterface
                .GetType()
                .GetInterfaces()
                .SelectMany(y => y.GetMembers(bindingFlags).Where(z => z.GetCustomAttribute<SavableAttribute>(true) != null));

            foreach (MemberInfo savableAttribute in savableMembers)
                yield return savableAttribute;

            foreach (MemberInfo savableAttribute in savableInterfaces)
                yield return savableAttribute;
        }

        private static string GetTimedInterfaceSettingsFilePath(TimedInterface timedInterface)
        {
            return Path.Combine(SettingsDirectory, $"{timedInterface.GetType().Name}.json");
        }

        private static Type GetMifsConfigConfigurationType()
        {
            Type typeWhichInheritsFromMIFConfig = Assembly.GetEntryAssembly().GetTypes().FirstOrDefault(y => typeof(MIFConfig).IsAssignableFrom(y));

            if (typeWhichInheritsFromMIFConfig == null)
                typeWhichInheritsFromMIFConfig = typeof(MIFConfig);

            return typeWhichInheritsFromMIFConfig;
        }

        #endregion
    }
}
