using MAD.IntegrationFramework.Core.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Pardot.Domain
{
    [JsonConverter(typeof(NestedJsonConverter))]
    public class Account : IMutableEntity
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Level { get; set; }
        public string Website { get; set; }

        [JsonProperty("vanity_domain")]
        public string VanityDomain { get; set; }

        [JsonProperty("plugin_campaign_id")]
        public int PluginCampaignId { get; set; }

        [JsonProperty("tracking_code_template")]
        public string TrackingCodeTemplate { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Territory { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
