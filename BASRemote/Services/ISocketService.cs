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
        /// <summary>
        ///     Occurs when <see cref="ISocketService"/> receives a message.
        /// </summary>
        event Action<Message> OnMessage;

        /// <summary>
        ///     Occurs when <see cref="ISocketService" /> connection has been closed.
        /// </summary>
        event Action OnClose;

        /// <summary>
        ///     Occurs when <see cref="ISocketService" /> connection has been opened.
        /// </summary>
        event Action OnOpen;

        Task StartSocketAsync(int port);

        void SendAsync(Message message);
    }
}