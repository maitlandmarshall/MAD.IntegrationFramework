using MaitlandsInterfaceFramework.Core.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Lever.Domain
{
    public class Offer
    {
        public class Field
        {
            public string Text { get; set; }
            public string Identifier { get; set; }
            public string Value { get; set; }
        }

        public string Id { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }
        public string Creator { get; set; }

        public Field[] Fields { get; set; }
    }
}
