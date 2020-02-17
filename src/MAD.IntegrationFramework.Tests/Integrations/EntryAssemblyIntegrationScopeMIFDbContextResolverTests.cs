using MAD.IntegrationFramework.Database;
using MAD.IntegrationFramework.Integrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAD.IntegrationFramework.UnitTests.Integrations
{
    [TestClass]
    public class EntryAssemblyIntegrationScopeMIFDbContextResolverTests
    {
        private class T1 : MIFDbContext { }
        private class T2 : MIFDbContext { }
        internal class T3 : MIFDbContext { }
        public class T4 : MIFDbContext { }

        private ICollection GetResolvedCollection()
        {
            return new EntryAssemblyIntegrationScopeMIFDbContextResolver().ResolveTypes(typeof(T1).Assembly).ToList();
        }

        [TestMethod]
        public void ResolveTypes_FindsPrivates_ShouldFindT1AndT2()
        {
            var types = this.GetResolvedCollection();

            CollectionAssert.Contains(types, typeof(T1));
            CollectionAssert.Contains(types, typeof(T2));
        }

        [TestMethod]
        public void ResolveTypes_FindsInternals_ShouldFindT3()
        {
            CollectionAssert.Contains(this.GetResolvedCollection(), typeof(T3));
        }

        [TestMethod]
        public void ResolveTypes_FindsPublics_ShouldFindT4()
        {
            CollectionAssert.Contains(this.GetResolvedCollection(), typeof(T4));
        }
    }
}
