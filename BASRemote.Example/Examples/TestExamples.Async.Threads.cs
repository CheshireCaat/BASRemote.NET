using System;
using System.Threading.Tasks;
using BASRemote.Objects;

namespace BASRemote.Example.Examples
{
    public static partial class TestExamples
    {
        public static async Task MultipleAsyncFunctionRunWithThread(IBasRemoteClient client)
        {
            var thread = client.CreateThread();

            var result1 = await thread.RunFunction<int>("Add",
                new Params
                {
                    {"X", 4},
                    {"Y", 5}
                });

            var result2 = await thread.RunFunction<int>("Add",
                new Params
                {
                    {"X", 6},
                    {"Y", 7}
                });

            Console.WriteLine("--------ExampleAsyncFunctionWithThread-------");
            Console.WriteLine($"Result #1 is: {result1}");
            Console.WriteLine($"Result #2 is: {result2}");
        }

        public static async Task AsyncFunctionRunWithThread(IBasRemoteClient client)
        {
            var thread = client.CreateThread();

            var result = await thread.RunFunction("Add",
                new Params
                {
                    {"X", 4},
                    {"Y", 5}
                });

            Console.WriteLine("--------ExampleAsyncFunctionWithThread-------");
            Console.WriteLine($"Result is: {result}");
        }
    }
}