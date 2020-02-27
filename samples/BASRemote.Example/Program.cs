using System;
using System.Threading.Tasks;
using BASRemote.Objects;

namespace BASRemote.Example
{
    internal static class Program
    {
        private static async Task Main()
        {
            using (var client = new BasRemoteClient(new Options {ScriptName = "TestRemoteControl"}))
            {
                await client.Start();

                var arguments = new Params {["Query"] = "cats"};
                var result = await client
                    .RunFunction("GoogleSearch", arguments)
                    .GetTask<string[]>();

                foreach (var link in result)
                {
                    Console.WriteLine(link);
                }
            }

            Console.ReadKey();
        }
    }
}