using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Salesforce.Domain
{
    public class OpportunityHistory
    {
        public string Id { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? CloseDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string StageName { get; set; }
        public string CreatedById { get; set; }
        public string CreatedDate { get; set; }
        public decimal? Probability { get; set; }
        public string OpportunityId { get; set; }
        public string SystemModstamp { get; set; }
        public decimal? ExpectedRevenue { get; set; }
        public string ForecastCategory { get; set; }
    }
}
