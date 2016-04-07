using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using ScreamingFleas.SelfHealingWcfClient;

namespace ScreamingFleas.WcfClient.Tests
{
    [TestClass]
    public class ClientTests
    {
        private ServiceHost _serviceHost;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new UriBuilder();
            builder.Host = "localhost";
            builder.Port = 7584;
            builder.Scheme = "http";
            builder.Path = "SampleWcfService.svc";

            _serviceHost = new ServiceHost(typeof(SampleWcfService), builder.Uri);
            _serviceHost.Open();
        }

        [TestCleanup] 
        public void Cleanup()
        {
            _serviceHost.Close();
            _serviceHost = null;
        }

        [TestMethod]
        public void SanityCheck()
        {
            var client = SelfHealingChannel<ISampleWcfService>.Create();

            var result = client.GetData(1);

            Assert.AreEqual("You entered: 1", result);
        }

        [TestMethod]
        public void TestRecoveryWhenServiceCrashes()
        {
            var client = SelfHealingChannel<ISampleWcfService>.Create();

            _serviceHost.Close();
            try
            {
                client.GetData(1);
                Assert.Fail();
            } catch
            {
                Initialize();
            }

            var result = client.GetData(1);

            Assert.AreEqual("You entered: 1", result);
        }

        [TestMethod]
        public void TestRecovery()
        {
            var client = SelfHealingChannel<ISampleWcfService>.Create();

            var result = client.GetData(1);

            Assert.AreEqual("You entered: 1", result);

            try {
                client.ThrowException();
                Assert.Fail();
            } catch
            {
                // exception here. 
            }

            result = client.GetData(1);

            Assert.AreEqual("You entered: 1", result);
        }
    }
}
