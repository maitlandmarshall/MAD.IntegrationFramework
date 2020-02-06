using MaitlandsInterfaceFramework.Core.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Lever.Domain
{
    public class Opportunity
    {
        public class StageChange
        {
            public string ToStageId { get; set; }
            public int ToStageIndex { get; set; }
            public string UserId { get; set; }

            [JsonConverter(typeof(MillisecondEpochConverter))]
            public DateTime? UpdatedAt { get; set; }
        }

        public class Url
        {
            public string List { get; set; }
            public string Show { get; set; }
        }

        public class DataProtectionContainer
        {
            public class DataProtectionDetail
            {
                public bool Allowed { get; set; }

                [JsonConverter(typeof(MillisecondEpochConverter))]
                public DateTime? ExpiresAt { get; set; }
            }

            public DataProtectionDetail Store { get; set; }
            public DataProtectionDetail Contact { get; set; }
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Headline { get; set; }
        public string Contact { get; set; }
        public string[] Emails { get; set; }

        public Phone[] Phones { get; set; }
        public string Location { get; set; }
        public string[] Links { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime LastInteractionAt { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime LastAdvancedAt { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime? SnoozedUntil { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime? ArchivedAt { get; set; }

        public string ArchivedReason { get; set; }

        public string Stage { get; set; }

        public StageChange[] StageChanges { get; set; }
        public string Owner { get; set; }

        public string[] Tags { get; set; }
        public string[] Sources { get; set; }

        public string Origin { get; set; }
        public string[] Applications { get; set; }

        public string Resume { get; set; }
        public string[] Followers { get; set; }

        public Url Urls { get; set; }
        public DataProtectionContainer DataProtection { get; set; }

        public bool IsAnonymized { get; set; }

    }
}
