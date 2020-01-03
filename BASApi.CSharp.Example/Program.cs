using System;
using System.Threading;

namespace BASApi.CSharp.Example
{
    internal class Program
    {
        private static void Main()
        {
            var Api = new BasApi();
            Api.Init(11686);

            Api.Stop(true);

            Console.ReadKey();
        }
    }
}