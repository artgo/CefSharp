using AppDirect.WindowsClient.API.Subscription;
using AppDirect.WindowsClient.Common.API;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.API.Subscription
{
    [TestFixture]
    public class StatusHelperUnitTest
    {
        [Test]
        public void ConvertToDisplayStatusReturnsCancelledForNullStatus()
        {
            Assert.AreEqual(DisplayStatus.Cancelled,
                            StatusHelper.ConvertToDisplayStatus(null, "ACTIVE"));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsCancelledForNullSubscriptionStatus()
        {
            Assert.AreEqual(DisplayStatus.Cancelled,
                            StatusHelper.ConvertToDisplayStatus("ACTIVE", null));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsCancelledForCancelledStatus()
        {
            Assert.AreEqual(DisplayStatus.Cancelled,
                            StatusHelper.ConvertToDisplayStatus("CANCELLED", "ACTIVE"));
        }


        [Test]
        public void ConvertToDisplayStatusReturnsCancelledForCancelledSubscriptionStatus()
        {
            Assert.AreEqual(DisplayStatus.Cancelled,
                            StatusHelper.ConvertToDisplayStatus("ACTIVE", "CANCELLED"));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsPendingAdditionForPendingStatus()
        {
            Assert.AreEqual(DisplayStatus.UserPendingAddition,
                            StatusHelper.ConvertToDisplayStatus("PENDING_REMOTE_CREATION", "ACTIVE"));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsPendingRemovalForPendingStatus()
        {
            Assert.AreEqual(DisplayStatus.UserPendingRemoval,
                            StatusHelper.ConvertToDisplayStatus("PENDING_REMOTE_CANCELLATION", "ACTIVE"));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsCancelledForPendingSubscriptionStatus()
        {
            Assert.AreEqual(DisplayStatus.Cancelled,
                            StatusHelper.ConvertToDisplayStatus("ACTIVE", "PENDING_REMOTE_CANCELLATION"));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsActiveForActiveStatus()
        {
            Assert.AreEqual(DisplayStatus.Active,
                            StatusHelper.ConvertToDisplayStatus("ACTIVE", "ACTIVE"));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsActiveForFreeTrialStatus()
        {
            Assert.AreEqual(DisplayStatus.Active,
                            StatusHelper.ConvertToDisplayStatus("ACTIVE", "FREE_TRIAL"));
        }
    }
}
