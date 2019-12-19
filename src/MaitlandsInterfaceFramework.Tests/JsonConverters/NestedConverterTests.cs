using MaitlandsInterfaceFramework.Core.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MaitlandsInterfaceFramework.Tests.JsonConverters
{
    [TestClass]
    public class NestedConverterTests
    {
        private class NestedExample1Model
        {
            public int Id { get; set; }

            [JsonProperty("annual_spend.value")]
            public int? AnnualSpend { get; set; }

            [JsonProperty("annual_revenue.value")]
            public int? AnnualRevenue { get; set; }

            [JsonProperty("billing_address_one.value")]
            public string BillingAddressOne { get; set; }

            [JsonProperty("billing_address_two.value")]
            public string BillingAddressTwo { get; set; }

            [JsonProperty("billing_city.value")]
            public string BillingCity { get; set; }

            [JsonProperty("billing_country.value")]
            public string BillingCountry { get; set; }

            [JsonProperty("billing_state.value")]
            public string BillingState { get; set; }

            [JsonProperty("billing_zip.value")]
            public string BillingZip { get; set; }

            [JsonProperty("description.value")]
            public string Description { get; set; }

            [JsonProperty("employees.value")]
            public int? Employees { get; set; }

            [JsonProperty("fax.value")]
            public string Fax { get; set; }

            [JsonProperty("Fortune_1000.value")]
            public bool Fortune1000 { get; set; }

            [JsonProperty("GICS_Industry.value")]
            public string GICSIndustry { get; set; }

            [JsonProperty("GISC_Industry_Group.value")]
            public string GISCIndustryGroup { get; set; }

            [JsonProperty("GISC_Sector.value")]
            public string GISCSector { get; set; }

            [JsonProperty("GISC_Sub_Industry.value")]
            public string GISCSubIndustry { get; set; }

            [JsonProperty("industry.value")]
            public string Value { get; set; }

            [JsonProperty("name.value")]
            public string Name { get; set; }

            [JsonProperty("number.value")]
            public int? Number { get; set; }

            [JsonProperty("Number_of_Employees.value")]
            public int? NumberOfEmployees { get; set; }

            [JsonProperty("ownership.value")]
            public string Ownership { get; set; }

            [JsonProperty("phone.value")]
            public string Phone { get; set; }

            [JsonProperty("rating.value")]
            public string Rating { get; set; }

            [JsonProperty("shipping_address_one.value")]
            public string ShippingAddressOne { get; set; }

            [JsonProperty("shipping_address_two.value")]
            public string ShippingAddressTwo { get; set; }

            [JsonProperty("shipping_city.value")]
            public string ShippingCity { get; set; }

            [JsonProperty("shipping_country.value")]
            public string ShippingCountry { get; set; }

            [JsonProperty("shipping_state.value")]
            public string ShippingState { get; set; }

            [JsonProperty("shipping_zip.value")]
            public string ShippingZip { get; set; }

            [JsonProperty("sic.value")]
            public int Sic { get; set; }

            [JsonProperty("type.value")]
            public string Type { get; set; }

            [JsonProperty("Unispace_Region.value")]
            public string UnispaceRegion { get; set; }

            [JsonProperty("website.value")]
            public string Website { get; set; }

            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        [JsonClass("result.prospectAccount")]
        private class NestedExample1ListModel : List<NestedExample1Model> { }

        private string NestedExample1Json;
        
        [TestInitialize]
        public void Init()
        {
            this.NestedExample1Json = File.ReadAllText("JsonConverters\\NestedJsonExample1.js");
        }

        [TestMethod]
        public void TestNestedExampleWithList()
        {
            NestedExample1ListModel result = JsonConvert.DeserializeObject<NestedExample1ListModel>(this.NestedExample1Json, new NestedJsonConverter());

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
    }
}
