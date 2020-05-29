using Microsoft.Extensions.Configuration;

namespace Compute
{
    internal class Config
    {
        public static string EmailAppFrom => Configuration.GetValue<string>("EmailAppFrom");
        public static string EmailAppErrorTo => Configuration.GetValue<string>("EmailAppErrorTo");

        public static IConfiguration Configuration { get; set; }

        public static void LegacyDotNet(string app = "APP") =>
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables(prefix: $"{app}_")
                .Build();
    }
}