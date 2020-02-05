using System;
using System.Threading;
using BASRemote.Objects;

namespace BASRemote.Example
{
    public static partial class Functions
    {
        public static void MultipleFunctionRun(IBasRemoteClient client)
        {
            var wait1 = new ManualResetEventSlim(false);
            var wait2 = new ManualResetEventSlim(false);
            dynamic data1 = null;
            dynamic data2 = null;

            client.RunFunctionSync("Add", new Params
            {
                {"X", 4},
                {"Y", 5}
            }, result1 =>
            {
                data1 = result1;
                wait1.Set();

                client.RunFunctionSync("Add", new Params
                {
                    {"X", 10},
                    {"Y", 15}
                }, result2 =>
                {
                    data2 = result2;
                    wait2.Set();
                }, exception => { });
            }, exception => { });

            wait1.Wait();
            wait2.Wait();

            Console.WriteLine();
            Console.WriteLine("[MultipleFunctionRun]");
            Console.WriteLine($"Result #1 is: {data1}");
            Console.WriteLine($"Result #2 is: {data2}");
        }

        public static void NotExistingFunctionRun(IBasRemoteClient client)
        {
            var wait = new ManualResetEventSlim(false);
            dynamic data = null;

            client.RunFunctionSync("Add1",
                new Params
                {
                    {"X", 41},
                    {"Y", 51}
                }, result =>
                {
                    data = result;
                    wait.Set();
                }, exception =>
                {
                    data = exception.Message;
                    wait.Set();
                });

            wait.Wait();

            Console.WriteLine();
            Console.WriteLine("[NotExistingFunctionRun]");
            Console.WriteLine($"Result is: {data}");
        }

        public static void FunctionRun(IBasRemoteClient client)
        {
            var wait = new ManualResetEventSlim(false);
            dynamic data = null;

            client.RunFunctionSync("Add",
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

            Console.WriteLine();
            Console.WriteLine("[FunctionRun]");
            Console.WriteLine($"Result is: {data}");
        }
    }
}
