using System.Text.RegularExpressions;
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
        public void SubscriptionWSSerializesToNotNull()
        {
            Assert.IsNotNull(_serializedXml);
        }

        [Test]
        public void SubscriptionWSSerializesToXml()
        {
            Assert.IsTrue(_serializedXml.StartsWith("<?xml"));
        }

        [Test]
        public void SubscriptionWSSerializesRoot()
        {
            Assert.IsTrue(_serializedXml.Contains("<subscription"));
        }

        [Test]
        public void SubscriptionWSSerializesPaymentPlanId()
        {
            Assert.IsTrue(_serializedXml.Contains("<paymentPlanId>0</paymentPlanId>"));
        }

        [Test]
        public void SubscriptionWSSerializesUserId()
        {
            Assert.IsTrue(_serializedXml.Contains("<id>1</id>"));
        }

        [Test]
        public void SubscriptionWSSerializesCompanyId()
        {
            Assert.IsTrue(_serializedXml.Contains("<id>2</id>"));
        }

        [Test]
        public void SubscriptionWSSerializesOrder()
        {
            Assert.IsTrue(Regex.IsMatch(_serializedXml, @".*<id>2</id>.*<id>1</id>.*<paymentPlanId>0</paymentPlanId>.*", RegexOptions.Singleline));
        }

        [Test]
        public void SubscriptionWSSerializesCompanyIdFirst()
        {
            Assert.IsTrue(Regex.IsMatch(_serializedXml, @".*<company>\s*<id>2</id>.*", RegexOptions.Singleline));
        }


        [Test]
        public void SubscriptionWSSerializesUserIdFirst()
        {
            Assert.IsTrue(Regex.IsMatch(_serializedXml, @".*<user>\s*<id>1</id>.*", RegexOptions.Singleline));
        }


    }
}