using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAD.IntegrationFramework.Pardot.Domain
{
    public class Prospect : IMutableEntity
    {
        public int Id { get; set; }

        public Campaign Campaign { get; set; }

        public string Salutation { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Company { get; set; }

        [JsonProperty("prospect_account_id")]
        public int? ProspectAccountId { get; set; }

        public string Website { get; set; }

        [JsonProperty("job_title")]
        public string JobTitle { get; set; }

        public string Department { get; set; }
        public string Country { get; set; }
        
        [JsonProperty("address_one")]
        public string AddressOne { get; set; }

        [JsonProperty("address_two")]
        public string AddressTwo { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Territory { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Source { get; set; }
        
        [JsonProperty("annual_revenue")]
        public string AnnualRevenue { get; set; }

        public string Employees { get; set; }
        public string Industry { get; set; }

        [JsonProperty("years_in_business")]
        public string YearsInBusiness { get; set; }

        public string Comments { get; set; }
        public string Notes { get; set; }

        public int Score { get; set; }
        public string Grade { get; set; }

        [JsonProperty("last_activity_at")]
        public DateTime? LastActivityAt { get; set; }

        [JsonProperty("recent_interaction")]
        public string RecentInteraction { get; set; }

        [JsonProperty("crm_lead_fid")]
        public string CrmLeadFid { get; set; }

        [JsonProperty("crm_contact_fid")]
        public string CrmContactFid { get; set; }

        [JsonProperty("crm_owner_fid")]
        public string CrmOwnerFid { get; set; }

        [JsonProperty("crm_account_fid")]
        public string CrmAccountFid { get; set; }

        [JsonProperty("crm_last_sync")]
        public DateTime? CrmLastSync { get; set; }

        [JsonProperty("crm_url")]
        public string CrmUrl { get; set; }

        [JsonProperty("is_do_not_email")]
        public bool? IsDoNotEmail { get; set; }

        [JsonProperty("is_do_not_call")]
        public bool? IsDoNotCall { get; set; }

        [JsonProperty("opted_out")]
        public bool? OptedOut { get; set; }

        [JsonProperty("is_reviewed")]
        public bool? IsReviewed { get; set; }

        [JsonProperty("is_starred")]
        public bool? IsStarred { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
