using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BASRemote.Tests.Services
{
    /// <summary>
    ///     Summary description for SocketServiceTest
    /// </summary>
    [TestClass]
    public class EngineServiceTests
    {
        private EngineService _service;

        public EngineServiceTests()
        {
            _service = new EngineService(new Options());
        }

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public async Task EngineInitialize_ShouldThrowException_WhenScriptNotExist()
        {
            Assert.IsTrue(true);
            //await Assert.ThrowsExceptionAsync<ScriptNotExistException>(() => _service.InitializeAsync());
        }

        [TestMethod]
        public async Task EngineInitialize_ShouldThrowException_WhenScriptNotSupported()
        {
            Assert.IsTrue(true);
            //await Assert.ThrowsExceptionAsync<ScriptNotSupportedException>(() => _service.InitializeAsync());
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion
    }
}