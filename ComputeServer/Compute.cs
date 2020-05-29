using Compute.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Compute
{
    public static class Compute
    {
        public static string DataPath { get; set; } = @"C:\T_"; // Environment.CurrentDirectory;

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IComputeServer, ComputeServer>();
            services.AddTransient<IMasterFile, MasterFile>();
            services.AddTransient<IComputeFile, ComputeFile>();
        }
    }
}
