using MaitlandsInterfaceFramework.Core.Configuration;
using MaitlandsInterfaceFramework.Lever.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MaitlandsInterfaceFramework.Lever.Tests
{
    [TestClass]
    public class LeverApiClientTests
    {
        private class TestConfig : MIFConfig, ILeverConfig
        {
            public string LeverApiKey { get; set; }
        }

        [TestInitialize]
        public void Init()
        {
            string apiKey = File.ReadAllText("ApiKey.txt");

            MIF.SetConfigForTesting(new TestConfig
            {
                LeverApiKey = apiKey
            });
        }

        [TestMethod]
        public async Task TestRequisitionsAsync()
        {
            LeverApiClient client = new LeverApiClient();
            IEnumerable<Requisition> requisitions = await client.Requisitions();

            Assert.IsNotNull(requisitions);
            Assert.IsTrue(requisitions.Any());
        }

        [TestMethod]
        public async Task TestPostingsAsync()
        {
            LeverApiClient client = new LeverApiClient();
            IEnumerable<Posting> postings = await client.Postings();

            Assert.IsNotNull(postings);
            Assert.IsTrue(postings.Any());
        }

        [TestMethod]
        public async Task TestStagesAsync()
        {
            LeverApiClient client = new LeverApiClient();
            IEnumerable<Stage> stages = await client.Stages();

            Assert.IsNotNull(stages);
            Assert.IsTrue(stages.Any());
        }

        [TestMethod]
        public async Task TestOpportunitiesAsync()
        {
            LeverApiClient client = new LeverApiClient();
            IEnumerable<Opportunity> opportunities = await client.Opportunities();

            Assert.IsNotNull(opportunities);
            Assert.IsTrue(opportunities.Any());
        }

        [TestMethod]
        public async Task TestCandidatesAsync()
        {
            LeverApiClient client = new LeverApiClient();
            IEnumerable<Opportunity> candidates = await client.Candidates();

            Assert.IsNotNull(candidates);
            Assert.IsTrue(candidates.Any());
        }

        [TestMethod]
        public async Task TestCandidateOffersAsync()
        {
            LeverApiClient client = new LeverApiClient();
            IEnumerable<Opportunity> candidates = await client.Candidates(50);

            foreach (var c in candidates)
            {
                var cOffers = await client.OffersForCandidate(c.Id);

                if (cOffers.Any())
                    break;
            }
        }

        [TestMethod]
        public async Task TestCandidateReferralsAsync()
        {
            LeverApiClient client = new LeverApiClient();
            IEnumerable<Opportunity> candidates = await client.Candidates(50);

            foreach (var c in candidates)
            {
                var cReferrals = await client.ReferralsForCandidate(c.Id);

                if (cReferrals.Any())
                    break;
            }
        }

        [TestMethod]
        public async Task TestCandidateInterviewsAsync()
        {
            LeverApiClient client = new LeverApiClient();
            IEnumerable<Opportunity> candidates = await client.Candidates(50);

            foreach (var c in candidates)
            {
                var cInterviews = await client.InterviewsForCandidate(c.Id);

                if (cInterviews.Any())
                    break;
            }
        }

        [TestMethod]
        public async Task TestUsersAsync()
        {
            LeverApiClient client = new LeverApiClient();
            IEnumerable<User> users = await client.Users();

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Any());
        }
    }
}
