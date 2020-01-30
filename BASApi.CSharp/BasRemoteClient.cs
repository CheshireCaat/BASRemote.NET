using System;
using System.Threading.Tasks;
using BASApi.CSharp.Exceptions;
using BASApi.CSharp.Interfaces;
using BASApi.CSharp.Services;
using BASApi.CSharp.Utility;

namespace BASApi.CSharp
{
    /// <inheritdoc cref="IBasRemoteClient" />
    public sealed class BasRemoteClient : IBasRemoteClient
    {
        private readonly EngineService _engineService;

        private readonly SocketService _socketService;

        /// <summary>
        /// </summary>
        /// <param name="options"></param>
        public BasRemoteClient(BasRemoteOptions options)
        {
            if (string.IsNullOrEmpty(options.ScriptName)) throw new ArgumentNullException(nameof(options.ScriptName));
            if (string.IsNullOrEmpty(options.Password)) throw new ArgumentNullException(nameof(options.Password));
            if (string.IsNullOrEmpty(options.Login)) throw new ArgumentNullException(nameof(options.Login));

            _engineService = new EngineService(options);
            _socketService = new SocketService(options);
        }

        /// <inheritdoc />
        public async Task Start()
        {
            await _engineService.Initialize();

            if (_engineService.IsSupported) 
                throw new VersionNotSupportedException();

            await _engineService.GetExecutable();
        }

        public void Send(string type, dynamic data, bool isAsync = false)
        {
            throw new NotImplementedException();
        }

        public Task Async(string type, dynamic data)
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public async Task Stop()
        {
            await Task.Yield();
        }
    }
}