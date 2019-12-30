using MaitlandsInterfaceFramework.Core.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MaitlandsInterfaceFramework.Pardot.Api
{
    [JsonConverter(typeof(NestedJsonConverter))]
    public abstract class QueryResponse
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

    [JsonConverter(typeof(NestedJsonConverter))]
    public class QueryResponse<T> : QueryResponse
    {
        [JsonConverter(typeof(NestedJsonConverter))]
        public class QueryResponseResult
        {
            [JsonProperty("total_results")]
            public int? TotalResults { get; set; }

            [JsonProperty("{},[]")]
            public List<T> Items { get; set; }
        }

        public QueryResponseResult Result { get; set; }
    }
}
