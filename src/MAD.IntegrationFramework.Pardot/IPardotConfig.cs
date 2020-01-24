using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Pardot
{
    public interface IPardotConfig
    {
        string PardotEmail { get; set; }
        string PardotPassword { get; set; }
        string PardotUserKey { get; set; }
    }
}
