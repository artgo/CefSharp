using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.Common.API;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UnitTests
{
    [TestFixture]
    public class StatusHelperUnitTest
    {
        [Test]
        public void ConvertToDisplayStatusReturnsCancelledForNullStatus()
        {
            Assert.AreEqual(DisplayStatus.Cancelled,
                            StatusHelper.ConvertToDisplayStatus(null, ApiSubscriptionStatus.ACTIVE));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsCancelledForNullSubscriptionStatus()
        {
            Assert.AreEqual(DisplayStatus.Cancelled,
                            StatusHelper.ConvertToDisplayStatus(ApiStatus.ACTIVE, null));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsCancelledForCancelledStatus()
        {
            Assert.AreEqual(DisplayStatus.Cancelled,
                            StatusHelper.ConvertToDisplayStatus(ApiStatus.CANCELLED, ApiSubscriptionStatus.ACTIVE));
        }


        [Test]
        public void ConvertToDisplayStatusReturnsCancelledForCancelledSubscriptionStatus()
        {
            Assert.AreEqual(DisplayStatus.Cancelled,
                            StatusHelper.ConvertToDisplayStatus(ApiStatus.ACTIVE, ApiSubscriptionStatus.CANCELLED));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsPendingAdditionForPendingStatus()
        {
            Assert.AreEqual(DisplayStatus.PendingAddition,
                            StatusHelper.ConvertToDisplayStatus(ApiStatus.PENDING_REMOTE_CREATION, ApiSubscriptionStatus.ACTIVE));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsPendingRemovalForPendingStatus()
        {
            Assert.AreEqual(DisplayStatus.PendingRemoval,
                            StatusHelper.ConvertToDisplayStatus(ApiStatus.PENDING_REMOTE_CANCELLATION, ApiSubscriptionStatus.ACTIVE));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsPendingRemovalForPendingSubscriptionStatus()
        {
            Assert.AreEqual(DisplayStatus.PendingRemoval,
                            StatusHelper.ConvertToDisplayStatus(ApiStatus.ACTIVE, ApiSubscriptionStatus.PENDING_REMOTE_CANCELLATION));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsActiveForActiveStatus()
        {
            Assert.AreEqual(DisplayStatus.Active,
                            StatusHelper.ConvertToDisplayStatus(ApiStatus.ACTIVE, ApiSubscriptionStatus.ACTIVE));
        }

        [Test]
        public void ConvertToDisplayStatusReturnsActiveForFreeTrialStatus()
        {
            Assert.AreEqual(DisplayStatus.Active,
                            StatusHelper.ConvertToDisplayStatus(ApiStatus.ACTIVE, ApiSubscriptionStatus.FREE_TRIAL));
        }
    }
}
