using System.Windows;
using AppDirect.WindowsClient.API;
using AppDirect.WindowsClient.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AppDirect.WindowsClient.Tests.UI
{
    [TestClass]
    public class RegistrationViewTest
    {
        private volatile IAppDirectApi _appDirectApiMock;
        private volatile ICachedAppDirectApi _cachedAppDirectApiMock;

        [TestInitialize]
        public void Initialize()
        {
            lock (this)
            {
                _appDirectApiMock = Substitute.For<IAppDirectApi>();
                _cachedAppDirectApiMock = Substitute.For<ICachedAppDirectApi>();

                var kernel = ServiceLocator.Kernel;
                kernel.Bind<IAppDirectApi>().ToConstant(_appDirectApiMock);
                kernel.Bind<ICachedAppDirectApi>().ToConstant(_cachedAppDirectApiMock);
            }
        }

        [TestMethod]
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
