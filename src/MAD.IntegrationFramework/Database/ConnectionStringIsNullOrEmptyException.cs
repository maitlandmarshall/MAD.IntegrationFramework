using MAD.IntegrationFramework.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Database
{
    public class ConnectionStringIsNullOrEmptyException : Exception
    {
        public ConnectionStringIsNullOrEmptyException() : base($"{nameof(MIFConfig)}.{nameof(MIFConfig.SqlConnectionString)} is null or empty.") { }
    }
}
