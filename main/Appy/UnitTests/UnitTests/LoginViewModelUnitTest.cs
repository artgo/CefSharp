using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.Storage;
using AppDirect.WindowsClient.UI;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UnitTests
{
    [TestFixture]
    public class LoginViewModelUnitTest
    {
        private volatile LoginViewModel _loginViewModel = new LoginViewModel();

        private const string Username = TestData.TestUsername;
        private const string Password = TestData.TestPassword;
        private const string BadPassword = "BadPassword";

        [TestFixtureSetUp]
        public void SetUpForTests()
        {
            var cachedAppDirectApiMock = Substitute.For<ICachedAppDirectApi>();

            var localStorage = new LocalStorage();

            cachedAppDirectApiMock.Authenticate(Username, Password).Returns(true);

            var kernel = ServiceLocator.Kernel;
            kernel.Rebind<ICachedAppDirectApi>().ToConstant(cachedAppDirectApiMock);
            kernel.Rebind<LocalStorage>().ToConstant(localStorage);
        }

        [Test]
        public void LoginReturnsTrueForValidLogin()
        {
            Assert.IsTrue(_loginViewModel.Login(Username, Password));
            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void LoginReturnsFalseForInvalidLogin()
        {
            Assert.IsFalse(_loginViewModel.Login(Username, BadPassword));
            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, BadPassword);
        }

        [Test]
        public void ValidUsernameIsStored()
        {
            _loginViewModel.Login(Username, Password);
            Assert.AreEqual(Username, ServiceLocator.LocalStorage.LoginInfo.Username);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void ValidPasswordIsStored()
        {
            _loginViewModel.Login(Username, Password);
            Assert.AreEqual(Password, ServiceLocator.LocalStorage.LoginInfo.Password);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void PasswordSetDateIsStored()
        {
            _loginViewModel.Login(Username, Password);
            Assert.AreEqual(DateTime.Now.Date, ServiceLocator.LocalStorage.LoginInfo.PasswordSetDate.Date);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, Password);
        }

        [Test]
        public void IncorrectLoginIsNotStored()
        {
            _loginViewModel.Login(Username, BadPassword);
            Assert.IsNull(ServiceLocator.LocalStorage.LoginInfo);

            ServiceLocator.CachedAppDirectApi.Received().Authenticate(Username, BadPassword);
        }
    }
}
