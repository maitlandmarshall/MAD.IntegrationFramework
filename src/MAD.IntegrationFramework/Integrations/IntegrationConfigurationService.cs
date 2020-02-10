using MAD.IntegrationFramework.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    internal sealed class IntegrationConfigurationService
    {
        private static object syncToken = new object();

        private IIntegrationPathResolver integrationPathResolver;

        public IntegrationConfigurationService(IIntegrationPathResolver integrationPathResolver)
        {
            this.integrationPathResolver = integrationPathResolver;
        }

        public void LoadConfiguration(TimedIntegration timedIntegration)
        {
            IEnumerable<MemberInfo> savableMembers = GetSavableMembers(timedIntegration);

            if (!savableMembers.Any())
                return;

            string timedInterfaceSettingsFilePath = this.integrationPathResolver.ResolvePath(timedIntegration);

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

        public void SaveConfiguration(TimedIntegration timedInterface)
        {
            lock (syncToken)
            {
                Type timedInterfaceType = timedInterface.GetType();

                IEnumerable<MemberInfo> savableMembers = this.GetSavableMembers(timedInterface);

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

                File.WriteAllText(this.integrationPathResolver.ResolvePath(timedInterface), timedInterfaceSettingsData);
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
