using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BASRemote.Exceptions;
using BASRemote.Objects;

namespace BASRemote.Example
{
    public static partial class Threads
    {
        public static async Task ParallelAsyncFunctionRunWithThread(IBasRemoteClient client)
        {
            var threads = new List<IBasThread>
            {
                client.CreateThread(),
                client.CreateThread()
            };

            var result = await Task.WhenAll(Enumerable.Range(1, 2).Select(x =>
            {
                var y = x * 2;
                return threads[x - 1].RunFunction<int>("Add",
                    new Params
                    {
                        {"X", x},
                        {"Y", y}
                    });
            }));

            foreach (var thread in threads) thread.Stop();

            Console.WriteLine();
            Console.WriteLine("[ParallelAsyncFunctionRunWithThread]");
            Console.WriteLine($"Result #1 is: {result[0]}");
            Console.WriteLine($"Result #2 is: {result[1]}");
        }

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

            Console.WriteLine();
            Console.WriteLine("[MultipleAsyncFunctionRunWithThread]");
            Console.WriteLine($"Result #1 is: {result1}");
            Console.WriteLine($"Result #2 is: {result2}");

            thread.Stop();
        }

        public static async Task NotExistingAsyncFunctionRunWithThread(IBasRemoteClient client)
        {
            var thread = client.CreateThread();
            object result;

            try
            {
                result = await thread.RunFunction<int>("Add1",
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
            Console.WriteLine("[NotExistingAsyncFunctionRunWithThread]");
            Console.WriteLine($"Result is: {result}");

            thread.Stop();
        }

        public static async Task AsyncFunctionRunWithThread(IBasRemoteClient client)
        {
            var thread = client.CreateThread();

            var result = await thread.RunFunction<int>("Add",
                new Params
                {
                    {"X", 0},
                    {"Y", 0}
                });

            Console.WriteLine();
            Console.WriteLine("[AsyncFunctionRunWithThread]");
            Console.WriteLine($"Result is: {result}");

            thread.Stop();
        }
    }
}