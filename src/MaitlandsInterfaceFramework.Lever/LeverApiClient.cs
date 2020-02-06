using MaitlandsInterfaceFramework.Core.Configuration;
using MaitlandsInterfaceFramework.Lever.Api;
using MaitlandsInterfaceFramework.Lever.Domain;
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
using System.Web;

namespace MaitlandsInterfaceFramework.Lever
{
    public class LeverApiClient
    {
        private const string ApiBaseUri = "https://api.lever.co/v1";

        private ILeverConfig Config
        {
            get => MIFConfig.Instance as ILeverConfig;
        }

        public LeverApiClient()
        {
            if (this.Config == null)
                throw new Exception($"{nameof(MIFConfig)} must implement {nameof(ILeverConfig)}");
        }

        private async Task<T> ExecuteRequest<T>(string relativeUri, string parameters = "")
        {
            HttpWebRequest request = HttpWebRequest.CreateHttp(Path.Combine(ApiBaseUri, relativeUri.ToLower()) + parameters);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.None;

            if (String.IsNullOrEmpty(this.Config.LeverApiKey))
                throw new Exception($"{nameof(this.Config.LeverApiKey)} must have a value");

            request.Headers.Add(HttpRequestHeader.Authorization, $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Config.LeverApiKey))}:");
            request.Headers.Add(HttpRequestHeader.Accept, "application/json");

            try
            {
                using HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
                using Stream responseStream = response.GetResponseStream();
                using StreamReader sr = new StreamReader(responseStream);

                string responseBody = await sr.ReadToEndAsync();

                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch (WebException)
            {
                throw;
            }
        }

        private async Task<IEnumerable<T>> GetAllForEndpoint<T> (string endpoint, int? take = null)
        {
            ApiResponse<T> apiResult;
            List<T> result = new List<T>();

            string offset = String.Empty;

            do
            {
                apiResult = await this.ExecuteRequest<ApiResponse<T>>(endpoint, offset);
                result.AddRange(apiResult.Data);

                if (apiResult.HasNext == true)
                {
                    if (take.HasValue)
                    {
                        if (result.Count >= take.Value)
                            break;
                    }

                    string nextGuid = HttpUtility.UrlDecode(apiResult.Next);
                    offset = $"?offset={nextGuid}";
                }
                    
            } while (apiResult.HasNext == true);

            if (take.HasValue)
            {
                return result.Take(take.Value);
            }
            else
            {
                return result;
            }
        }

        public async Task<IEnumerable<Requisition>> Requisitions(int? take = null)
        {
            return await this.GetAllForEndpoint<Requisition>(nameof(this.Requisitions), take);
        }

        public async Task<IEnumerable<Posting>> Postings(int? take = null)
        {
            return await this.GetAllForEndpoint<Posting>(nameof(this.Postings), take);
        }

        public async Task<IEnumerable<Stage>> Stages(int? take = null)
        {
            return await this.GetAllForEndpoint<Stage>(nameof(this.Stages), take);
        }

        public async Task<IEnumerable<Opportunity>> Candidates(int? take = null)
        {
            return await this.GetAllForEndpoint<Opportunity>(nameof(this.Candidates), take);
        }

        public async Task<IEnumerable<Opportunity>> Opportunities(int? take = null)
        {
            return await this.GetAllForEndpoint<Opportunity>(nameof(this.Opportunities), take);
        }

        public async Task<IEnumerable<Offer>> OffersForOpportunity(string opportunityId, int? take = null)
        {
            return await this.GetAllForEndpoint<Offer>($"opportunities/{opportunityId}/offers", take);
        }

        public async Task<IEnumerable<Offer>> OffersForCandidate(string candidateId, int? take = null)
        {
            return await this.GetAllForEndpoint<Offer>($"candidates/{candidateId}/offers", take);
        }

        public async Task<IEnumerable<Referral>> ReferralsForCandidate(string candidateId, int? take = null)
        {
            return await this.GetAllForEndpoint<Referral>($"candidates/{candidateId}/referrals", take);
        }

        public async Task<IEnumerable<Referral>> ReferralsForOpportunity(string opportunityId, int? take = null)
        {
            return await this.GetAllForEndpoint<Referral>($"opportunities/{opportunityId}/referrals", take);
        }

        public async Task<IEnumerable<Interview>> InterviewsForOpportunity(string opportunityId, int? take = null)
        {
            return await this.GetAllForEndpoint<Interview>($"opportunities/{opportunityId}/interviews", take);
        }

        public async Task<IEnumerable<Interview>> InterviewsForCandidate(string candidateId, int? take = null)
        {
            return await this.GetAllForEndpoint<Interview>($"candidates/{candidateId}/interviews", take);
        }

        public async Task<IEnumerable<User>> Users(int? take = null)
        {
            return await this.GetAllForEndpoint<User>(nameof(this.Users), take);
        }

        public async Task<IEnumerable<Application>> ApplicationsForCandidate(string candidateId, int? take = null)
        {
            return await this.GetAllForEndpoint<Application>($"candidates/{candidateId}/applications", take);
        }
    }
}
