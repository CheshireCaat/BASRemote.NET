using System;
using System.Threading.Tasks;
using BASRemote.Example.Extensions;
using BASRemote.Objects;

namespace BASRemote.Example
{
    public static partial class Threads
    {
        public static async Task MultipleFunctionRunWithThread(IBasRemoteClient client)
        {
            var thread = client.CreateThread();
            object data1 = null;
            object data2 = null;

            thread.RunFunctionSync("Add", new Params
            {
                {"X", 4},
                {"Y", 5}
            }, result1 => data1 = result1, exception => { });

            await thread.Wait();
            thread.Stop();

            thread.RunFunctionSync("Add", new Params
            {
                {"X", 10},
                {"Y", 15}
            }, result2 => data2 = result2, exception => { });

            await thread.Wait();
            thread.Stop();

            Console.WriteLine();
            Console.WriteLine("[MultipleFunctionRunWithThread]");
            Console.WriteLine($"Result #1 is: {data1}");
            Console.WriteLine($"Result #2 is: {data2}");
        }

        public static async Task NotExistingFunctionRunWithThread(IBasRemoteClient client)
        {
            var thread = client.CreateThread();
            object data = null;

            thread.RunFunctionSync("Add1",
                new Params
                {
                    {"X", 41},
                    {"Y", 51}
                },
                result => data = result,
                exception => data = exception.Message);

            await thread.Wait();
            thread.Stop();

            Console.WriteLine();
            Console.WriteLine("[NotExistingFunctionRunWithThread]");
            Console.WriteLine($"Result is: {data}");
        }

        public static async Task FunctionRunWithThread(IBasRemoteClient client)
        {
            var thread = client.CreateThread();
            object data = null;

            thread.RunFunctionSync("Add",
                new Params
                {
                    {"X", 4},
                    {"Y", 5}
                },
                result => data = result,
                exception => { });

            await thread.Wait();
            thread.Stop();

            Console.WriteLine();
            Console.WriteLine("[FunctionRunWithThread]");
            Console.WriteLine($"Result is: {data}");
        }
    }
}