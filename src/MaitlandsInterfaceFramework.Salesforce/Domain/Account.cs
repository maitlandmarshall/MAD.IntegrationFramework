using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Salesforce.Domain
{
    public class Account
    {
        public string Id { get; set; }
        public string Fax { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Phone { get; set; }
        public string Jigsaw { get; set; }
        public string OwnerId { get; set; }
        public string SicDesc { get; set; }
        public string Website { get; set; }
        public string Industry { get; set; }
        public string ParentId { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsDeleted { get; set; }
        public string BillingCity { get; set; }
        public string CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Description { get; set; }
        public string BillingState { get; set; }
        public string ShippingCity { get; set; }
        public string AccountSource { get; set; }
        public decimal? AnnualRevenue { get; set; }
        public string BillingStreet { get; set; }
        public string ShippingState { get; set; }
        public Address BillingAddress { get; set; }
        public string BillingCountry { get; set; }
        public DateTime? LastViewedDate { get; set; }
        public string MasterRecordId { get; set; }
        public string ShippingStreet { get; set; }
        public string SystemModstamp { get; set; }
        public decimal? BillingLatitude { get; set; }
        public string JigsawCompanyId { get; set; }
        public Address ShippingAddress { get; set; }
        public string ShippingCountry { get; set; }
        public decimal? BillingLongitude { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public decimal? ShippingLatitude { get; set; }
        public string BillingPostalCode { get; set; }
        public int? NumberOfEmployees { get; set; }
        public decimal? ShippingLongitude { get; set; }
        public DateTime? LastReferencedDate { get; set; }
        public string ShippingPostalCode { get; set; }
        public string BillingGeocodeAccuracy { get; set; }
        public string ShippingGeocodeAccuracy { get; set; }
    }
}
