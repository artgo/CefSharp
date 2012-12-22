using System;
using MainApplication;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MainApplicationUnitTests
{
    [TestClass]
    public class MainViewModelUnitTest
    {
        private MainViewModel _mainViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _mainViewModel = new MainViewModel();
        }

        [TestMethod]
        public void MyApplicationsCollectionIsPopulated()
        {
            Assert.IsTrue(_mainViewModel.SuggestedApplications.Count > 0);
        }

        [TestMethod]
        public void SuggestedApplicationsCollectionIsNotNull()
        {
            Assert.IsNotNull(_mainViewModel.MyApplications);
        }
    }
}
