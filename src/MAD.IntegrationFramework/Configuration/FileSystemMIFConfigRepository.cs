using MAD.IntegrationFramework.Services;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Configuration
{
    internal class FileSystemMIFConfigRepository : IMIFConfigRepository
    {
        private const string SettingsRelativePath = "settings.json";

        private readonly IRelativeFilePathResolver relativeFilePathResolver;

        public FileSystemMIFConfigRepository(IRelativeFilePathResolver relativeFilePathResolver)
        {
            this.relativeFilePathResolver = relativeFilePathResolver;
        }

        public async Task Save(MIFConfig config)
        {
            string settingsData = JsonConvert.SerializeObject(config, Formatting.Indented);

            await File.WriteAllTextAsync(
                path: this.relativeFilePathResolver.ResolvePath(SettingsRelativePath),
                contents: settingsData
            );
        }
    }
}
