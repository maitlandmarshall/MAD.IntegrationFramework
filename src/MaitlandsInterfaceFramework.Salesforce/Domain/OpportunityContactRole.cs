using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Salesforce.Domain
{
    public class OpportunityContactRole
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public string ContactId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsPrimary { get; set; }
        public string CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string OpportunityId { get; set; }
        public string SystemModstamp { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
