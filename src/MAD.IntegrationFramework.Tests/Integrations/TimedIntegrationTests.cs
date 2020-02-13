using MAD.IntegrationFramework.Integrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.UnitTests.Integrations
{
    [TestClass]
    public class TimedIntegrationTests
    {
        public class TestIntegration : TimedIntegration
        {
            public override TimeSpan Interval => TimeSpan.FromSeconds(5);
            public override bool IsEnabled => true;

            public override Task Execute()
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public async Task TimedIntegrationExecuteOccursForInterval()
        {
            
        }

    }
}
