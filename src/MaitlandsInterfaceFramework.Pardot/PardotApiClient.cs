using MaitlandsInterfaceFramework.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("MaitlandsInterfaceFramework.Pardot.Tests")]
namespace MaitlandsInterfaceFramework.Pardot
{
    public class PardotApiClient
    {
        private const string PardotApiBaseUri = "https://pi.pardot.com/api/";

        private string ApiKey;

        public PardotApiClient()
        {
            IPardotConfig config = MIFConfig.Instance as IPardotConfig;

            if (config == null)
                throw new Exception($"{nameof(MIFConfig)} must implement {nameof(IPardotConfig)}.");
        }

        internal async Task<ResponseType> ExecuteWebRequest<ResponseType>(string relativeUri)
        {
            IPardotConfig config = MIFConfig.Instance as IPardotConfig;

            Uri requestUri = new Uri(new Uri(PardotApiBaseUri), relativeUri);
            HttpWebRequest request = HttpWebRequest.CreateHttp(requestUri.ToString());
            
            if (!String.IsNullOrEmpty(this.ApiKey))
                request.Headers.Add(HttpRequestHeader.Authorization, $"Pardot api_key={this.ApiKey}, user_key={config.PardotUserKey}");

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            using Stream responseStream = response.GetResponseStream();
            using StreamReader sr = new StreamReader(responseStream);

            string responseContent = await sr.ReadToEndAsync();

            return JsonConvert.DeserializeObject<ResponseType>(responseContent);
        }

        internal async Task LoginAndGetApiKey()
        {
            IPardotConfig config = MIFConfig.Instance as IPardotConfig;
            this.ExecuteWebRequest($"login/version/4?email={config.PardotEmail}&password={config.PardotPassword}&user_key={config.PardotUserKey}");
        }
    }
}
