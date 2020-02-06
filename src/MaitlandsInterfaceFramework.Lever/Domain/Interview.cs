using MaitlandsInterfaceFramework.Core.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Lever.Domain
{
    public class Interview
    {
        public class Interviewer
        {
            public string Email { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public string Id { get; set; }
        public string Panel { get; set; }
        public string Subject { get; set; }
        public string Note { get; set; }

        public Interviewer[] Interviewers { get; set; }

        public string Timezone { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime Date { get; set; }

        public int Duration { get; set; }
        public string Location { get; set; }
        public string FeedbackTemplate { get; set; }
        public string[] FeedbackForms { get; set; }
        public string FeedbackReminder { get; set; }
        public string User { get; set; }
        public string Stage { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        [JsonProperty("canceledAt")]
        public DateTime? CancelledAt { get; set; }

        public string[] Postings { get; set; }
    }
}
