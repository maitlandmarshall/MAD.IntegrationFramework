using MaitlandsInterfaceFramework.Core.Configuration;
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
    }
}
