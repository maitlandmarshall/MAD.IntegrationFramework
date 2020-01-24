using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Pardot.Domain
{
    public class VisitorActivity : IImmutableEntity
    {
        public int Id { get; set; }

        [JsonProperty("prospect_id")]
        public int? ProspectId { get; set; }

        [JsonProperty("visitor_id")]
        public int? VisitorId { get; set; }

        public int Type { get; set; }

        [JsonProperty("type_name")]
        public string TypeName { get; set; }

        public string Details { get; set; }

        [JsonProperty("email_id")]
        public int? EmailId { get; set; }

        [JsonProperty("email_template_id")]
        public int? EmailTemplateId { get; set; }

        [JsonProperty("list_email_id")]
        public int? ListEmailId { get; set; }

        [JsonProperty("form_id")]
        public int? FormId { get; set; }

        [JsonProperty("form_handler_id")]
        public int? FormHandlerId { get; set; }

        [JsonProperty("site_search_query_id")]
        public int? SiteSearchQueryId { get; set; }

        [JsonProperty("landing_page_id")]
        public int? LandingPageId { get; set; }

        [JsonProperty("paid_search_ad_id")]
        public int? PaidSearchAdId { get; set; }

        [JsonProperty("multivariate_test_variation_id")]
        public int? MultivariateTestVariationId { get; set; }

        [JsonProperty("visitor_page_view_id")]
        public int? VisitorPageViewId { get; set; }

        [JsonProperty("file_id")]
        public int? FileId { get; set; }

        [JsonProperty("campaign_id")]
        public int? CampaignId { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
