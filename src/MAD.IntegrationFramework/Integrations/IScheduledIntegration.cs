using System;

namespace MAD.IntegrationFramework.Integrations
{
    public interface IScheduledIntegration
    {
        DateTime NextRunDateTime { get; }

        [Savable]
        DateTime? LastRunDateTime { get; set; }
    }
}
