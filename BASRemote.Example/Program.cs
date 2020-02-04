using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BASRemote.Example.Examples;

namespace BASRemote.Example
{
    internal static class Program
    {
        private static void WriteStart()
        {
            Console.WriteLine("--------------BASRemote.Example--------------");
        }

        private static void WriteEnd()
        {
            Console.WriteLine("---------------------------------------------");
        }

        private static async Task Main()
        {
            using (var client = new BasRemoteClient(new Options {ScriptName = "RemoteControlTest"}))
            {
                WriteStart();

                var stopWatch = Stopwatch.StartNew();
                await client.StartAsync();
                stopWatch.Stop();

                var ts = stopWatch.Elapsed;
                var elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
                Console.WriteLine("RunTime " + elapsedTime);

                await TestExamples.MultipleAsyncFunctionRunWithThread(client);
                await TestExamples.AsyncFunctionRunWithThread(client);

                TestExamples.MultipleFunctionRunWithThread(client);
                TestExamples.FunctionRunWithThread(client);

                WriteEnd();
            }

            Console.ReadKey();
        }
    }
}