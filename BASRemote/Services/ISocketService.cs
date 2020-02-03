using System;
using System.Threading.Tasks;
using BASRemote.Objects;

namespace BASRemote.Services
{
    /// <summary>
    ///     Provides methods for interacting with a BAS socket.
    /// </summary>
    internal interface ISocketService : IDisposable
    {
        event Action<Message> OnMessage;
        event Action OnClose;
        event Action OnOpen;
        Task StartSocketAsync(int port);
        void SendAsync(Message message);
    }
}