using MaitlandsInterfaceFramework.Core.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework.Salesforce.Tests
{
    [TestClass]
    public class SalesforceApiFactoryTests
    {
        public class TestConfig : MIFConfig, ISalesforceConfig
        {
            public string SalesforceConsumerKey { get; set; }
            public string SalesforceConsumerSecret { get; set; }
            public string SalesforceAccessToken { get; set; }
            public string SalesforceOAuthCallbackUrl { get; set; } = "http://localhost:666/api/salesforceOAuth/callback";
            public string SalesforceRefreshToken { get; set; }
        }

        public class Account
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        [TestInitialize]
        public void Initialize()
        {
            TestConfig config = JsonConvert.DeserializeObject<TestConfig>(File.ReadAllText("ApiKey.txt"));

            MIF.SetConfigForTesting(config);
        }

        [TestMethod]
        public async Task GetApiClientIsAlreadyAuthenticated()
        {
            SalesforceApiFactory factory = new SalesforceApiFactory();
            var apiClient = await factory.GetApiClient();

            var accountResult = await apiClient.QueryAsync<Account>("SELECT Id, Name, Description FROM Account");

            Assert.IsTrue(accountResult.Records.Any());
        }
    }
}
