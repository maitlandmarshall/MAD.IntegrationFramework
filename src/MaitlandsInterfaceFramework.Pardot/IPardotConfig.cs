using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot
{
    public interface IPardotConfig
    {
        string PardotEmail { get; set; }
        string PardotPassword { get; set; }
        string PardotUserKey { get; set; }
    }
}
