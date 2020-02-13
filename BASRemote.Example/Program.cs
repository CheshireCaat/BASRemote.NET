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
        }

        private static async Task ThreadsRun(IBasRemoteClient client)
        {
            await Threads.ParallelAsyncFunctionRun(client);
            await Threads.NotExistingAsyncFunctionRun(client);
            await Threads.MultipleAsyncFunctionRun(client);
            await Threads.AsyncFunctionRun(client);
        }
    }
}