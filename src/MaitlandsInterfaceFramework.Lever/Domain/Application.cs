using MaitlandsInterfaceFramework.Core.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Lever.Domain
{
    public class Application
    {
        public class RequisitionForHireContainer
        {
            public string Id { get; set; }
            public string RequisitionCode { get; set; }
            public string HiringManagerOnHire { get; set; }
        }

        public class CustomQuestion
        {
            public class Option
            {
                public string Text { get; set; }
            }

            public string Type { get; set; }
            public string Text { get; set; }
            public string Description { get; set; }

            public bool? Required { get; set; }
            public Option[] Options { get; set; }

            public JToken Value { get; set; }

            public string BaseTemplateId { get; set; }
            public string Stage { get; set; }

            [JsonConverter(typeof(MillisecondEpochConverter))]
            public DateTime? CreatedAt { get; set; }

            [JsonConverter(typeof(MillisecondEpochConverter))]
            public DateTime? CompletedAt { get; set; }
        }

        public string Id { get; set; }
        public string CandidateId { get; set; }
        public string OpportunityId { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime CreatedAt { get; set; }

        public string Type { get; set; }

        public string Posting { get; set; }
        public string PostingOwner { get; set; }
        public string PostingHiringManager { get; set; }

        public string User { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public Phone Phone { get; set; }
        public string Company { get; set; }

        public string[] Links { get; set; }
        public string Comments { get; set; }
        public string Resume { get; set; }

        public RequisitionForHireContainer RequisitionForHire { get; set; }
        public CustomQuestion[] CustomQuestions { get; set; }
    }
}
