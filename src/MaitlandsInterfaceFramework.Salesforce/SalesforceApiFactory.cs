using MaitlandsInterfaceFramework.Core.Configuration;
using Salesforce.Common;
using Salesforce.Force;
using System;
using System.Net.Http;
using System.Reflection;
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

            using AuthenticationClient auth = new AuthenticationClient
            {
                InstanceUrl = "https://ap16.salesforce.com"
            };
            await auth.TokenRefreshAsync(config.SalesforceConsumerKey, config.SalesforceRefreshToken, config.SalesforceConsumerSecret);

            ForceClient client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);
            FieldInfo jsonHttpClientField = client.GetType().GetField("_jsonHttpClient", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            JsonHttpClient jsonHttpClient = jsonHttpClientField.GetValue(client) as JsonHttpClient;

            FieldInfo httpClientField = jsonHttpClient.GetType().GetField("HttpClient", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            HttpClient httpClient = httpClientField.GetValue(jsonHttpClient) as HttpClient;

            HttpClientHandler handler = typeof(HttpMessageInvoker).GetField("_handler", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(httpClient) as HttpClientHandler;
            handler.AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.None;

            return client;
        }
    }
}
