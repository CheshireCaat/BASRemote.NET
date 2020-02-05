using System;
using System.Linq;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Objects;

namespace BASRemote.Example
{
    public static partial class Functions
    {
        public static async Task ParallelAsyncFunctionRun(IBasRemoteClient client)
        {
            var result = await Task.WhenAll(Enumerable.Range(1, 2).Select(x =>
            {
                var y = x * 2;
                return client.RunFunction<int>("Add",
                    new Params
                    {
                        {"X", x},
                        {"Y", y}
                    });
            }));

            Console.WriteLine();
            Console.WriteLine("[ParallelAsyncFunctionRun]");
            Console.WriteLine($"Result #1 is: {result[0]}");
            Console.WriteLine($"Result #2 is: {result[1]}");
        }

        public static async Task MultipleAsyncFunctionRun(IBasRemoteClient client)
        {
            var result1 = await client.RunFunction<int>("Add",
                new Params
                {
                    {"X", 4},
                    {"Y", 5}
                });

            var result2 = await client.RunFunction<int>("Add",
                new Params
                {
                    {"X", 6},
                    {"Y", 7}
                });

            Console.WriteLine();
            Console.WriteLine("[MultipleAsyncFunctionRun]");
            Console.WriteLine($"Result #1 is: {result1}");
            Console.WriteLine($"Result #2 is: {result2}");
        }

        public static async Task NotExistingAsyncFunctionRun(IBasRemoteClient client)
        {
            object result;

            try
            {
                result = await client.RunFunction<int>("Add1",
                    new Params
                    {
                        {"X", 4},
                        {"Y", 5}
                    });
            }
            catch (FunctionException exception)
            {
                result = exception.Message;
            }

            Console.WriteLine();
            Console.WriteLine("[NotExistingAsyncFunctionRun]");
            Console.WriteLine($"Result is: {result}");
        }

        public static async Task AsyncFunctionRun(IBasRemoteClient client)
        {
            var result = await client.RunFunction<int>("Add",
                new Params
                {
                    {"X", 0},
                    {"Y", 0}
                });

            Console.WriteLine();
            Console.WriteLine("[AsyncFunctionRun]");
            Console.WriteLine($"Result is: {result}");
        }
    }
}