using System.Linq;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.IntegrationTests.Common;
using BASRemote.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BASRemote.IntegrationTests
{
    [TestClass]
    public class ThreadsTests : BaseTest
    {
        [TestMethod]
        public async Task ParallelAsyncFunctionRun()
        {
            var threads = Enumerable.Range(1, 3).Select(x => Client.CreateThread()).ToList();

            var result = await Task.WhenAll(Enumerable.Range(1, 3).Select(x =>
            {
                var y = x * 2;
                return threads[x - 1].RunFunction("Add",
                    new Params
                    {
                        {"X", x},
                        {"Y", y}
                    }).GetTask<int>();
            }));

            foreach (var thread in threads)
            {
                Assert.IsFalse(thread.IsRunning);
                thread.Stop();
                Assert.IsTrue(thread.Id == 0);
            }

            Assert.IsTrue(result.Length == 3);
            CollectionAssert.AreEqual(new[] {3, 6, 9}, result);
        }

        [TestMethod]
        public async Task MultipleAsyncFunctionRun()
        {
            var thread = Client.CreateThread();

            var result1 = await thread.RunFunction("Add",
                new Params
                {
                    {"X", 4},
                    {"Y", 5}
                }).GetTask<int>();

            var result2 = await thread.RunFunction("Add",
                new Params
                {
                    {"X", 6},
                    {"Y", 7}
                }).GetTask<int>();


            Assert.IsFalse(thread.IsRunning);
            thread.Stop();
            Assert.IsTrue(thread.Id == 0);

            Assert.AreEqual(9, result1);
            Assert.AreEqual(13, result2);
        }

        [TestMethod]
        public async Task NotExistingAsyncFunctionRun()
        {
            var thread = Client.CreateThread();

            await Assert.ThrowsExceptionAsync<FunctionException>(() => thread.RunFunction("Add1",
                new Params
                {
                    {"X", 4},
                    {"Y", 5}
                }).GetTask<int>());

            Assert.IsFalse(thread.IsRunning);
            thread.Stop();
            Assert.IsTrue(thread.Id == 0);
        }

        [TestMethod]
        public async Task AsyncFunctionRun()
        {
            var thread = Client.CreateThread();

            var result = await thread.RunFunction("Add",
                new Params
                {
                    {"X", 0},
                    {"Y", 0}
                }).GetTask<int>();


            Assert.IsFalse(thread.IsRunning);
            thread.Stop();
            Assert.IsTrue(thread.Id == 0);

            Assert.AreEqual(0, result);
        }
    }
}