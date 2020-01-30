using System;
using System.Threading;
using System.Threading.Tasks;

namespace BASApi.CSharp.Example
{
    internal class Program
    {
        private static async Task Main()
        {
            var options = new BasRemoteOptions
            {
                ScriptName = "TokMaster",
                Login = "likangt2012@gmail.com",
                Password = "10101990Gt",
                WorkingDir = "C:\\Users\\likan\\OneDrive\\Рабочий стол\\BASRemote-test"
            };
            var api = new BasRemoteClient(options);
            await api.Start();

            Console.ReadKey();
        }
    }
}