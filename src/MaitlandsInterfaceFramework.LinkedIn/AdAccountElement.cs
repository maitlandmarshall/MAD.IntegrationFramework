using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.LinkedIn
{
    public class AdAccountElement
    {
        public string Currency { get; set; }

        [JsonProperty("Id")]
        public long AccountId { get; set; }

        public string Name { get; set; }

        public bool NotifiedOnCampaignOptimization { get; set; }
        public bool NotifiedOnCreativeApproval { get; set; }
        public bool NotifiedOnCreativeRejection { get; set; }
        public bool NotifiedOnEndOfCampaign { get; set; }
        public bool NotifiedOnNewFeaturesEnabled { get; set; }

        public string Reference { get; set; }

        public List<string> ServingStatuses { get; set; }

        public string Status { get; set; }
        public string Type { get; set; }
    }
}
