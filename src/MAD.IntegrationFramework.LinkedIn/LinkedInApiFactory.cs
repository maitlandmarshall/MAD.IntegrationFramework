using Sparkle.LinkedInNET;

namespace MAD.IntegrationFramework.LinkedIn
{
    public static class LinkedInApiFactory
    {
        public static LinkedInApi GetApi()
        {
            ILinkedInConfig linkedInConfig = MIFConfig.Instance as ILinkedInConfig;

            if (linkedInConfig == null)
                return null;

            return new LinkedInApi(new LinkedInApiConfiguration { ApiKey = linkedInConfig.LinkedInAppClientId, ApiSecretKey = linkedInConfig.LinkedInAppClientPassword });
        }

        public static UserAuthorization GetUserAuthorization()
        {
            ILinkedInConfig linkedInConfig = MIFConfig.Instance as ILinkedInConfig;

            if (linkedInConfig == null)
                return null;

            return new UserAuthorization(linkedInConfig.ClientLinkedInApiToken);
        }
    }
}
