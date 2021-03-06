﻿using Autofac;
using MAD.IntegrationFramework.Configuration;
using MAD.IntegrationFramework.Database;
using MAD.IntegrationFramework.Integrations;
using MAD.IntegrationFramework.Tests.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Tests.Integrations
{
    [TestClass]
    public class TimedIntegrationScopeFactoryTests
    {
        private class TestDbContext : MIFDbContext
        {
            public TestDbContext(DbContextOptions options) : base(options)
            {
            }
        }

        private class TestIntegrationScopeMIFDbContextResolver : IIntegrationScopeMIFDbContextResolver
        {
            public IEnumerable<Type> ResolveTypes()
            {
                yield return typeof(TestDbContext);
            }
        }

        private class TestTimedIntegration : TimedIntegration
        {
            public TestDbContext DbContext { get; }

            public override TimeSpan Interval => throw new NotImplementedException();
            public override bool IsEnabled => throw new NotImplementedException();

            public TestTimedIntegration(TestDbContext dbContext)
            {
                this.DbContext = dbContext;
            }

            public override Task Execute()
            {
                throw new NotImplementedException();
            }
        }

        private ILifetimeScope GetIntegrationScope()
        {
            var scopeFactory = this.GetTimedIntegrationScopeFactory();
            var rootScope = new ContainerBuilder().Build();

            ILifetimeScope integrationScope = scopeFactory.Create(typeof(TestTimedIntegration), rootScope);
            return integrationScope;
        }

        private TimedIntegrationScopeFactory GetTimedIntegrationScopeFactory()
        {
            return new TimedIntegrationScopeFactory(
                integrationScopeMIFDbContextResolver: new TestIntegrationScopeMIFDbContextResolver(),
                mifDbContextFactory: new InMemoryMIFDbContextFactory(),
                config: new BasicMIFConfigFactory().Create(),
                logger: Log.Logger
            );
        }

        [TestMethod]
        public void Create_TimedIntegration_IsRegistered()
        {
            var scope = this.GetIntegrationScope();
            Assert.IsTrue(scope.IsRegistered<TestTimedIntegration>());
        }

        [TestMethod]
        public void Create_DbContext_IsRegistered()
        {
            var scope = this.GetIntegrationScope();
            Assert.IsTrue(scope.IsRegistered<TestDbContext>());
        }

        [TestMethod]
        public void Create_TimedIntegrationDbContextConstructorParam_IsResolved()
        {
            var scope = this.GetIntegrationScope();

            TestTimedIntegration testTimedIntegration = scope.Resolve<TestTimedIntegration>();

            Assert.IsNotNull(testTimedIntegration.DbContext);
        }

        [TestMethod]
        public void Create_TimedIntegrationDbContextConstructorParam_IsDisposedOnScopeDispose()
        {
            MIFDbContext dbContext; 

            using (var scope = this.GetIntegrationScope())
            {
                TestTimedIntegration testTimedIntegration = scope.Resolve<TestTimedIntegration>();
                dbContext = testTimedIntegration.DbContext;
            }

            Assert.ThrowsException<ObjectDisposedException>(() => dbContext.ChangeTracker);
        }


    }
}
