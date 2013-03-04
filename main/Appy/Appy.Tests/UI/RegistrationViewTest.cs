using System.Windows;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.UI;
using NSubstitute;
using NUnit.Framework;

namespace AppDirect.WindowsClient.Tests.UI
{
    [TestFixture]
    public class RegistrationViewTest
    {
        private volatile IAppDirectApi _appDirectApiMock;
        private volatile ICachedAppDirectApi _cachedAppDirectApiMock;

        [TestFixtureSetUp]
        public void Initialize()
        {
            lock (this)
            {
                _appDirectApiMock = Substitute.For<IAppDirectApi>();
                _cachedAppDirectApiMock = Substitute.For<ICachedAppDirectApi>();

                var kernel = ServiceLocator.Kernel;
                kernel.Rebind<IAppDirectApi>().ToConstant(_appDirectApiMock);
                kernel.Rebind<ICachedAppDirectApi>().ToConstant(_cachedAppDirectApiMock);
            }
        }

        [Test]
        public void OnRegisterClickCallsRightApis()
        {
            //var registerWindow = new RegistrationView();
            //registerWindow.RegisterButton_Click(registerWindow, new RoutedEventArgs());
            //_appDirectApiMock.Received().RegisterUser(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            //    Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            //    Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
