using System.Threading;

namespace BASRemote.Example.Extensions
{
    public static class BasThreadEx
    {
        public static void Wait(this IBasThread thread)
        {
            while (thread.IsRunning) Thread.Sleep(500);
        }
    }
}