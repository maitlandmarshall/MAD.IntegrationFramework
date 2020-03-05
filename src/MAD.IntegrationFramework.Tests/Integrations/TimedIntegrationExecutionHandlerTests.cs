using MAD.IntegrationFramework.Database;
using MAD.IntegrationFramework.Integrations;
using MAD.IntegrationFramework.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Tests.Integrations
{
    [TestClass]
    public class TimedIntegrationExecutionHandlerTests
    {
        private class FiveSecondTimedIntegration : TimedIntegration
        {
            public override TimeSpan Interval => throw new NotImplementedException();
            public override bool IsEnabled => true;

            public override async Task Execute()
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
        private class ExceptionTimedIntegration : TimedIntegration
        {
            public override TimeSpan Interval => throw new NotImplementedException();
            public override bool IsEnabled => true;

            public override Task Execute()
            {
                throw new Exception();
            }
        }

        private TimedIntegrationExecutionHandler GetExecutionHandler(InMemoryMIFDbContextFactory<TimedIntegrationLogDbContext> dbContextFactory = null)
        {
            return new TimedIntegrationExecutionHandler(
                timedIntegrationLogDbContextFactory: dbContextFactory ?? new InMemoryMIFDbContextFactory<TimedIntegrationLogDbContext>(),
                timedIntegrationMetaDataService: new NullIntegrationMetaDataService()
            );
        }

        [TestMethod]
        public async Task Execute_Logging_CreatesWithStartDateBeforeCompletion()
        {
            var dbContextFactory = new InMemoryMIFDbContextFactory<TimedIntegrationLogDbContext>();
            var executionHandler = this.GetExecutionHandler(dbContextFactory);

            FiveSecondTimedIntegration fiveSecond = new FiveSecondTimedIntegration();
            Task execute = executionHandler.Execute(fiveSecond, new TimedIntegrationTimer(typeof(FiveSecondTimedIntegration)));

            using (var dbContext = dbContextFactory.Create())
            {
                TimedIntegrationLog log = dbContext.TimedIntegrationLogs.LastOrDefault();

                Assert.IsNotNull(log);
                Assert.IsNotNull(log.StartDateTime);
                Assert.IsNull(log.EndDateTime);
            }

            await execute;
        }

        [TestMethod]
        public async Task Execute_Logging_CreatesLogWithStartAndEndDates()
        {
            var dbContextFactory = new InMemoryMIFDbContextFactory<TimedIntegrationLogDbContext>();
            var executionHandler = this.GetExecutionHandler(dbContextFactory);

            FiveSecondTimedIntegration fiveSecond = new FiveSecondTimedIntegration();
            await executionHandler.Execute(fiveSecond, new TimedIntegrationTimer(typeof(FiveSecondTimedIntegration)));

            using (var dbContext = dbContextFactory.Create())
            {
                TimedIntegrationLog log = dbContext.TimedIntegrationLogs.LastOrDefault();

                Assert.IsNotNull(log.EndDateTime);
            }
        }

        [TestMethod]
        public async Task Execute_Logging_CreatesLogWithEndDateOnException()
        {
            var dbContextFactory = new InMemoryMIFDbContextFactory<TimedIntegrationLogDbContext>();
            var executionHandler = this.GetExecutionHandler(dbContextFactory);

            ExceptionTimedIntegration exception = new ExceptionTimedIntegration();

            await Assert.ThrowsExceptionAsync<Exception>(() => executionHandler.Execute(exception, new TimedIntegrationTimer(typeof(FiveSecondTimedIntegration))));

            using (var dbContext = dbContextFactory.Create())
            {
                TimedIntegrationLog log = dbContext.TimedIntegrationLogs.LastOrDefault();

                Assert.IsNotNull(log);
                Assert.IsNotNull(log.StartDateTime);
                Assert.IsNotNull(log.EndDateTime);
            }
        }
    }
}
