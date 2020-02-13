using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Objects;

namespace BASRemote.Example
{
    public static class Threads
    {
        public static async Task ParallelAsyncFunctionRun(IBasRemoteClient client)
        {
            var threads = new List<IBasThread>
            {
                client.CreateThread(),
                client.CreateThread()
            };

            var result = await Task.WhenAll(Enumerable.Range(1, 2).Select(x =>
            {
                var y = x * 2;
                return threads[x - 1].RunFunction("Add",
                    new Params
                    {
                        {"X", x},
                        {"Y", y}
                    }).GetTask<int>();
            }));

            foreach (var thread in threads) thread.Stop();

            Console.WriteLine();
            Console.WriteLine("[ParallelAsyncFunctionRun]");
            Console.WriteLine($"Result #1 is: {result[0]}");
            Console.WriteLine($"Result #2 is: {result[1]}");
        }

        public static async Task MultipleAsyncFunctionRun(IBasRemoteClient client)
        {
            var thread = client.CreateThread();

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

            Console.WriteLine();
            Console.WriteLine("[MultipleAsyncFunctionRun]");
            Console.WriteLine($"Result #1 is: {result1}");
            Console.WriteLine($"Result #2 is: {result2}");

            thread.Stop();
        }

        public static async Task NotExistingAsyncFunctionRun(IBasRemoteClient client)
        {
            var thread = client.CreateThread();
            object result;

            try
            {
                result = await thread.RunFunction("Add1",
                    new Params
                    {
                        {"X", 4},
                        {"Y", 5}
                    }).GetTask<int>();
            }
            catch (FunctionException exception)
            {
                result = exception.Message;
            }

            Console.WriteLine();
            Console.WriteLine("[NotExistingAsyncFunctionRun]");
            Console.WriteLine($"Result is: {result}");

            thread.Stop();
        }

        public static async Task AsyncFunctionRun(IBasRemoteClient client)
        {
            var thread = client.CreateThread();

            var result = await thread.RunFunction("Add",
                new Params
                {
                    {"X", 0},
                    {"Y", 0}
                }).GetTask<int>();

            Console.WriteLine();
            Console.WriteLine("[AsyncFunctionRun]");
            Console.WriteLine($"Result is: {result}");

            thread.Stop();
        }
    }
}