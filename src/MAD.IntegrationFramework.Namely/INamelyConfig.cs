using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Namely
{
    public interface INamelyConfig
    {
        string NamelyClientName { get; set; }
        string NamelyToken { get; set; }

    }
}
