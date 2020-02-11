using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Salesforce
{
    public interface ISalesforceConfig
    {
        string SalesforceConsumerKey { get; set; }
        string SalesforceConsumerSecret { get; set; }
        string SalesforceAccessToken { get; set; }
        string SalesforceRefreshToken { get; set; }
        string SalesforceOAuthCallbackUrl { get; set; }
    }
}
