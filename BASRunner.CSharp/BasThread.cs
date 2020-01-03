using System;
using BASRunner.CSharp.Interfaces;

namespace BASRunner.CSharp
{
    public sealed class BasThread : IBasThread
    {
        private bool _isRunning;

        private int _threadId;
        
        public BasThread()
        {
            _isRunning = false;
            _threadId = 0;
        }

        public IBasFunction RunFunction(string functionName, params (string Key, object Value)[] functionParameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsRunningFunction()
        {
            return _isRunning;
        }

        /// <inheritdoc />
        public void StopThread()
        {
            if (_threadId != 0)
            {
                //Api.RemoveTask(_threadId);
                //Api.Send("stop_thread", { thread_id: _threadId});
            }

            _isRunning = false;
            _threadId = 0;
        }

        /// <inheritdoc />
        public int GetId()
        {
            return _threadId;
        }
    }
}