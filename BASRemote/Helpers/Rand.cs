using System;

namespace BASRemote.Helpers
{
    internal static class Rand
    {
        private static readonly Random Rnd = new Random();

        public static int NextInt(int min, int max)
        {
            return Rnd.Next(min, max);
        }
    }
}