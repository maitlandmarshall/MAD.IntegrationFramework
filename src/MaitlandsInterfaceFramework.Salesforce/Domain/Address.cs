using System;
using System.Collections.Generic;
using System.Text;

namespace MaitlandsInterfaceFramework.Salesforce.Domain
{
    public class Address
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string GeocodeAccuracy { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string Street { get; set; }
    }
}
