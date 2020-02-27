using System.Linq;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.IntegrationTests.Common;
using BASRemote.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BASRemote.IntegrationTests
{
    [TestClass]
    public class FunctionsTests : BaseTest
    {
        [TestMethod]
        [DataRow(1, 3)]
        [DataRow(3, 5)]
        [DataRow(5, 7)]
        public async Task ParallelAsyncFunctionRun(int start, int count)
        {
            var range = GetRange(start, count);
            var expected = range.Select(x => x + x * 2).ToArray();

            var result = await Task.WhenAll(range
                .Select(x => Client.RunFunction("Add",
                    new Params
                    {
                        {"X", x},
                        {"Y", x * 2}
                    }).GetTask<int>()));

            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 1, 1)]
        [DataRow(1, 0, 1)]
        public async Task MultipleAsyncFunctionRun(int x, int y, int sum)
        {
            var result1 = await Client.RunFunction("Add",
                new Params
                {
                    {"X", x},
                    {"Y", y}
                }).GetTask<int>();

            var result2 = await Client.RunFunction("Add",
                new Params
                {
                    {"X", x + 1},
                    {"Y", y + 1}
                }).GetTask<int>();

            Assert.AreEqual(sum, result1);
            Assert.AreEqual(sum + 2, result2);
        }

        [TestMethod]
        [DataRow(0, 0)]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        public async Task NotExistingAsyncFunctionRun(int x, int y)
        {
            await Assert.ThrowsExceptionAsync<FunctionException>(() => Client.RunFunction("Add1",
                new Params
                {
                    {"X", x},
                    {"Y", y}
                }).GetTask<int>());
        }

        [TestMethod]
        [DataRow(0, 0, 0)]
        [DataRow(0, 1, 1)]
        [DataRow(1, 0, 1)]
        public async Task AsyncFunctionRun(int x, int y, int sum)
        {
            var result = await Client.RunFunction("Add",
                new Params
                {
                    {"X", x},
                    {"Y", y}
                }).GetTask<int>();

            Assert.AreEqual(sum, result);
        }
    }
}