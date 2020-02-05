using MaitlandsInterfaceFramework.Core.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Lever.Domain
{
    public class Posting
    {
        public class ContentList
        {
            public string Text { get; set; }
            public string Content { get; set; }
        }

        public class ContentContainer
        {
            public string Description { get; set; }
            public string DescriptionHtml { get; set; }
            public ContentList[] Lists { get; set; }
            public string Closing { get; set; }
            public string ClosingHtml { get; set; }
            public JArray CustomQuestions { get; set; }
        }

        public class Category
        {
            public string Team { get; set; }
            public string Department { get; set; }
            public string Location { get; set; }
            public string Commitment { get; set; }
            public string Level { get; set; }
        }

        public class UrlContainer
        {
            public string List { get; set; }
            public string Show { get; set; }
            public string Apply { get; set; }
        }

        public string Id { get; set; }
        public string Text { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime UpdatedAt { get; set; }

        public string User { get; set; }
        public string Owner { get; set; }
        public string HiringManager { get; set; }
        public Category Categories { get; set; }
        public ContentContainer Content { get; set; }

        public JArray Tags { get; set; }
        public string State { get; set; }
        public string[] DistributionChannels { get; set; }
        public string ReqCode { get; set; }
        public string[] RequisitionCodes { get; set; }

        public UrlContainer Urls { get; set; }
    }
}
