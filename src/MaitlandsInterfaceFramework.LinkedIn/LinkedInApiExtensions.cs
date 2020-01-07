using MaitlandsInterfaceFramework.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Sparkle.LinkedInNET;
using Sparkle.LinkedInNET.Common;
using Sparkle.LinkedInNET.Organizations;
using Sparkle.LinkedInNET.Profiles;
using Sparkle.LinkedInNET.Shares;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace MaitlandsInterfaceFramework.LinkedIn
{
    public static class LinkedInApiExtensions
    {
        public static LinkedInApiPaginatedResult<AdAnalyticsElement> GetAdsReporting(this LinkedInApi api, OrgEntElements company, DateTime startDate, DateTime? endDate = null)
        {
            StringBuilder apiQueryBuilder = new StringBuilder();
            apiQueryBuilder.Append("/v2/adAnalyticsV2?q=analytics&pivot=CAMPAIGN&timeGranularity=DAILY");

            // Build the startDate param into the query 
            apiQueryBuilder.Append($"&dateRange.start.day={startDate.Day}&dateRange.start.month={startDate.Month}&dateRange.start.year={startDate.Year}");

            if (endDate.HasValue)
            {
                // Build the endDate param into the query
                apiQueryBuilder.Append($"&dateRange.end.day={endDate?.Day}&dateRange.end.month={endDate?.Month}&dateRange.end.year={endDate?.Year}");
            }

            apiQueryBuilder.Append($"&companies[0]={company.OrganizationalTarget}");

            string resultJson = api.RawGetJsonQuery(apiQueryBuilder.ToString(), LinkedInApiFactory.GetUserAuthorization());
            LinkedInApiPaginatedResult<AdAnalyticsElement> result = JsonConvert.DeserializeObject<LinkedInApiPaginatedResult<AdAnalyticsElement>>(resultJson);

            return result;
        }

        public static LinkedInApiPaginatedResult<LikeElement> GetLikesForShare(this LinkedInApi api, PostShareResult share, int count = 500, int start = 0)
            => GetLikesForActivity(api, share.Activity, count, start);

        public static LinkedInApiPaginatedResult<LikeElement> GetLikesForActivity(this LinkedInApi api, string activityUrn, int count = 500, int start = 0)
        {
            string apiPath = $"/v2/socialActions/{activityUrn}/likes?count={count}&start={start}";

            string likesJson = api.RawGetJsonQuery(apiPath, LinkedInApiFactory.GetUserAuthorization());
            LinkedInApiPaginatedResult<LikeElement> result = JsonConvert.DeserializeObject<LinkedInApiPaginatedResult<LikeElement>>(likesJson);

            return result;
        }

        public static LinkedInApiPaginatedResult<AdAccountElement> GetAdAccounts(this LinkedInApi api)
        {
            string adAccountsJson = api.RawGetJsonQuery("/v2/adAccountsV2?q=search", LinkedInApiFactory.GetUserAuthorization());
            LinkedInApiPaginatedResult<AdAccountElement> adAccounts = JsonConvert.DeserializeObject<LinkedInApiPaginatedResult<AdAccountElement>>(adAccountsJson);

            return adAccounts;
        }

        public static List<ProfileElement> GetProfilesByIds(this LinkedInApi api, IEnumerable<string> personIds)
        {
            if (personIds.Count() == 0)
            {
                return new List<ProfileElement>();
            }

            List<ProfileElement> result = new List<ProfileElement>();
            const int batchAmount = 50;

            do
            {
                IEnumerable<string> batch = personIds.Take(batchAmount);

                string personIdsConcat = String.Join(",", batch.Select(y => $"(id:{y.Split(':').Last()})"));
                PersonList profiles = api.Profiles.GetProfilesByIds(LinkedInApiFactory.GetUserAuthorization(), personIdsConcat);

                foreach (var p in profiles.Results)
                {
                    JProperty jProperty = p as JProperty;
                    ProfileElement profile = jProperty.First.ToObject<ProfileElement>();

                    if (profile.ProfileId == "private")
                        continue;

                    result.Add(profile);
                }

                personIds = personIds.Skip(batchAmount);
            } while (personIds.Any());

            return result;
        }

        public static LinkedInApiPaginatedResult<CampaignElement> GetAdCampaignsForCompany(this LinkedInApi api, OrgEntElements company)
        {
            string campaignsJson = api.RawGetJsonQuery(
                path: $"/v2/adCampaignsV2?q=search&search.associatedEntity.values[0]={company.OrganizationalTarget}", 
                user: LinkedInApiFactory.GetUserAuthorization()
            );

            LinkedInApiPaginatedResult<CampaignElement> campaigns = JsonConvert.DeserializeObject<LinkedInApiPaginatedResult<CampaignElement>>(campaignsJson);

            return campaigns;
        }

        public static CampaignElement GetAdCampaign(this LinkedInApi api, string campaignId)
        {
            string campaignJson = api.RawGetJsonQuery(
                path: $"/v2/adCampaignsV2/{campaignId}",
                user: LinkedInApiFactory.GetUserAuthorization()
            );

            CampaignElement campaign = JsonConvert.DeserializeObject<CampaignElement>(campaignJson);

            return campaign;
        }

        public static JObject GetSocialActionSummary (this LinkedInApi api, params string[] shareUrns)
        {
            return JsonConvert.DeserializeObject<JObject>(api.RawGetJsonQuery(
                path: $"/v2/socialActions?{String.Join("&", shareUrns.Select(y => $"ids={y}"))}",
                user: LinkedInApiFactory.GetUserAuthorization()
            ));
        }

        public static IEnumerable<PostShareResult> GetAllSharesForOrganization(this LinkedInApi api, string organizationUrn)
        {
            PostShares sharesPage;
            HashSet<string> activities = new HashSet<string>();
            int currentRecordIndex = 0;

            do
            {
                sharesPage = api.Shares.GetShares(
                    user: LinkedInApiFactory.GetUserAuthorization(),
                    urn: organizationUrn,
                    sharesPerOwner: 1000,
                    count: 50,
                    start: currentRecordIndex
                 );

                if (sharesPage.Elements.Count == 0)
                    yield break;

                foreach (PostShareResult share in sharesPage.Elements)
                {
                    if (activities.Contains(share.Activity))
                        continue;

                    yield return share;
                    activities.Add(share.Activity);
                }

                currentRecordIndex += sharesPage.Elements.Count;

            } while (sharesPage.Paging.Total > 0);
        }
    }
}
