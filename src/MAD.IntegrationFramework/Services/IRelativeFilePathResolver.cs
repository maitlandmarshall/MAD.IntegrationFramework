using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Services
{
    internal interface IRelativeFilePathResolver
    {
        string ResolvePath(string relativeFilePath);
    }
}
