using MaitlandsInterfaceFramework.Core.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MaitlandsInterfaceFramework.Pardot.Api
{
    [JsonConverter(typeof(NestedJsonConverter))]
    public abstract class ApiResponse
    {
        public struct QueryResponseAttributes
        {
            public string Stat { get; set; }
            public int? Version { get; set; }

            [JsonProperty("err_code")]
            public int? ErrorCode { get; set; }
        }

        [JsonProperty("err")]
        public string Error { get; set; }

        [JsonProperty("@attributes")]
        public QueryResponseAttributes Attributes { get; set; }
    }
}
