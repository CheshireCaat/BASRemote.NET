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
        [DataRow(1, 3)]
        [DataRow(3, 5)]
        [DataRow(5, 7)]
        public async Task ParallelAsyncFunctionRun(int start, int count)
        {
            var range = GetRange(start, count);
            var threads = range.Select(x => Client.CreateThread()).ToList();
            var expected = range.Select(x => x + x * 2).ToArray();

            var result = await Task.WhenAll(range
                .Select((x, i) => threads[i].RunFunction("Add",
                    new Params
                    {
                        {"X", x},
                        {"Y", x * 2}
                    }).GetTask<int>()));

            AssertThreads(threads);

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 1, 1)]
        [DataRow(1, 0, 1)]
        public async Task MultipleAsyncFunctionRun(int x, int y, int sum)
        {
            var thread = Client.CreateThread();

            var result1 = await thread.RunFunction("Add",
                new Params
                {
                    {"X", x},
                    {"Y", y}
                }).GetTask<int>();

            var result2 = await thread.RunFunction("Add",
                new Params
                {
                    {"X", x + 1},
                    {"Y", y + 1}
                }).GetTask<int>();


            AssertThread(thread);

            Assert.AreEqual(sum, result1);
            Assert.AreEqual(sum + 2, result2);
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        public async Task NotExistingAsyncFunctionRun(int x, int y)
        {
            var thread = Client.CreateThread();

            await Assert.ThrowsExceptionAsync<FunctionException>(() => thread.RunFunction("Add1",
                new Params
                {
                    {"X", x},
                    {"Y", y}
                }).GetTask<int>());

            AssertThread(thread);
        }

        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 1, 1)]
        [DataRow(1, 0, 1)]
        public async Task AsyncFunctionRun(int x, int y, int sum)
        {
            var thread = Client.CreateThread();

            var result = await thread.RunFunction("Add",
                new Params
                {
                    {"X", x},
                    {"Y", y}
                }).GetTask<int>();

            AssertThread(thread);
            Assert.AreEqual(sum, result);
        }
    }
}