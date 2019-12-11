using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Api
{
    public class LoginResponse
    {
        [JsonProperty("api_key")]
        public string ApiKey { get; set; }
    }
}
