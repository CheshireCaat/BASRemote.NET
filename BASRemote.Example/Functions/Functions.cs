using System;
using System.Threading.Tasks;
using BASRemote.Objects;

namespace BASRemote.Example
{
    public static partial class Functions
    {
        public static async Task MultipleFunctionRun(IBasRemoteClient client)
        {
            var tcs1 = new TaskCompletionSource<dynamic>();
            var tcs2 = new TaskCompletionSource<dynamic>();

            client.RunFunctionSync("Add", new Params
            {
                {"X", 4},
                {"Y", 5}
            }, result1 =>
            {
                tcs1.TrySetResult(result1);

                client.RunFunctionSync("Add", new Params
                    {
                        {"X", 10},
                        {"Y", 15}
                    },
                    result2 => { tcs2.TrySetResult(result2); },
                    exception => { });
            }, exception => { });

            var values = await Task.WhenAll(tcs1.Task, tcs2.Task);

            Console.WriteLine();
            Console.WriteLine("[MultipleFunctionRun]");
            Console.WriteLine($"Result #1 is: {values[0]}");
            Console.WriteLine($"Result #2 is: {values[1]}");
        }

        public static async Task NotExistingFunctionRun(IBasRemoteClient client)
        {
            var tcs = new TaskCompletionSource<dynamic>();

            client.RunFunctionSync("Add1",
                new Params
                {
                    {"X", 41},
                    {"Y", 51}
                },
                result => tcs.TrySetResult(result),
                exception => tcs.TrySetResult(exception.Message));

            var value = await tcs.Task;

            Console.WriteLine();
            Console.WriteLine("[NotExistingFunctionRun]");
            Console.WriteLine($"Result is: {value}");
        }

        public static async Task FunctionRun(IBasRemoteClient client)
        {
            var tcs = new TaskCompletionSource<dynamic>();

            client.RunFunctionSync("Add",
                new Params
                {
                    {"X", 4},
                    {"Y", 5}
                },
                result => tcs.TrySetResult(result),
                exception => { });

            var value = await tcs.Task;

            Console.WriteLine();
            Console.WriteLine("[FunctionRun]");
            Console.WriteLine($"Result is: {value}");
        }
    }
}