using MaitlandsInterfaceFramework.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Core
{
    public interface IScheduledInterface
    {
        DateTime NextRunDateTime { get; }

        [Savable]
        DateTime? LastRunDateTime { get; set; }
    }
}
