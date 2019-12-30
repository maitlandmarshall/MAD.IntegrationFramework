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
        private const int MaxQueryResults = 200;

        private const string PardotApiBaseUri = "https://pi.pardot.com/api/";
        private static BlockingCollection<object> ConcurrentRequests = new BlockingCollection<object>(5);

        public string ApiKey { get; set; }

        public PardotApiClient()
        {
            IPardotConfig config = MIFConfig.Instance as IPardotConfig;

            if (config == null)
                throw new Exception($"{nameof(MIFConfig)} must implement {nameof(IPardotConfig)}.");
        }

        internal async Task<ResponseType> ExecuteWebRequest<ResponseType>(string relativeUri, params (string argName, object argValue)[] args)
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
                    ResponseType result = JsonConvert.DeserializeObject<ResponseType>(responseContent);

                    if (result is QueryResponse queryResponse)
                    {
                        if (queryResponse.Attributes.ErrorCode.HasValue)
                        {
                            if (queryResponse.Attributes.ErrorCode == 1)
                            {
                                this.ApiKey = null;

                                return await this.ExecuteWebRequest<ResponseType>(relativeUri, args);
                            }
                            else
                            {
                                throw new Exception(queryResponse.Error);
                            }
                        }
                    }

                    return result;
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
                ("email", config.PardotEmail), 
                ("password", config.PardotPassword), 
                ("user_key", config.PardotUserKey)
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

            return await this.ExecuteWebRequest<Email>($"email/version/4/do/read/id/{emailId}");
        }

        public async Task<IEnumerable<Visit>> GetVisits(params int[] visitorIds)
        {
            List<Visit> totalVisits = new List<Visit>();
            QueryResponse<Visit> paginatedResponse;

            do
            {
                paginatedResponse = await this.ExecuteWebRequest<QueryResponse<Visit>>("visit/version/4/do/query", 
                    ("visitor_ids", String.Join(",", visitorIds)),
                    ("offset", totalVisits.Count));

                totalVisits.AddRange(paginatedResponse.Result.Items);

            } while (paginatedResponse.Result.Items.Count > 0 && (!paginatedResponse.Result.TotalResults.HasValue || totalVisits.Count != paginatedResponse.Result.TotalResults.Value));

            return totalVisits;
        }

        public Task<IEnumerable<TargetType>> PerformBulkQuery<TargetType>(BulkQueryParameters parameters = null, JsonConverter jsonConverter = null) where TargetType : IEntity
            => PerformBulkQuery<TargetType>(typeof(TargetType).Name, parameters);
            
        public async Task<IEnumerable<TargetType>> PerformBulkQuery<TargetType>(string pardotApiEndpointName, BulkQueryParameters parameters = null) where TargetType : IEntity
        {
            List<TargetType> totalResult = new List<TargetType>();
            int lastPaginatedResultCount = 0;

            DateTime? createdBefore = parameters?.CreatedBefore;
            DateTime? createdAfter = parameters?.CreatedAfter;
            DateTime? updatedBefore = parameters?.UpdatedBefore;
            DateTime? updatedAfter = parameters?.UpdatedAfter;
            int? idGreaterThan = parameters?.IdGreaterThan;
            int? idLessThan = parameters?.IdLessThan;
            int? take = parameters?.Take;

            bool isImmutableEntity = typeof(IImmutableEntity).IsAssignableFrom(typeof(TargetType));
            bool isMutableEntity = typeof(IMutableEntity).IsAssignableFrom(typeof(TargetType));

            do
            {
                QueryResponse<TargetType> queryResponse = await this.ExecuteWebRequest<QueryResponse<TargetType>>($"{pardotApiEndpointName}/version/4/do/query",
                    //("output", "bulk"),
                    ("created_before", createdBefore),
                    ("created_after", createdAfter),
                    ("updated_before", updatedBefore),
                    ("updated_after", updatedAfter),
                    ("id_greater_than", idGreaterThan),
                    ("id_less_than", idLessThan));

                List<TargetType> items = queryResponse.Result.Items;

                lastPaginatedResultCount = items.Count;

                if (lastPaginatedResultCount == 0)
                    break;

                totalResult.AddRange(items);

                if (!createdBefore.HasValue
                    && !createdAfter.HasValue
                    && !updatedBefore.HasValue
                    && !updatedAfter.HasValue
                    && !idGreaterThan.HasValue
                    && !idLessThan.HasValue)
                {
                    idGreaterThan = items.Max(y => y.Id);
                }
                else
                {
                    if (createdBefore.HasValue && isImmutableEntity)
                    {
                        createdBefore = items.Cast<IImmutableEntity>().Min(y => y.CreatedAt);
                    }

                    if (updatedBefore.HasValue && isMutableEntity)
                    {
                        updatedBefore = items.Cast<IMutableEntity>().Min(y => y.UpdatedAt);
                    }

                    if (createdAfter.HasValue && isImmutableEntity)
                    {
                        createdAfter = items.Cast<IImmutableEntity>().Max(y => y.CreatedAt);
                    }

                    if (updatedAfter.HasValue && isMutableEntity)
                    {
                        updatedAfter = items.Cast<IMutableEntity>().Max(y => y.CreatedAt);
                    }

                    if (idGreaterThan.HasValue)
                        idGreaterThan = items.Max(y => y.Id);

                    if (idLessThan.HasValue)
                        idLessThan = items.Min(y => y.Id);
                }

            } while (lastPaginatedResultCount == MaxQueryResults && (!take.HasValue || totalResult.Count < take.Value));

            return totalResult;
        }
    }
}
