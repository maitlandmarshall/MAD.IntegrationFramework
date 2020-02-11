using MaitlandsInterfaceFramework.Core.Configuration;
using Salesforce.Common;
using Salesforce.Force;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework.Salesforce
{
    public class SalesforceApiFactory
    {
        public async Task<ForceClient> GetApiClient()
        {
            ISalesforceConfig config = MIFConfig.Instance as ISalesforceConfig;

            if (config is null)
                throw new NotImplementedException($"{nameof(MIFConfig)} must implement {nameof(ISalesforceConfig)}.");

            using AuthenticationClient auth = new AuthenticationClient();
            auth.InstanceUrl = "https://ap16.salesforce.com";

            return new ForceClient(auth.InstanceUrl, config.SalesforceAccessToken, auth.ApiVersion);
        }
    }
}
