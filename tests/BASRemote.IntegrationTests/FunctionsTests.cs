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
        public async Task ParallelAsyncFunctionRun()
        {
            var result = await Task.WhenAll(Enumerable.Range(1, 3).Select(x =>
            {
                var y = x * 2;
                return Client.RunFunction("Add",
                    new Params
                    {
                        {"X", x},
                        {"Y", y}
                    }).GetTask<int>();
            }));

            Assert.IsTrue(result.Length == 3);
            CollectionAssert.AreEqual(new[] {3, 6, 9}, result);
        }

        [TestMethod]
        public async Task MultipleAsyncFunctionRun()
        {
            var result1 = await Client.RunFunction("Add",
                new Params
                {
                    {"X", 4},
                    {"Y", 5}
                }).GetTask<int>();

            var result2 = await Client.RunFunction("Add",
                new Params
                {
                    {"X", 6},
                    {"Y", 7}
                }).GetTask<int>();

            Assert.AreEqual(9, result1);
            Assert.AreEqual(13, result2);
        }

        [TestMethod]
        public async Task NotExistingAsyncFunctionRun()
        {
            await Assert.ThrowsExceptionAsync<FunctionException>(() => Client.RunFunction("Add1",
                new Params
                {
                    {"X", 4},
                    {"Y", 5}
                }).GetTask<int>());
        }

        [TestMethod]
        public async Task AsyncFunctionRun()
        {
            var result = await Client.RunFunction("Add",
                new Params
                {
                    {"X", 0},
                    {"Y", 0}
                }).GetTask<int>();

            Assert.AreEqual(0, result);
        }
    }
}