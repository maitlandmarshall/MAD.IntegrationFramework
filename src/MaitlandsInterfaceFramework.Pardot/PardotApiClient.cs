using MaitlandsInterfaceFramework.Core.Configuration;
using MaitlandsInterfaceFramework.Core.Converters;
using MaitlandsInterfaceFramework.Pardot.Api;
using MaitlandsInterfaceFramework.Pardot.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
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
        private static BlockingCollection<object> ConcurrentRequests = new BlockingCollection<object>(5);

        public string ApiKey { get; set; }

        public PardotApiClient()
        {
            IPardotConfig config = MIFConfig.Instance as IPardotConfig;

            if (config == null)
                throw new Exception($"{nameof(MIFConfig)} must implement {nameof(IPardotConfig)}.");
        }

        internal async Task<ResponseType> ExecuteWebRequest<ResponseType>(string relativeUri, JsonConverter jsonConverter = null, params (string argName, object argValue)[] args)
        {
            string apiFinalUri = relativeUri;

            if (String.IsNullOrEmpty(this.ApiKey) && apiFinalUri.Contains("login") == false)
                await this.LoginAndGetApiKey();

            IPardotConfig config = MIFConfig.Instance as IPardotConfig;

            if (args.Length > 0)
            {
                string[] argumentsFormatted = args
                    .Where(y => y.argValue != null)
                    .Select(y => {
                        object argValue = y.argValue;
                        string argName = y.argName;

                        string argFinalValue;

                        switch (argValue)
                        {
                            case DateTime argValueDateTime:
                                argFinalValue = argValueDateTime.ToString("yyyy-MM-ddTHH:mm:ss");

                                break;
                            default:
                                argFinalValue = argValue.ToString();
                                break;
                        }

                        return $"{argName}={argFinalValue}";
                     }).ToArray();

                string argSegment = String.Join("&", argumentsFormatted);
                apiFinalUri += $"?{argSegment}&format=json";
            }
            else
            {
                apiFinalUri += $"?format=json";
            }

            Uri requestUri = new Uri(new Uri(PardotApiBaseUri), apiFinalUri);

            HttpWebRequest request = HttpWebRequest.CreateHttp(requestUri.ToString());
            Guid requestGuid = Guid.NewGuid();

            if (!String.IsNullOrEmpty(this.ApiKey))
                request.Headers.Add(HttpRequestHeader.Authorization, $"Pardot api_key={this.ApiKey}, user_key={config.PardotUserKey}");

            try
            {
                ConcurrentRequests.Add(new object());

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using Stream responseStream = response.GetResponseStream();
                using StreamReader sr = new StreamReader(responseStream);

                string responseContent = await sr.ReadToEndAsync();

                try
                {
                    if (typeof(ResponseType) == typeof(LoginResponse))
                    {
                        return JsonConvert.DeserializeObject<ResponseType>(responseContent);
                    }
                    else
                    {
                        JObject result = JsonConvert.DeserializeObject<JObject>(responseContent);
                        JToken errCode = result["@attributes"]["err_code"];

                        if (errCode != null)
                        {
                            int errorId = errCode.Value<int>();

                            // If the ApiKey has expired
                            if (errorId == 1)
                            {
                                // Reset the API key so the next request retrieves it automatically

                                this.ApiKey = null;
                                return await this.ExecuteWebRequest<ResponseType>(relativeUri, jsonConverter, args);
                            }
                            else
                            {
                                throw new Exception(responseContent);
                            }
                        }

                        if (args.Any(y => y.argName == "output" && (y.argValue as string) == "bulk"))
                        {
                            JToken resultItems = result["result"];

                            if (resultItems.Type == JTokenType.Null)
                                return default(ResponseType);

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
                            if (jsonConverter != null)
                            {
                                JsonSerializer serializer = JsonSerializer.CreateDefault();
                                serializer.Converters.Add(jsonConverter);

                                return JObject.FromObject(result.Last.First).ToObject<ResponseType>(serializer);
                            }
                            else
                            {
                                return result.Last.First.ToObject<ResponseType>();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(responseContent, ex);
                }
            }
            finally
            {
                ConcurrentRequests.Take();
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
            if (emailId == 0)
                throw new Exception($"{nameof(emailId)} must be greater than 0");

            return await this.ExecuteWebRequest<Email>($"email/version/4/do/read/id/{emailId}", new NestedJsonConverter());
        }

        public async Task<IEnumerable<Visit>> GetVisits(params int[] visitorIds)
        {
            return await this.ExecuteWebRequest<List<Visit>>("visit/version/4/do/query", new NestedJsonConverter(), ("visitor_ids", String.Join(",", visitorIds)));
        }

        public Task<IEnumerable<TargetType>> PerformBulkQuery<TargetType>(BulkQueryParameters parameters = null, JsonConverter jsonConverter = null) where TargetType : IEntity
            => PerformBulkQuery<TargetType>(typeof(TargetType).Name, parameters, jsonConverter);
            
        public async Task<IEnumerable<TargetType>> PerformBulkQuery<TargetType>(string pardotApiEndpointName, BulkQueryParameters parameters = null, JsonConverter jsonConverter = null) where TargetType : IEntity
        {
            const int maxBulkQueryReturnResult = 200;

            List<TargetType> totalResult = new List<TargetType>();
            int lastPaginatedResultCount = 0;

            DateTime? createdBefore = parameters?.CreatedBefore;
            DateTime? createdAfter = parameters?.CreatedAfter;
            DateTime? updatedBefore = parameters?.UpdatedBefore;
            DateTime? updatedAfter = parameters?.UpdatedAfter;
            int? idGreaterThan = parameters?.IdGreaterThan;
            int? idLessThan = parameters?.IdLessThan;

            bool isImmutableEntity = typeof(IImmutableEntity).IsAssignableFrom(typeof(TargetType));
            bool isMutableEntity = typeof(IMutableEntity).IsAssignableFrom(typeof(TargetType));

            do
            {
                List<TargetType> paginatedResult = await this.ExecuteWebRequest<List<TargetType>>($"{pardotApiEndpointName}/version/4/do/query", jsonConverter,
                    ("output", "bulk"),
                    ("created_before", createdBefore),
                    ("created_after", createdAfter),
                    ("updated_before", updatedBefore),
                    ("updated_after", updatedAfter),
                    ("id_greater_than", idGreaterThan),
                    ("id_less_than", idLessThan)) 
                ?? new List<TargetType>();

                lastPaginatedResultCount = paginatedResult.Count;

                if (paginatedResult.Count == 0)
                    break;

                totalResult.AddRange(paginatedResult);

                if (!createdBefore.HasValue
                    && !createdAfter.HasValue
                    && !updatedBefore.HasValue
                    && !updatedAfter.HasValue
                    && !idGreaterThan.HasValue
                    && !idLessThan.HasValue)
                {
                    idGreaterThan = paginatedResult.Max(y => y.Id);
                }
                else
                {
                    if (createdBefore.HasValue && isImmutableEntity)
                    {
                        createdBefore = paginatedResult.Cast<IImmutableEntity>().Min(y => y.CreatedAt);
                    }

                    if (updatedBefore.HasValue && isMutableEntity)
                    {
                        updatedBefore = paginatedResult.Cast<IMutableEntity>().Min(y => y.UpdatedAt);
                    }

                    if (createdAfter.HasValue && isImmutableEntity)
                    {
                        createdAfter = paginatedResult.Cast<IImmutableEntity>().Max(y => y.CreatedAt);
                    }

                    if (updatedAfter.HasValue && isMutableEntity)
                    {
                        updatedAfter = paginatedResult.Cast<IMutableEntity>().Max(y => y.CreatedAt);
                    }

                    if (idGreaterThan.HasValue)
                        idGreaterThan = paginatedResult.Max(y => y.Id);

                    if (idLessThan.HasValue)
                        idLessThan = paginatedResult.Min(y => y.Id);
                }

            } while (lastPaginatedResultCount == maxBulkQueryReturnResult);

            return totalResult;
        }
    }
}
