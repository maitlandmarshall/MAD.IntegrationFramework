using MaitlandsInterfaceFramework.Core.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Lever.Domain
{
    public class Referral
    {
        public class Field
        {
            public class Option
            {
                public string Text { get; set; }
            }

            public string Type { get; set; }
            public string Text { get; set; }
            public string Description { get; set; }
            public bool Required { get; set; }
            public string Value { get; set; }

            public string Prompt { get; set; }

            public Option[] Options { get; set; }
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public string Instructions { get; set; }

        public Field[] Fields { get; set; }

        public string BaseTemplateId { get; set; }
        public string User { get; set; }
        public string Referrer { get; set; }
        public string Stage { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime? CreatedAt { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime? CompletedAt{ get; set; }

    }
}
