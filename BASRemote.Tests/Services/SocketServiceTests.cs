using BASRemote.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BASRemote.Tests.Services
{
    /// <summary>
    ///     Summary description for SocketServiceTests
    /// </summary>
    [TestClass]
    public class SocketServiceTests
    {
        private SocketService _service;

        public SocketServiceTests()
        {
            _service = new SocketService(new Options());
        }

        /// <summary>
        ///     Gets or sets the test context which provides
        ///     information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
            //
            // TODO: Subscribe test logic here
            //
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