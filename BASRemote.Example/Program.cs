using System;
using System.Threading.Tasks;

namespace BASRemote.Example
{
    internal static class Program
    {
        private static async Task Main()
        {
            using (var client = new BasRemoteClient(new Options {ScriptName = "RemoteControlTest"}))
            {
                Console.WriteLine("--------------BASRemote.Example--------------");

                await client.Start();
                await FunctionsRun(client);
                await ThreadsRun(client);

                Console.WriteLine("---------------------------------------------");
            }

            Console.ReadKey();
        }

        private static async Task FunctionsRun(IBasRemoteClient client)
        {
            await Functions.ParallelAsyncFunctionRun(client);
            await Functions.NotExistingAsyncFunctionRun(client);
            await Functions.MultipleAsyncFunctionRun(client);
            await Functions.AsyncFunctionRun(client);

            await Functions.NotExistingFunctionRun(client);
            await Functions.MultipleFunctionRun(client);
            await Functions.FunctionRun(client);
        }

        private static async Task ThreadsRun(IBasRemoteClient client)
        {
            await Threads.ParallelAsyncFunctionRunWithThread(client);
            await Threads.NotExistingAsyncFunctionRunWithThread(client);
            await Threads.MultipleAsyncFunctionRunWithThread(client);
            await Threads.AsyncFunctionRunWithThread(client);

            await Threads.NotExistingFunctionRunWithThread(client);
            await Threads.MultipleFunctionRunWithThread(client);
            await Threads.FunctionRunWithThread(client);
        }
    }
}