using MaitlandsInterfaceFramework.Core.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Lever.Domain
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime? DeactivatedAt { get; set; }

        public string ExternalDirectoryId { get; set; }
        public string AccessRole { get; set; }
        public string Photo { get; set; }
    }
}
