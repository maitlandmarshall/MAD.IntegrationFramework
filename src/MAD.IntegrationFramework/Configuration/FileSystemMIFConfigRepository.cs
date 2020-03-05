using MAD.IntegrationFramework.Services;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Configuration
{
    internal class FileSystemMIFConfigRepository : IMIFConfigRepository
    {
        private const string SettingsRelativePath = "settings.json";

        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private readonly IRelativeFilePathResolver relativeFilePathResolver;

        public FileSystemMIFConfigRepository(IRelativeFilePathResolver relativeFilePathResolver)
        {
            this.relativeFilePathResolver = relativeFilePathResolver;
        }

        public async Task Save(MIFConfig config)
        {
            await semaphoreSlim.WaitAsync();

            try
            {
                string settingsData = JsonConvert.SerializeObject(config, Formatting.Indented);

                await File.WriteAllTextAsync(
                    path: this.relativeFilePathResolver.ResolvePath(SettingsRelativePath),
                    contents: settingsData
                );
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
