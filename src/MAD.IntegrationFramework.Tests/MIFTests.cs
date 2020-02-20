using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Tests
{
    [TestClass]
    public class MIFTests
    {
        [TestMethod]
        public async Task StartStopTest()
        {
            await MIF.Start();
            await MIF.Stop();
        }
    }
}
