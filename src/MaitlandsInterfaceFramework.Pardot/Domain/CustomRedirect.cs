using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Domain
{
    public class CustomRedirect
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        [JsonProperty("destination_url")]
        public string DestinationUrl { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public Campaign Campaign { get; set; }
    }
}
