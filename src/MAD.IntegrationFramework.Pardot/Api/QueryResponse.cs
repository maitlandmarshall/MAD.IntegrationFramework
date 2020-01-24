using MAD.IntegrationFramework.Core.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Pardot.Api
{
    [JsonConverter(typeof(NestedJsonConverter))]
    public class QueryResponse<T> : ApiResponse
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
