using Newtonsoft.Json;
using System;

namespace MAD.IntegrationFramework.LinkedIn
{
    public class LikeElement
    {
        public string Actor { get; set; }
        public ApiAction Created { get; set; }
        public ApiAction LastModified { get; set; }

        [JsonProperty("$URN")]
        public string Urn { get; set; }

        public string Object { get; set; }
    }
}