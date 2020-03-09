using MAD.IntegrationFramework.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MAD.IntegrationFramework.Integrations
{
    internal sealed class FileSystemIntegrationMetaDataMemento : IIntegrationMetaDataMemento
    {
        private static readonly object syncToken = new object();

        private readonly IRelativeFilePathResolver relativeFilePathResolver;

        public FileSystemIntegrationMetaDataMemento(IRelativeFilePathResolver integrationPathResolver)
        {
            this.relativeFilePathResolver = integrationPathResolver;
        }

        // TODO: Should this be moved to its own service?
        internal string GetMetaDataFilePath(TimedIntegration timedIntegration)
        {
            return this.relativeFilePathResolver.ResolvePath($"{timedIntegration.GetType().Name}.json");
        }

        public void Load(TimedIntegration timedIntegration)
        {
            IEnumerable<MemberInfo> savableMembers = this.GetSavableMembers(timedIntegration);

            if (!savableMembers.Any())
                return;

            string timedIntegrationSettingsFilePath = this.GetMetaDataFilePath(timedIntegration);

            if (!File.Exists(timedIntegrationSettingsFilePath))
                return;

            string timedIntegrationSettingData = File.ReadAllText(timedIntegrationSettingsFilePath);

            if (string.IsNullOrEmpty(timedIntegrationSettingData))
                return;

            Dictionary<string, object> propertiesToLoad = JsonConvert.DeserializeObject<Dictionary<string, object>>(timedIntegrationSettingData);

            foreach (MemberInfo member in savableMembers)
            {
                string memberKey = member.Name;
                object memberValue = propertiesToLoad[memberKey];

                switch (member)
                {
                    case PropertyInfo pi:
                        pi.SetValue(timedIntegration, memberValue);
                        break;
                    case FieldInfo fi:
                        fi.SetValue(timedIntegration, memberValue);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public void Save(TimedIntegration timedIntegration)
        {
            lock (syncToken)
            {
                Type timedIntegrationType = timedIntegration.GetType();

                IEnumerable<MemberInfo> savableMembers = this.GetSavableMembers(timedIntegration);

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
                            memberValue = pi.GetValue(timedIntegration, null);
                            break;
                        case FieldInfo fi:
                            memberValue = fi.GetValue(timedIntegration);
                            break;
                        default:
                            throw new NotSupportedException();
                    }

                    propertiesToSave[memberKey] = memberValue;
                }

                string timedIntegrationSettingsData = JsonConvert.SerializeObject(propertiesToSave);

                File.WriteAllText(this.GetMetaDataFilePath(timedIntegration), timedIntegrationSettingsData);
            }
        }

        private IEnumerable<MemberInfo> GetSavableMembers(TimedIntegration timedIntegration)
        {
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            IEnumerable<MemberInfo> savableMembers = timedIntegration
                .GetType()
                .GetMembers(bindingFlags)
                .Where(y => y.GetCustomAttribute<SavableAttribute>(true) != null);

            IEnumerable<MemberInfo> savableInterfaces = timedIntegration
                .GetType()
                .GetInterfaces()
                .SelectMany(y => y.GetMembers(bindingFlags).Where(z => z.GetCustomAttribute<SavableAttribute>(true) != null));

            foreach (MemberInfo savableAttribute in savableMembers)
                yield return savableAttribute;

            foreach (MemberInfo savableAttribute in savableInterfaces)
                yield return savableAttribute;
        }
    }
}
