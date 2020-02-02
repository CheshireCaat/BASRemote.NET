using System;
using System.Threading;
using System.Threading.Tasks;
using BASRemote.Objects;

namespace BASRemote.Example
{
    internal class Program
    {
        //FileStream fs = File.Open("C:\\Steam\\session.dat", FileMode.Open, FileAccess.Read, FileShare.None);

        private static async Task Main()
        {
            var options = new BasRemoteOptions
            {
                WorkingDirectory = "C:\\Lenovo\\BAS Remote",
                ScriptName = "TestRemoveControlEx",
                Login = "likangt2012@gmail.com",
                Password = "10101990Gt"
            };

            using (var client = new BasRemoteClient(options))
            {
                await client.StartAsync();

                client.SendAsync<string>("script_report")
                    .Then(Console.WriteLine);

                var result = await client.SendAndWaitAsync<string>("script_report");
                
                Console.WriteLine($"[Task] \n\r {result}");
                Console.ReadKey();
            }

            Console.ReadKey();
        }

        public void Test()
        {
            //var thread = client.CreateThread();
            ////thread.RunFunction("CheckIp", new Params())
            ////    .Then(Console.WriteLine)
            ////    .Catch(e =>
            ////    {
            ////        Console.WriteLine(e.Message);
            ////    });
            //try
            //{
            //    var result = await thread.RunFunctionAsync("CheckIp", new Params());
            //    Console.WriteLine(result);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
        }
    }
}