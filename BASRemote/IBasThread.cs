﻿using BASRemote.Interfaces;

namespace BASRemote
{
    /// <summary>
    /// </summary>
    public interface IBasThread : IFunctionRunner<IBasThread>
    {
        /// <summary>
        ///     Check if thread is already busy with running function.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        ///     Gets current thread id.
        /// </summary>
        int Id { get; }

        /// <summary>
        ///     Immediately stops thread execution.
        /// </summary>
        void Stop();
    }
}