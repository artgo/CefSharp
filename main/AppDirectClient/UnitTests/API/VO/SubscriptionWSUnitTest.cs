using AppDirect.WindowsClient.API.VO;
using NUnit.Framework;
using System.IO;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.Tests.API.VO
{
    [TestFixture]
    public class SubscriptionWSUnitTest
    {
        private volatile XmlSerializer _xmlSerializer;
        private volatile SubscriptionWS _subscriptionWs;
        private volatile string _serializedXml;

        [SetUp]
        public void Init()
        {
            _xmlSerializer = new XmlSerializer(typeof(SubscriptionWS));

            _subscriptionWs = new SubscriptionWS();
            var user = new UserWS() { id = "1" };
            var company = new CompanyWS() { id = "2" };
            _subscriptionWs.paymentPlanId = "0";
            _subscriptionWs.user = user;
            _subscriptionWs.company = company;

            _serializedXml = SerializeToString(_subscriptionWs);
        }

        private string SerializeToString(SubscriptionWS sub)
        {
            var textWriter = new StringWriter();
            _xmlSerializer.Serialize(textWriter, sub);
            var xml = textWriter.ToString();
            return xml;
        }

        [Test]
        public void SunscriptionWSSerializesToNotNull()
        {
            Assert.IsNotNull(_serializedXml);
        }

        [Test]
        public void SunscriptionWSSerializesToXml()
        {
            Assert.IsTrue(_serializedXml.StartsWith("<?xml"));
        }

        [Test]
        public void SunscriptionWSSerializesRoot()
        {
            Assert.IsTrue(_serializedXml.Contains("<subscription"));
        }

        [Test]
        public void SunscriptionWSSerializesPaymentPlanId()
        {
            Assert.IsTrue(_serializedXml.Contains("<paymentPlanId>0</paymentPlanId>"));
        }

        [Test]
        public void SunscriptionWSSerializesUserId()
        {
            Assert.IsTrue(_serializedXml.Contains("<id>1</id>"));
        }

        [Test]
        public void SunscriptionWSSerializesCompanyId()
        {
            Assert.IsTrue(_serializedXml.Contains("<id>2</id>"));
        }
    }
}