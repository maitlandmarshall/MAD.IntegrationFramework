using MaitlandsInterfaceFramework.Core.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework.Namely.Tests
{
    [TestClass]
    public class NamelyApiClientTests
    {
        class NamelyApiTestConfig : MIFConfig, INamelyConfig
        {
            public string NamelyClientName { get; set; }
            public string NamelyToken { get; set; }
        }

        string ReportGuid;

        [TestInitialize]
        public void Init()
        {
            NamelyApiTestConfig config = new NamelyApiTestConfig();

            Dictionary<string, string> apiKeys = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("NamelyApiKeys.txt"));
            config.NamelyClientName = apiKeys[nameof(config.NamelyClientName)];
            config.NamelyToken = apiKeys[nameof(config.NamelyToken)];

            this.ReportGuid = apiKeys["NamelyReportGuid"];

            MIF.SetConfigForTesting(config);
        }

        [TestMethod]
        public async Task GetReportTest()
        {
            var api = new NamelyApiClient();
            var reportResult = await api.GetReport(this.ReportGuid);
        }
    }
}
