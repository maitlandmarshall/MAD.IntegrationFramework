using MAD.IntegrationFramework.Integrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.UnitTests.Integrations
{
    [TestClass]
    public class EntryAssemblyIntegrationResolverTests
    {
        private class T1 : TimedIntegration
        {
            public override TimeSpan Interval => throw new NotImplementedException();
            public override bool IsEnabled => throw new NotImplementedException();
            public override Task Execute()
            {
                throw new NotImplementedException();
            }
        }
        private class T2 : TimedIntegration
        {
            public override TimeSpan Interval => throw new NotImplementedException();
            public override bool IsEnabled => throw new NotImplementedException();
            public override Task Execute()
            {
                throw new NotImplementedException();
            }
        }

        internal class T3 : TimedIntegration
        {
            public override TimeSpan Interval => throw new NotImplementedException();

            public override bool IsEnabled => throw new NotImplementedException();

            public override Task Execute()
            {
                throw new NotImplementedException();
            }
        }

        public class T4 : TimedIntegration
        {
            public override TimeSpan Interval => throw new NotImplementedException();

            public override bool IsEnabled => throw new NotImplementedException();

            public override Task Execute()
            {
                throw new NotImplementedException();
            }
        }

        private ICollection GetResolvedCollection()
        {
            return new EntryAssemblyIntegrationResolver().ResolveTypes(typeof(T1).Assembly).ToList();
        }

        [TestMethod]
        public void ResolvesMany()
        {
            Assert.IsTrue(this.GetResolvedCollection().Count > 1);
        }

        [TestMethod]
        public void ResolvesPrivates()
        {
            var types = this.GetResolvedCollection();

            CollectionAssert.Contains(types, typeof(T1));
            CollectionAssert.Contains(types, typeof(T2));
        }

        [TestMethod]
        public void ResolvesInternals()
        {
            CollectionAssert.Contains(this.GetResolvedCollection(), typeof(T3));
        }

        [TestMethod]
        public void ResolvesPublics()
        {
            CollectionAssert.Contains(this.GetResolvedCollection(), typeof(T4));
        }
    }
}
