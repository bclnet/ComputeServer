using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tdstream.Server;

namespace Compute
{
    internal class HostService : IDisposable
    {
        const string AppName = nameof(HostService);
        public static readonly ManualResetEventSlim[] Ready = { new ManualResetEventSlim(false), new ManualResetEventSlim(false) };
        readonly ILogger _log;
        readonly IComputeServer _server;
        TdsServer _tdsServer;

        public HostService(ILogger<HostService> log, IComputeServer server)
        {
            _log = log;
            _server = server;
        }

        public void Dispose()
        {
            _tdsServer?.Dispose();
            Ready[0].Dispose();
            Ready[1].Dispose();
        }

        public bool IsStopping { get; set; }

        public async Task RunAsync(string dataPath, CancellationToken cancellationToken)
        {
            Compute.DataPath = dataPath ?? Compute.DataPath;
            try
            {
                var port = 1433; // TdsServer.FindFreeTcpPort();
                _tdsServer = new TdsServer(port, _server.LoginHandler, _server.QueryHandler);
                _tdsServer.Run();
                _server.Run();
                _log.LogInformation($"Listening on {port}");
                Ready[0].Wait(cancellationToken);
            }
            catch (Exception e)
            {
                //EmailManager.SendEmail(Config.EmailAppFrom, Config.EmailAppErrorTo, AppName, ex);
                Console.WriteLine($"Exception {e.Message}");
                if (IsStopping && e is OperationCanceledException)
                {
                    Ready[1].Set();
                    _log.LogInformation("Exit");
                    return;
                }
            }
        }

        public void Start() => _log.LogInformation(AppName);
    }
}
