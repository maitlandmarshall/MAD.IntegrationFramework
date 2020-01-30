using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Services
{
    internal interface IRelativeFilePathResolver
    {
        string GetRelativeFilePath(string filePath);
    }
}
