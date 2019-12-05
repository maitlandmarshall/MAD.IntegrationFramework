using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.LinkedIn
{
    public struct AuditStamp
    {
        public decimal Time { get; set; }
    }

    public struct AmountCurrency
    {
        public string Amount { get; set; }
        public string CurrencyCode { get; set; }
    }

    public struct Locale
    {
        public string Country { get; set; }
        public string Language { get; set; }
    }

    public struct Schedule
    {
        public decimal Start { get; set; }
        public decimal End { get; set; }
    }

    public class CampaignElement
    {
        public class ChangeAuditStamp
        {
            public AuditStamp Created { get; set; }
            public AuditStamp LastModified { get; set; }
        }

        public string Account { get; set; }
        public string AssociatedEntity { get; set; }
        public bool AudienceExpansionsEnabled { get; set; }
        public string CampaignGroup { get; set; }
        
        public ChangeAuditStamp ChangeAuditStamps { get; set; }

        public string CostType { get; set; }
        public string CreativeSelection { get; set; }

        public AmountCurrency DailyBudget { get; set; }

        [JsonProperty("id")]
        public string CampaignId { get; set; }

        public Locale Locale { get; set; }

        public string Name { get; set; }

        public bool OffsiteDeliveryEnabled { get; set; }
        public string OptimizationTargetType { get; set; }

        public Schedule RunSchedule { get; set; }

        public List<string> ServingStatuses { get; set; }
        public string Status { get; set; }

        public string Type { get; set; }

        public AmountCurrency UnitCost { get; set; }
    }
}
