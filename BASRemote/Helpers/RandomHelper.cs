using System;

namespace BASRemote.Helpers
{
    internal static class RandomHelper
    {
        private static readonly Random Rand = new Random();

        public static int GenerateThreadId()
        {
            return Rand.Next(1, 1000000);
        }

        public static int GeneratePort()
        {
            return Rand.Next(10000, 20000);
        }
    }
}
