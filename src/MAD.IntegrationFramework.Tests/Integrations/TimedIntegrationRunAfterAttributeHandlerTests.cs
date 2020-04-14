using MAD.IntegrationFramework;
using MAD.IntegrationFramework.Integrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Tests.Integrations
{
    [TestClass]
    public class TimedIntegrationRunAfterAttributeHandlerTests
    {
        private class BaseIntegration : TimedIntegration
        {
            public override TimeSpan Interval => TimeSpan.FromSeconds(1);
            public override bool IsEnabled => true;

            public override Task Execute()
            {
                throw new NotImplementedException();
            }
        }

        private class RunsFirst : BaseIntegration
        {   
        }

        [RunAfter(typeof(RunsFirst))]
        private class RunsSeconds : BaseIntegration { }

        [RunAfter(typeof(RunsSeconds))]
        private class RunsThird : BaseIntegration { }

        [TestMethod]
        public void WaitForOthers_HierarchialRunAfter_RunsInCorrectOrder()
        {
            var handler = new TimedIntegrationRunAfterAttributeHandler();

            IEnumerable<TimedIntegrationTimer> integrations = new List<TimedIntegrationTimer>
            {
                new TimedIntegrationTimer(typeof(RunsFirst)),
                new TimedIntegrationTimer(typeof(RunsSeconds)),
                new TimedIntegrationTimer(typeof(RunsThird))
            };

            var third = integrations.ElementAt(2);
            third.LastStart = DateTime.Now;

            var second = integrations.ElementAt(1);
            second.LastStart = DateTime.Now;

            var first = integrations.ElementAt(0);
            first.LastStart = DateTime.Now;

            var thirdTask = handler.WaitForOthers(third, integrations);
            var secondTask = handler.WaitForOthers(second, integrations);
            var firstTask = handler.WaitForOthers(first, integrations);

            // The first task should complete instantly
            Assert.IsTrue(firstTask.Wait(TimeSpan.FromSeconds(1)));

            // The remaining tasks should still be incomplete
            Assert.IsFalse(secondTask.IsCompleted);
            Assert.IsFalse(thirdTask.IsCompleted);

            first.LastFinish = DateTime.Now;

            // The second task should complete instantly
            Assert.IsTrue(secondTask.Wait(TimeSpan.FromSeconds(1)));

            // The third task should still be waiting
            Assert.IsFalse(thirdTask.IsCompleted);

            second.LastFinish = DateTime.Now;

            // The third task should complete instantly
            Assert.IsTrue(thirdTask.Wait(TimeSpan.FromSeconds(1)));
        }
    }
}
