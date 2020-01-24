using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MAD.IntegrationFramework.Pardot.Domain
{
    public class Visitor : IMutableEntity
    {
        public int Id { get; set; }

        [JsonProperty("page_view_count")]
        public int? PageViewCount { get; set; }

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

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            if (errorContext.Member is string memberString)
            {
                if (memberString == "hostname")
                    errorContext.Handled = true;
            }
        }

    }
}
