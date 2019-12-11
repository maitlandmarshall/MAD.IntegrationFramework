using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Domain
{
    public class Visitor
    {
        public int Id { get; set; }

        [JsonProperty("page_view_count")]
        public int PageViewCount { get; set; }

        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }

        public string Hostname { get; set; }

        [JsonProperty("campaign_parameter")]
        public string CampaignParameter { get; set; }

        [JsonProperty("medium_parameter")]
        public string MediumParameter { get; set; }

        [JsonProperty("source_parameter")]
        public string SourceParameter { get; set; }

        [JsonProperty("content_parameter")]
        public string ContentParameter { get; set; }

        [JsonProperty("term_parameter")]
        public string TermParameter { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
