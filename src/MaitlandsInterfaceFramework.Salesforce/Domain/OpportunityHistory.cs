using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Salesforce.Domain
{
    public class OpportunityHistory : ISalesforceEntity
    {
        public string Id { get; set; }
        public string OpportunityId { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StageName { get; set; }
        public double? Amount { get; set; }
        public double? ExpectedRevenue { get; set; }
        public DateTime? CloseDate { get; set; }
        public double? Probability { get; set; }
        public string ForecastCategory { get; set; }
        public DateTime SystemModstamp { get; set; }
        public bool IsDeleted { get; set; }
    }
}
