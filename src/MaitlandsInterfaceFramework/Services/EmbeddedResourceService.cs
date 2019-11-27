using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MaitlandsInterfaceFramework.Services
{
    public static class EmbeddedResourceService
    {
        public static string ResourceAsString(string resourceName)
        {
            Assembly callingAssembly = Assembly.GetCallingAssembly();

            string fullResourcePath = callingAssembly
                .GetManifestResourceNames()
                .FirstOrDefault(y => y.EndsWith(resourceName));


            if (String.IsNullOrEmpty(fullResourcePath))
                throw new Exception($"{resourceName} does not exist.");


            using (Stream stream = callingAssembly.GetManifestResourceStream(fullResourcePath))
            using (StreamReader sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
