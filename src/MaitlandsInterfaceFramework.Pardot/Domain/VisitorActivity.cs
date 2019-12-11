using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Pardot.Domain
{
    public class VisitorActivity
    {
        public int Id { get; set; }

        [JsonProperty("prospect_id")]
        public int ProspectId { get; set; }
    }
}
