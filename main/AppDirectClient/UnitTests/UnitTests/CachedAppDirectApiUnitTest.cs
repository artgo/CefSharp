using System.Web.Script.Serialization;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.API.VO;
using AppDirect.WindowsClient.Common.Log;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UnitTests
{
    [TestFixture]
    public class CachedAppDirectApiUnitTest
    {
        private volatile IAppDirectApi _appDirectApiMock;
        private volatile CachedAppDirectApi _cachedAppDirectApi;
        private volatile ILogger _log;
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private string _testProductXml = "{\"id\":2796,\"name\":\"[7.02.12] Hercules\",\"type\":\"WEB_APP\",\"provider\":{\"uuid\":\"892b38c2-18ae-4318-9753-1718213b71c1\",\"name\":\"Shasta QA\"},\"listing\":{\"myAppLogoIconUrl\":\"https://ad-test1-file.s3.amazonaws.com/app_resources/2796/myAppIcon/img4770557193469565010.png\",\"imageUrl\":\"https://ad-test1-file.s3.amazonaws.com/app_resources/2796/thumbs_64/img6134573503750801630.png\",\"profileImageUrl\":\"https://ad-test1-file.s3.amazonaws.com/app_resources/2796/thumbs_112/img1379524079972447927.png\",\"blurb\":\"5 word description\",\"overview\":\"Lorem ipsum dolor sit amet, poker old hauled got jig sherrif quarrel hospitality soap barn done come plumb overalls preacher. Buffalo shed kickin', and penny tools out, cow. Her caught showed give moonshine havin' heapin', up naw has, plug-nickel no skinned nothin'. Bankrupt bull gospel give shed shotgun water good hee-haw greasy it skinny.\",\"rating\":null,\"reviewCount\":0},\"overview\":{\"splashTitle\":\"Splash\",\"splashDescription\":\"description\",\"imageUrl\":\"https://ad-test1-file.s3.amazonaws.com/app_resources/2796/overview/img1089312710507853699.png\",\"demoUrl\":null,\"documentationUrl\":null,\"systemRequirements\":null,\"downloadFileSize\":null,\"benefits\":[],\"versions\":{}},\"support\":{\"email\":\"support@do-not-reply.com\",\"phone\":null,\"knowledgebaseUrl\":null,\"description\":null},\"privacyUrl\":null,\"termsUrl\":null,\"resources\":[],\"screenshots\":[],\"addonOfferings\":[],\"featuredCustomers\":[],\"featuredMedia\":[],\"href\":\"https://teststaples.appdirect.com/api/marketplace/v1/products/2796\",\"lastModified\":1367360584000,\"pricing\":{\"footnotes\":[],\"editions\":[{\"id\":3754,\"rank\":null,\"name\":\"One Time Edition\",\"primary\":true,\"trial\":{\"length\":0,\"unit\":\"DAY\"},\"expiredTrialGracePeriod\":0,\"plans\":[{\"id\":3884,\"frequency\":\"ONE_TIME\",\"contract\":{\"minimumServiceLength\":null,\"cancellationPeriodLimit\":null,\"endOfContractGracePeriod\":null,\"blockSwitchToShorterContract\":false,\"blockContractDowngrades\":false,\"blockContractUpgrades\":false,\"gracePeriod\":null,\"terminationFee\":null,\"autoExtensionPricingId\":null},\"allowCustomUsage\":false,\"keepBillDateOnUsageChange\":false,\"separatePrepaid\":false,\"costs\":[{\"unit\":\"NOT_APPLICABLE\",\"minUnits\":0E-10,\"maxUnits\":null,\"meteredUsage\":false,\"increment\":null,\"pricePerIncrement\":false,\"blockContractDecrease\":false,\"blockContractIncrease\":false,\"blockOriginalContractDecrease\":false,\"amounts\":[{\"currency\":\"CAD\",\"value\":10.0000000000},{\"currency\":\"USD\",\"value\":10.0000000000}]}],\"discountDetails\":null,\"primaryPrice\":false}],\"bullets\":[],\"items\":[{\"unit\":\"USER\",\"amount\":1.0000000000,\"unlimited\":false}]},{\"id\":3755,\"rank\":null,\"name\":\"Free\",\"primary\":false,\"trial\":{\"length\":0,\"unit\":\"DAY\"},\"expiredTrialGracePeriod\":0,\"plans\":[{\"id\":3885,\"frequency\":\"MONTHLY\",\"contract\":{\"minimumServiceLength\":null,\"cancellationPeriodLimit\":null,\"endOfContractGracePeriod\":null,\"blockSwitchToShorterContract\":false,\"blockContractDowngrades\":false,\"blockContractUpgrades\":false,\"gracePeriod\":null,\"terminationFee\":null,\"autoExtensionPricingId\":null},\"allowCustomUsage\":false,\"keepBillDateOnUsageChange\":false,\"separatePrepaid\":false,\"costs\":[{\"unit\":\"NOT_APPLICABLE\",\"minUnits\":0E-10,\"maxUnits\":null,\"meteredUsage\":false,\"increment\":null,\"pricePerIncrement\":false,\"blockContractDecrease\":false,\"blockContractIncrease\":false,\"blockOriginalContractDecrease\":false,\"amounts\":[{\"currency\":\"CAD\",\"value\":0E-10},{\"currency\":\"USD\",\"value\":0E-10}]}],\"discountDetails\":null,\"primaryPrice\":false}],\"bullets\":[],\"items\":[{\"unit\":\"USER\",\"amount\":1.0000000000,\"unlimited\":false}]}]},\"publishedOn\":1341249608000,\"buyable\":true,\"free\":true,\"freeTrialOrEditionPresent\":true,\"referable\":false,\"leadgen\":false,\"downloadFileSize\":null,\"features\":[{\"header\":\"header\",\"description\":\"Description*\",\"imageUrl\":null}],\"tags\":[],\"supportedLanguages\":[\"en\"]}";

        [SetUp]
        public void Init()
        {
            _appDirectApiMock = Substitute.For<IAppDirectApi>();
            _log = Substitute.For<ILogger>();
            _cachedAppDirectApi = new CachedAppDirectApi(_appDirectApiMock, _log);
        }

        [Test]
        public void CachedMyAppsCallsMyApps()
        {
            var apps = _cachedAppDirectApi.MyApps;
            var myApps = _appDirectApiMock.Received().MyApps;
        }

        [Test]
        public void CachedSuggestedAppsCallsSuggestedApps()
        {
            var apps = _cachedAppDirectApi.SuggestedApps;
            var myApps = _appDirectApiMock.Received().SuggestedApps;
        }

        [Test]
        public void CachedSuggestedAppsDoesNotReturnReferableSuggestedApps()
        {
            _appDirectApiMock.SuggestedApps.ReturnsForAnyArgs(new WebApplicationsListApplication[]{new WebApplicationsListApplication(){Referable = "True"}});
            var apps = _cachedAppDirectApi.SuggestedApps;
            Assert.IsEmpty(apps);
        }

        [Test]
        public void CachedSuggestedAppsDoesReturnNonReferableSuggestedApps()
        {
            _appDirectApiMock.SuggestedApps.ReturnsForAnyArgs(new WebApplicationsListApplication[] { new WebApplicationsListApplication() { Referable = "False" } });
            var apps = _cachedAppDirectApi.SuggestedApps;
            Assert.IsNotEmpty(apps);
        }

        [Test]
        public void CachedIsAuthenticatedAppsCallsIsAuthenticated()
        {
            var apps = _cachedAppDirectApi.SuggestedApps;
            var myApps = _appDirectApiMock.Received().SuggestedApps;
        }

        [Test]
        public void CachedAuthenticateCallsAuthenticate()
        {
            _cachedAppDirectApi.Authenticate("1", "2");
            _appDirectApiMock.ReceivedWithAnyArgs().Authenticate("1", "2");
        }

        [Test]
        public void CachedUnAuthenticateCallsUnAuthenticate()
        {
            _cachedAppDirectApi.UnAuthenticate();
            _appDirectApiMock.ReceivedWithAnyArgs().UnAuthenticate();
        }

        [Test]
        public void MyAppsAreNeverNull()
        {
            Assert.IsNotNull(_cachedAppDirectApi.MyApps);
        }

        [Test]
        public void IsNotAuthenticatedByDefault()
        {
            Assert.IsFalse(_cachedAppDirectApi.IsAuthenticated);
        }

        [Test]
        public void ReturnedSizeForSuggestedAppsIsLessThanOrEqualTo35()
        {
            var suggestedApps = new WebApplicationsListApplication[101];
            for (var i = 0;  i < 101; i++)
            {
                var app = new WebApplicationsListApplication();
                app.Id = "1";
                suggestedApps[i] = app;
            }

            _appDirectApiMock.SuggestedApps.ReturnsForAnyArgs(suggestedApps);

            var apps = _cachedAppDirectApi.SuggestedApps;

            Assert.IsTrue(apps.Count <= 35);
        }

        [Test]
        public void ProvisionNullWhenNotAuthenticated()
        {
            Assert.IsNull(_cachedAppDirectApi.ProvisionApplication("1", "2", "3"));
        }

        [Test]
        public void DeprovisionFalseWhenNotAuthenticated()
        {
            Assert.IsFalse(_cachedAppDirectApi.DeprovisionApplication("5"));
        }
        
        [Test]
        public void UserInfoMatchesRaw()
        {
            UserInfoRaw testRawUserInfo = new UserInfoRaw()
                {
                    User_Id = "721dbad9-f567-4790-802a-b2a5ad5a7a58",
                    Name = "Sneha Agnihotri",
                    Given_Name = "Sneha",
                    Family_Name = "Agnihotri",
                    Email = "sneha.agnihotri@appdirect.com",
                    Verified = true,
                    Company_Id = "d9785380-6227-11e0-99c3-12313d022285",
                    Locale = "en_US"
                };

            _appDirectApiMock.UserInfo.ReturnsForAnyArgs(testRawUserInfo);

            var userInfo = _cachedAppDirectApi.UserInfo;

            Assert.AreEqual(testRawUserInfo.User_Id, userInfo.UserId);
            Assert.AreEqual(testRawUserInfo.Name, userInfo.Name);
            Assert.AreEqual(testRawUserInfo.Given_Name, userInfo.GivenName);
            Assert.AreEqual(testRawUserInfo.Family_Name, userInfo.FamilyName);
            Assert.AreEqual(testRawUserInfo.Email, userInfo.Email);
            Assert.AreEqual(testRawUserInfo.Company_Id, userInfo.CompanyId);
            Assert.AreEqual(testRawUserInfo.Locale, userInfo.Locale);
        }

        [Test]
        public void GetFreeSubscriptionPlanIdReturnsNullForNoFreeProduct()
        {
            string _applicationId = "test352";
            var testProduct = _serializer.Deserialize<Product>(_testProductXml);

            foreach (var edition in testProduct.Pricing.Editions)
            {
                foreach (var plan in edition.Plans)
                {
                    foreach (var cost in plan.Costs)
                    {
                        foreach (var amount in cost.Amounts)
                        {
                            amount.Value = 5m;
                        }
                    }
                }
            }
            
            _appDirectApiMock.GetExtendedAppInfo(_applicationId).Returns(testProduct);

            Assert.IsNull(_cachedAppDirectApi.GetFreeSubscriptionPlanId(_applicationId));
        }

        [Test]
        public void GetFreeSubscriptionPlanIdReturnsNullForMetered()
        {
            string _applicationId = "test352";
            var testProduct = _serializer.Deserialize<Product>(_testProductXml);

            foreach (var edition in testProduct.Pricing.Editions)
            {
                foreach (var plan in edition.Plans)
                {
                    foreach (var cost in plan.Costs)
                    {
                        cost.MeteredUsage = true;
                    }
                }
            }
            
            _appDirectApiMock.GetExtendedAppInfo(_applicationId).Returns(testProduct);

            Assert.IsNull(_cachedAppDirectApi.GetFreeSubscriptionPlanId(_applicationId));
        }

        [Test]
        public void GetFreeSubscriptionPlanIdReturnsFirstFree()
        {
            string _applicationId = "test352";
            var testProduct = _serializer.Deserialize<Product>(_testProductXml);

            foreach (var cost in testProduct.Pricing.Editions[0].Plans[0].Costs)
            {
                cost.MeteredUsage = false;

                foreach (var amount in cost.Amounts)
                {
                    amount.Value = 0m;
                }
            }

            _appDirectApiMock.GetExtendedAppInfo(_applicationId).Returns(testProduct);

            Assert.AreEqual(testProduct.Pricing.Editions[0].Plans[0].Id, _cachedAppDirectApi.GetFreeSubscriptionPlanId(_applicationId));
        }

        [Test]
        public void GetFreeSubscriptionPlanIdDoesNotReturnMeteredUsagePlans()
        {
            string _applicationId = "test352";
            var testProduct = _serializer.Deserialize<Product>(_testProductXml);

            foreach (var cost in testProduct.Pricing.Editions[0].Plans[0].Costs)
            {
                cost.MeteredUsage = true;

                foreach (var amount in cost.Amounts)
                {
                    amount.Value = 0m;
                }
            }

            _appDirectApiMock.GetExtendedAppInfo(_applicationId).Returns(testProduct);

            Assert.AreNotEqual(testProduct.Pricing.Editions[0].Plans[0].Id, _cachedAppDirectApi.GetFreeSubscriptionPlanId(_applicationId));
        }

        [Test]
        public void GetFreeSubscriptionPlanIdDoesNotReturnPlanWithAmountNotZero()
        {
            string _applicationId = "test352";
            var testProduct = _serializer.Deserialize<Product>(_testProductXml);

            foreach (var cost in testProduct.Pricing.Editions[0].Plans[0].Costs)
            {
                cost.MeteredUsage = true;

                foreach (var amount in cost.Amounts)
                {
                    amount.Value = 0m;
                }
            }

            testProduct.Pricing.Editions[0].Plans[0].Costs[0].Amounts[0].Value = 1m;

            _appDirectApiMock.GetExtendedAppInfo(_applicationId).Returns(testProduct);

            Assert.AreNotEqual(testProduct.Pricing.Editions[0].Plans[0].Id, _cachedAppDirectApi.GetFreeSubscriptionPlanId(_applicationId));
        }

        [Test]
        public void GetFreeSubscriptionPlanIdReturnsFreePlan()
        {
            string _applicationId = "test352";
            var testProduct = _serializer.Deserialize<Product>(_testProductXml);
            
            _appDirectApiMock.GetExtendedAppInfo(_applicationId).Returns(testProduct);

            var freeSubscriptionPlanId = _cachedAppDirectApi.GetFreeSubscriptionPlanId(_applicationId);
            ProductPricingEditionPlansPlan returnedPlan = null;
            foreach (var edition in testProduct.Pricing.Editions)
            {
                foreach (var plan in edition.Plans)
                {
                    if (plan.Id == freeSubscriptionPlanId)
                    {
                        returnedPlan = plan;
                        break;
                    }
                }
                if (returnedPlan != null)
                {
                    break;
                }
            }

            if (returnedPlan == null)
            {
                Assert.Fail();
            }

            foreach (var cost in returnedPlan.Costs)
            {
                Assert.IsTrue(cost.MeteredUsage == null || cost.MeteredUsage == false);

                foreach (var amount in cost.Amounts)
                {
                    Assert.IsTrue(amount.Value == null || amount.Value == decimal.Zero);
                }
            }
        }
    }
}