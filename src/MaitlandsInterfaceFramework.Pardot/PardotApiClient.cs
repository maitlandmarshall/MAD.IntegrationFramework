using MaitlandsInterfaceFramework.Core.Configuration;
using MaitlandsInterfaceFramework.Pardot.Api;
using MaitlandsInterfaceFramework.Pardot.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public string ApiKey { get; set; }

        public PardotApiClient()
        {
            IPardotConfig config = MIFConfig.Instance as IPardotConfig;

            if (config == null)
                throw new Exception($"{nameof(MIFConfig)} must implement {nameof(IPardotConfig)}.");
        }

        internal async Task<ResponseType> ExecuteWebRequest<ResponseType>(string relativeUri, JsonConverter jsonConverter = null, params (string argName, object argValue)[] args)
        {
            if (String.IsNullOrEmpty(this.ApiKey) && relativeUri.Contains("login") == false)
                await this.LoginAndGetApiKey();

            IPardotConfig config = MIFConfig.Instance as IPardotConfig;

            if (args.Length > 0)
            {
                string argSegment = String.Join("&", args.Where(y => y.argValue != null).Select(y => $"{y.argName}={y.argValue}"));
                relativeUri += $"?{argSegment}&format=json";
            }
            else
            {
                relativeUri += $"?format=json";
            }

            Uri requestUri = new Uri(new Uri(PardotApiBaseUri), relativeUri);

            HttpWebRequest request = HttpWebRequest.CreateHttp(requestUri.ToString());
            
            if (!String.IsNullOrEmpty(this.ApiKey))
                request.Headers.Add(HttpRequestHeader.Authorization, $"Pardot api_key={this.ApiKey}, user_key={config.PardotUserKey}");

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            using Stream responseStream = response.GetResponseStream();
            using StreamReader sr = new StreamReader(responseStream);

            string responseContent = await sr.ReadToEndAsync();

            if (typeof(ResponseType) == typeof(LoginResponse))
            {
                return JsonConvert.DeserializeObject<ResponseType>(responseContent);
            }
            else
            {
                JObject result = JsonConvert.DeserializeObject<JObject>(responseContent);

                if (args.Any(y => y.argName == "output" && (y.argValue as string) == "bulk"))
                {
                    if (jsonConverter != null)
                    {
                        JsonSerializer serializer = JsonSerializer.CreateDefault();
                        serializer.Converters.Add(jsonConverter);

                        return new JArray(result.Last.First.Last.First.ToList()).ToObject<ResponseType>(serializer);
                    }
                    else
                    {
                        return result.Last.First.Last.First.ToObject<ResponseType>();
                    }
                }
                else
                {
                    return result.Last.First.ToObject<ResponseType>();
                }
            }
        }

        internal async Task<LoginResponse> LoginAndGetApiKey()
        {
            IPardotConfig config = MIFConfig.Instance as IPardotConfig;

            LoginResponse response = await this.ExecuteWebRequest<LoginResponse>(
                relativeUri: $"login/version/4",
                null,
                ("email", config.PardotEmail), ("password", config.PardotPassword), ("user_key", config.PardotUserKey)
            );

            this.ApiKey = response.ApiKey;

            return response;
        }

        public async Task<Account> GetAccount()
        {
            return await this.ExecuteWebRequest<Account>("account/version/4/do/read");
        }

        public async Task<Email> GetEmail(int emailId)
        {
            return await this.ExecuteWebRequest<Email>($"email/version/4/do/read/id/{emailId}");
        }

        public Task<IEnumerable<TargetType>> PerformBulkQuery<TargetType>(BulkQueryParameters parameters = null, JsonConverter jsonConverter = null)
            => PerformBulkQuery<TargetType>(typeof(TargetType).Name, parameters, jsonConverter);
            
        public async Task<IEnumerable<TargetType>> PerformBulkQuery<TargetType>(string pardotApiEndpointName, BulkQueryParameters parameters = null, JsonConverter jsonConverter = null)
        {
            return await this.ExecuteWebRequest<List<TargetType>>($"{pardotApiEndpointName}/version/4/do/query", jsonConverter,
                ("output", "bulk"),
                ("created_before", parameters?.CreatedBefore),
                ("created_after", parameters?.CreatedAfter),
                ("updated_before", parameters?.UpdatedBefore),
                ("updated_after", parameters?.UpdatedAfter),
                ("id_greater_than", parameters?.IdGreaterThan),
                ("id_less_than", parameters?.IdLessThan)
            );
        }
    }
}
