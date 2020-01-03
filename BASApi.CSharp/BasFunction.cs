using System;
using BASApi.CSharp.Interfaces;

namespace BASApi.CSharp
{
    public sealed class BasFunction : IBasFunction
    {
        internal BasFunction()
        {
        }

        public object Result()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public int GetId()
        {
            throw new NotImplementedException();
        }
    }
}