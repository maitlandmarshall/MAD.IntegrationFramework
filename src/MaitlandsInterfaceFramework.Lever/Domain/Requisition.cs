using MaitlandsInterfaceFramework.Core.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Lever.Domain
{
    public class CompensationBand
    {
        public string Currency { get; set; }
        public string Interval { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
    }

    public class Requisition
    {
        public string Id { get; set; }
        public string RequisitionCode { get; set; }
        public string Name { get; set; }
        public bool Backfill { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime CreatedAt { get; set; }
        public string Creator { get; set; }
        public int HeadcountHired { get; set; }
        public string HeadcountTotal { get; set; }
        public string Status { get; set; }
        public string HiringManager { get; set; }
        public string Owner { get; set; }

        public CompensationBand CompensationBand { get; set; }
        public string EmploymentStatus { get; set; }
        public string Location { get; set; }
        public string InternalNotes { get; set; }
        public string[] Postings { get; set; }
        public string Department { get; set; }
        public string Team { get; set; }

        public string[] OfferIds { get; set; }

        public JObject CustomFields { get; set; }
    }
}
