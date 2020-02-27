using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

        public static void AssertThreads(IEnumerable<IBasThread> threads)
        {
            foreach (var thread in threads) AssertThread(thread);
        }

        public static void AssertThread(IBasThread thread)
        {
            Assert.IsFalse(thread.IsRunning);
            thread.Stop();
            Assert.IsTrue(thread.Id == 0);
        }

        public static List<int> GetRange(int start, int count)
        {
            return Enumerable.Range(start, count).ToList();
        }
    }
}