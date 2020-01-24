using MAD.IntegrationFramework.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Integrations
{
    public interface IScheduledInterface
    {
        DateTime NextRunDateTime { get; }

        [Savable]
        DateTime? LastRunDateTime { get; set; }
    }
}
