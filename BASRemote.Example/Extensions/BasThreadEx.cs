using System.Threading.Tasks;

namespace BASRemote.Example.Extensions
{
    public static class BasThreadEx
    {
        public static async Task Wait(this IBasThread thread)
        {
            while (thread.IsRunning) await Task.Delay(500);
        }
    }
}