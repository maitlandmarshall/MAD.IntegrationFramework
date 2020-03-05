using MAD.IntegrationFramework.Database;
using MAD.IntegrationFramework.Integrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MAD.IntegrationFramework.Tests.Integrations
{
    [TestClass]
    public class EntryAssemblyIntegrationScopeMIFDbContextResolverTests
    {
        private class T1 : MIFDbContext
        {
            public T1(DbContextOptions options) : base(options)
            {
            }
        }
        private class T2 : MIFDbContext
        {
            public T2(DbContextOptions options) : base(options)
            {
            }
        }
        internal class T3 : MIFDbContext
        {
            public T3(DbContextOptions options) : base(options)
            {
            }
        }
        public class T4 : MIFDbContext
        {
            public T4(DbContextOptions options) : base(options)
            {
            }
        }

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
