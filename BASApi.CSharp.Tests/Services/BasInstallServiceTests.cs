using System;
using BASApi.CSharp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BASApi.CSharp.Tests.Services
{
    [TestClass]
    public class BasInstallServiceTests
    {
        private BasInstallService _installService;

        [TestInitialize]
        public void SetUp()
        {
            _installService = new BasInstallService();
        }

        [TestMethod]
        public void Install_WhenVersionSpecified_ShouldInstallProgram()
        {
            var isDownloaded = false;

            _installService.OnDownloadCompleted += () => { isDownloaded = true; };
            _installService.Install(new Version(22, 2, 0), "C:\\Курсы\\1");

            Assert.AreEqual(true, isDownloaded);
        }

        [TestMethod]
        public void Install_WhenStringSpecified_ShouldInstallProgram()
        {
            var isDownloaded = false;

            _installService.OnDownloadCompleted += () => { isDownloaded = true; };
            _installService.Install(new Version(22, 2, 0), "C:\\Курсы\\2");

            Assert.AreEqual(true, isDownloaded);
        }

        [TestMethod]
        public void IsProgramInstalled_WhenCorrectName_IsTrue()
        {
            Assert.AreEqual(true, BasInstallService.IsProgramInstalled("BrowserAutomationStudio"));
        }

        [TestMethod]
        public void IsProgramInstalled_WhenWrongName_IsFalse()
        {
            Assert.AreEqual(false, BasInstallService.IsProgramInstalled("BrowserAutomationStud"));
        }
    }
}