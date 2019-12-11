using MaitlandsInterfaceFramework.Core.Configuration;
using MaitlandsInterfaceFramework.Pardot.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework.Pardot.Tests
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



        private async Task<IEnumerable<ResponseType>> BulkQueryTest<ResponseType>()
        {
            var client = GetClient();
            return await client.PerformBulkQuery<ResponseType>();
        }
    }
}
