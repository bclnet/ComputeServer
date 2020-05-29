using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace Compute
{
    class Program
    {
        public static IServiceProvider Services { get; set; }

        static Program()
        {
            Config.LegacyDotNet();
            var services = new ServiceCollection();
            services.AddLogging(logging =>
            {
                logging.AddNLog("nlog.config");
                logging.AddConsole();
            });
            Startup.ConfigureServices(services);
            Services = services.BuildServiceProvider();
        }

        static void Main(string[] args)
        {
            var log = Services.GetRequiredService<ILogger<Program>>();
            var service = Services.GetRequiredService<HostService>();
            var source = new CancellationTokenSource();
            var dataPath = args.FirstOrDefault();
            HostFactory.Run(x =>
            {
                x.SetServiceName("Compute Server");
                x.SetDisplayName("Compute Server");
                x.SetDescription("Compute Server");
                x
                .RunAsLocalSystem()
                .Service<HostService>(s =>
                {
                    s.ConstructUsing(tc => service);
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Dispose());
                    s.AfterStartingService(() =>
                        Task.Run(async () => await service.RunAsync(dataPath, source.Token), source.Token)
                    );
                    s.BeforeStoppingService(() =>
                    {
                        service.IsStopping = true;
                        source.Cancel();
                    });
                })
                .OnException((ex) =>
                    log.LogCritical(ex, "Exception")
                );
            });
        }
    }
}
