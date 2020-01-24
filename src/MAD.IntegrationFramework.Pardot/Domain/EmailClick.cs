using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Pardot.Domain
{
    public class EmailClick : IImmutableEntity
    {
        public int Id { get; set; }

        [JsonProperty("prospect_id")]
        public int ProspectId { get; set; }

        public string Url { get; set; }

        [JsonProperty("list_email_id")]
        public int? ListEmailId { get; set; }

        [JsonProperty("email_template_id")]
        public int? EmailTemplateId { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
