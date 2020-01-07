using MaitlandsInterfaceFramework.Core.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Domain
{ 
    [JsonConverter(typeof(NestedJsonConverter))]
    public class Email : IImmutableEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Subject { get; set; }

        [JsonProperty("message.html")]
        public string MessageHtml { get; set; }

        [JsonProperty("message.text")]
        public string MessageText { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
