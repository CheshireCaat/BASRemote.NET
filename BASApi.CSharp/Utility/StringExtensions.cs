using System;

namespace BASApi.CSharp.Utility
{
    internal static class StringExtensions
    {
        public static string[] Split(this string source, string separator)
        {
            return source.Split(new[] {separator}, StringSplitOptions.None);
        }
    }
}