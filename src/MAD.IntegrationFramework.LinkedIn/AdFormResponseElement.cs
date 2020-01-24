using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MAD.IntegrationFramework.LinkedIn
{
    public class AdFormResponseElement
    {
        public string Account { get; set; }
        public string Campaign { get; set; }
        public string Creative { get; set; } 
        public string Form { get; set; }

        [JsonProperty("Id")]
        public string FormId { get; set; }
        public string LeadType { get; set; }

        public List<AdFormResponseAnswer> FormResponse { get; set; }

    }

    public class AdFormResponseAnswer
    {
        public List<IDictionary<string, object>> Answers { get; set; }
        public List<IDictionary<string, object>> ContentResponses { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
