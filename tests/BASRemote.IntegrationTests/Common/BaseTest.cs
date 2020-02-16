using System.Configuration;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BASRemote.IntegrationTests.Common
{
    [TestClass]
    public class BaseTest
    {
        protected static IBasRemoteClient Client;

        [AssemblyInitialize]
        public static async Task Initialize(TestContext _)
        {
            Client = new BasRemoteClient(new Options
            {
                ScriptName = ConfigurationManager.AppSettings["ScriptName"]
            });

            await Client.Start();
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            Client.Dispose();
        }
    }
}