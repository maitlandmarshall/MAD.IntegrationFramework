using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Logging
{
    internal interface IExceptionLogger
    {
        Task LogException(Exception exception, string interfaceName = "");
    }
}
