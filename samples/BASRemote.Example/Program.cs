using System;
using System.Threading.Tasks;
using BASRemote.Objects;

namespace BASRemote.Example
{
    internal static class Program
    {
        private static BasRemoteClient _client;

        private static async Task Main()
        {
            Console.WriteLine("[Google Search Example]");

            using (_client = new BasRemoteClient(new Options {ScriptName = "TestRemoteControl"}))
            {
                await _client.Start();

                while (true)
                {
                    Console.Write("Specify search query (or enter 'exit' to close app): ");
                    var query = Console.ReadLine();

                    if (query == "exit")
                    {
                        break;
                    }

                    await GoogleSearch(query);
                }
            }

            Console.ReadKey();
        }

        private static async Task GoogleSearch(string query)
        {
            var function = _client.RunFunction("GoogleSearch", new Params {["Query"] = query});
            var results = await function.GetTask();

            foreach (var result in results)
            {
                Console.WriteLine($"[!] - {result}");
            }
        }
    }
}