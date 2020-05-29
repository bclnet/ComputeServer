using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace Compute
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogger>(LogManager.GetLogger(string.Empty));
            services.AddTransient<HostService>();
            Compute.ConfigureServices(services);
        }
    }
}