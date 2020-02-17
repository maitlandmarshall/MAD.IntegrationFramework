using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Configuration
{
    internal interface IMIFConfigRepository
    {
        Task Save(MIFConfig config);
    }
}
