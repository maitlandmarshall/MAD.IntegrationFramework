using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.LinkedIn
{
    public interface ILinkedInConfig
    {
        string ClientLinkedInApiAuthCode { get; set; }
        string ClientLinkedInApiToken { get; set; }
        string ClientLinkedInApiState { get; set; }

        string LinkedInAppClientId { get; set; }
        string LinkedInAppClientPassword { get; set; }
    }
}
