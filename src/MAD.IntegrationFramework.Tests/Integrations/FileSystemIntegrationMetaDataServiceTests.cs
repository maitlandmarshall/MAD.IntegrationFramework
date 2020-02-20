using MAD.IntegrationFramework.Integrations;
using MAD.IntegrationFramework.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MAD.IntegrationFramework.Tests.Integrations
{
    [TestClass]
    public class FileSystemIntegrationMetaDataServiceTests
    {
        class TestFilePathResolver : IRelativeFilePathResolver
        {
            public string ResolvePath(string relativeFilePath)
            {
                return Path.Combine(Directory.GetCurrentDirectory(), relativeFilePath);
            }
        }

        private FileSystemIntegrationMetaDataService GetService()
        {
            return new FileSystemIntegrationMetaDataService(
                integrationPathResolver: new TestFilePathResolver()
            );
        }

        private class TestTimedIntegration : TimedIntegration
        {
            public override TimeSpan Interval => throw new NotImplementedException();
            public override bool IsEnabled => throw new NotImplementedException();

            [Savable]
            public DateTime? TestDateTime { get; set; }

            public override Task Execute()
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void Save_SavableAttributeFields_CreatesSaveFile()
        {
            FileSystemIntegrationMetaDataService fileSystemIntegrationMetaDataService = this.GetService();

            TestTimedIntegration testIntegration = new TestTimedIntegration
            {
                TestDateTime = new DateTime(2020, 1, 1)
            };

            fileSystemIntegrationMetaDataService.Save(testIntegration);

            string savePath = fileSystemIntegrationMetaDataService.GetMetaDataFilePath(testIntegration);

            Assert.IsTrue(File.Exists(savePath));
        }

        [TestMethod]
        public void Load_SavableAttributeFields_DateTimesAreEqual()
        {
            FileSystemIntegrationMetaDataService fileSystemIntegrationMetaDataService = this.GetService();

            TestTimedIntegration testIntegration = new TestTimedIntegration
            {
                TestDateTime = new DateTime(2020, 2, 1)
            };

            fileSystemIntegrationMetaDataService.Save(testIntegration);

            TestTimedIntegration testLoadIntegration = new TestTimedIntegration();
            fileSystemIntegrationMetaDataService.Load(testLoadIntegration);

            Assert.AreEqual(testIntegration.TestDateTime, testLoadIntegration.TestDateTime);
        }
    }
}
