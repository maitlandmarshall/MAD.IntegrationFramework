using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Domain
{
    public class Opportunity : IMutableEntity
    {
        public int Id { get; set; }

        public Campaign Campaign { get; set; }

        public string Name { get; set; }

        public decimal Value { get; set; }
        public int Probability { get; set; }
        public string Type { get; set; }
        public string Stage { get; set; }
        public string Status { get; set; }

        [JsonProperty("closed_at")]
        public DateTime? ClosedAt { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public JObject Prospects { get; set; }
    }
}
