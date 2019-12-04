using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.LinkedIn
{
    public class ProfileElement
    {
        public string LocalizedFirstName { get; set; }
        public string LocalizedHeadline { get; set; }
        public string VanityName { get; set; }
        
        [JsonProperty("id")]
        public string ProfileId { get; set; }

        public string LocalizedLastName { get; set; }
    }
}
