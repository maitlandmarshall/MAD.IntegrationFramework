using MAD.IntegrationFramework.Core.Configuration;
using MAD.IntegrationFramework.Pardot.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Pardot.Tests
{
    [TestClass]
    public class PardotApiClientTests
    {
        public class TestConfig : MIFConfig, IPardotConfig
        {
            public string PardotEmail { get; set; }
            public string PardotPassword { get; set; }
            public string PardotUserKey { get; set; }

            public TestConfig()
            {
                string keys = File.ReadAllText("PardotApiKeys.txt");
                Dictionary<string, object> keysObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(keys);

                this.PardotEmail = keysObj[nameof(PardotEmail)] as string;
                this.PardotPassword = keysObj[nameof(PardotPassword)] as string;
                this.PardotUserKey = keysObj[nameof(PardotUserKey)] as string;
            }
        }

        private static PardotApiClient GetClient()
        {
            return new PardotApiClient();
        }

        [TestInitialize]
        public void Init()
        {
            MIF.SetConfigForTesting(new TestConfig());
        }

        [TestMethod]
        public async Task LoginAndGetApiKeyTest()
        {
            PardotApiClient client = new PardotApiClient();
            await client.LoginAndGetApiKey();
        }

        [TestMethod]
        public async Task GetAccountTest()
        {
            PardotApiClient client = GetClient();

            Account account = await client.GetAccount();

            Assert.IsNotNull(account);
        }


        #region BULK QUERY TESTS WITHOUT DATE PARAMS

        [TestMethod]
        public async Task GetCampaignsTest()
        {
            var campaigns = await this.BulkQueryTest<Campaign>();

            Assert.IsNotNull(campaigns);
        }

        [TestMethod]
        public async Task GetCustomRedirectsTest()
        {
            var customRedirects = await this.BulkQueryTest<CustomRedirect>();

            Assert.IsNotNull(customRedirects);
        }

        [TestMethod]
        public async Task GetEmailClickTest()
        {
            var emailClicks = await this.BulkQueryTest<EmailClick>();

            Assert.IsNotNull(emailClicks);
        }

        [TestMethod]
        public async Task GetOpportunitiesTest()
        {
            var opportunitites = await this.BulkQueryTest<Opportunity>();

            Assert.IsNotNull(opportunitites);
        }

        [TestMethod]
        public async Task GetProspectsTest()
        {
            var prospects = await this.BulkQueryTest<Prospect>();

            Assert.IsNotNull(prospects);
        }

        [TestMethod]
        public async Task GetProspectAccountsTest()
        {
            var prospectAccounts = await this.BulkQueryTest<ProspectAccount>();

            Assert.IsNotNull(prospectAccounts);
        }

        [TestMethod]
        public async Task GetUsersTest()
        {
            var users = await this.BulkQueryTest<User>();

            Assert.IsNotNull(users);
        }

        [TestMethod]
        public async Task GetVisitorsTest()
        {
            var visitors = await this.BulkQueryTest<Visitor>();

            Assert.IsNotNull(visitors);
        }

        #endregion

        #region BULK QUERY TESTS WITH DATE PARAMS

        [TestMethod]
        public async Task GetCampaignsWithDateTest()
        {
            var campaigns = await this.BulkQueryTest<Campaign>(true);

            Assert.IsNotNull(campaigns);
        }

        [TestMethod]
        public async Task GetCustomRedirectsWithDateTest()
        {
            var customRedirects = await this.BulkQueryTest<CustomRedirect>(true);

            Assert.IsNotNull(customRedirects);
        }

        [TestMethod]
        public async Task GetEmailClickWithDateTest()
        {
            var emailClicks = await this.BulkQueryTest<EmailClick>(true);

            Assert.IsNotNull(emailClicks);
        }

        [TestMethod]
        public async Task GetOpportunitiesWithDateTest()
        {
            var opportunitites = await this.BulkQueryTest<Opportunity>(true);

            Assert.IsNotNull(opportunitites);
        }

        [TestMethod]
        public async Task GetProspectsWithDateTest()
        {
            var prospects = await this.BulkQueryTest<Prospect>(true);

            Assert.IsNotNull(prospects);
        }

        [TestMethod]
        public async Task GetProspectAccountsWithDateTest()
        {
            var prospectAccounts = await this.BulkQueryTest<ProspectAccount>(true);

            Assert.IsNotNull(prospectAccounts);
        }

        [TestMethod]
        public async Task GetUsersWithDateTest()
        {
            var users = await this.BulkQueryTest<User>(true);

            Assert.IsNotNull(users);
        }

        [TestMethod]
        public async Task GetVisitorsWithDateTest()
        {
            var visitors = await this.BulkQueryTest<Visitor>(true);

            Assert.IsNotNull(visitors);
        }

        [TestMethod]
        public void TestFailResponse()
        {
            string failResponseJson = "{\"@attributes\":{\"stat\":\"fail\",\"version\":1,\"err_code\":1},\"err\":\"Invalid API key or user key\"}";
            string successResponseJson = "{\"@attributes\":{\"stat\":\"ok\",\"version\":1}}";

            JObject failResponse = JsonConvert.DeserializeObject<JObject>(failResponseJson);
            JObject successResponse = JsonConvert.DeserializeObject<JObject>(successResponseJson);

            ParseResponse(failResponse);
            ParseResponse(successResponse);
        }

        private void ParseResponse(JObject successOrFailResponse)
        {
            var errCode = successOrFailResponse["@attributes"]["err_code"];

            if (errCode == null)
                return;

            int errorCode = errCode.Value<int>();

            Assert.IsTrue(errorCode > 0);
        }

        #endregion


        private async Task<IEnumerable<ResponseType>> BulkQueryTest<ResponseType>(bool isDateTest = false) where ResponseType : IEntity
        {
            var client = GetClient();

            if (isDateTest)
            {
                return await client.PerformBulkQuery<ResponseType>(new Api.BulkQueryParameters
                {
                    CreatedAfter = DateTime.Now.AddDays(-1),
                    UpdatedAfter = DateTime.Now.AddDays(-1)
                });
            }
            else
            {
                var result = await client.PerformBulkQuery<ResponseType>(new Api.BulkQueryParameters { Take = 600 });
                Assert.IsTrue(result.Count() <= 600);

                return result;
            }

            
        }
    }
}
