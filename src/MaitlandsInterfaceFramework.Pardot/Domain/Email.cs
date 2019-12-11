using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Domain
{
    public class Email
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
