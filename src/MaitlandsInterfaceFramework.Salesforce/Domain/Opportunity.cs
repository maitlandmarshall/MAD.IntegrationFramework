using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Salesforce.Domain
{
    public class Opportunity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool? IsWon { get; set; }
        public decimal? Amount { get; set; }
        public string Fiscal { get; set; }
        public string OwnerId { get; set; }
        public bool? IsClosed { get; set; }
        public string NextStep { get; set; }
        public string AccountId { get; set; }
        public DateTime? CloseDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string StageName { get; set; }
        public string CampaignId { get; set; }
        public int? FiscalYear { get; set; }
        public string LeadSource { get; set; }
        public string CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Description { get; set; }
        public decimal? Probability { get; set; }
        public string Pricebook2Id { get; set; }
        public int? FiscalQuarter { get; set; }
        public bool? HasOverdueTask { get; set; }
        public DateTime? LastViewedDate { get; set; }
        public string SystemModstamp { get; set; }
        public bool? HasOpenActivity { get; set; }
        public string ForecastCategory { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? LastReferencedDate { get; set; }
        public string ForecastCategoryName { get; set; }
        public bool? HasOpportunityLineItem { get; set; }
    }
}
