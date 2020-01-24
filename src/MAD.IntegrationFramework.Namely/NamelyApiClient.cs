using MAD.IntegrationFramework.Core.Configuration;
using MAD.IntegrationFramework.Namely.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Namely
{
    public class NamelyApiClient
    {
        public NamelyApiClient()
        {
            INamelyConfig config = MIFConfig.Instance as INamelyConfig;

            if (config == null)
                throw new Exception($"{nameof(MIFConfig)} must implement {nameof(INamelyConfig)}");
        }

        public async Task<ReportResponse> GetReport (string reportGuid)
        {
            INamelyConfig config = MIFConfig.Instance as INamelyConfig;

            if (String.IsNullOrEmpty(config.NamelyToken) || String.IsNullOrEmpty(config.NamelyClientName))
                throw new Exception($"Namely requires {nameof(config.NamelyToken)} and {nameof(config.NamelyClientName)}");

            HttpWebRequest request = HttpWebRequest.CreateHttp($"https://{config.NamelyClientName}.namely.com/api/v1/reports/{reportGuid}");
            request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {config.NamelyToken}");
            request.Headers.Add(HttpRequestHeader.Accept, "application/json");

            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
            string responseJson;

            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                responseJson = await sr.ReadToEndAsync();
            }

            return JsonConvert.DeserializeObject<ReportResponse>(responseJson);
        }
    }
}
