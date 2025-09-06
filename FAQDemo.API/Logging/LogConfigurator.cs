using Serilog;
using Serilog.Events;

namespace FAQDemo.API.Logging
{
    public static class LogConfigurator
    {
        public static void ConfigureLogging(IConfiguration configuration, string environment)
        {
            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration) // Console/File + static enrichers from appsettings
                .Enrich.WithProperty("Environment", environment); // dynamic: always attach env name

            Log.Logger = loggerConfig.CreateLogger();
        }
    }
}