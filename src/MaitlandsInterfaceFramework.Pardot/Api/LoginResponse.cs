using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Api
{
    public class LoginResponse : ApiResponse
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }
    }
}
