using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace MAD.IntegrationFramework.Services
{
    internal class DefaultRelativeFilePathResolver : IRelativeFilePathResolver
    {
        private string RootDirectory => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public string ResolvePath (string filePath)
        {
            return Path.Combine(this.RootDirectory, filePath);
        }
    }
}
