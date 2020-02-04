using System;
using System.Threading;
using BASRemote.Objects;

namespace BASRemote.Example.Examples
{
    public static partial class TestExamples
    {
        public static void MultipleFunctionRunWithThread(IBasRemoteClient client)
        {
            var wait1 = new ManualResetEventSlim(false);
            var wait2 = new ManualResetEventSlim(false);
            var thread = client.CreateThread();
            dynamic data1 = null;
            dynamic data2 = null;

            thread.RunFunctionSync("Add", new Params
            {
                {"X", 4},
                {"Y", 5}
            }, result1 =>
            {
                data1 = result1;
                wait1.Set();

                thread.RunFunctionSync("Add", new Params
                {
                    {"X", 4},
                    {"Y", 5}
                }, result2 =>
                {
                    data2 = result2;
                    wait2.Set();
                }, exception => { });
            }, exception => { });

            wait1.Wait();
            wait2.Wait();

            Console.WriteLine("--------ExampleAsyncFunctionWithThread-------");
            Console.WriteLine($"Result #1 is: {data1}");
            Console.WriteLine($"Result #2 is: {data2}");
        }

        public static void FunctionRunWithThread(IBasRemoteClient client)
        {
            var wait = new ManualResetEventSlim(false);
            var thread = client.CreateThread();
            dynamic data = null;

            thread.RunFunctionSync("Add",
                new Params
                {
                    {"X", 4},
                    {"Y", 5}
                }, result =>
                {
                    data = result;
                    wait.Set();
                }, exception => { });

            wait.Wait();

            Console.WriteLine("--------ExampleFunctionWithThread-------");
            Console.WriteLine($"Result is: {data}");
        }
    }
}