namespace Build
{
    using System.CommandLine.Parsing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Registers Serilog from configuration.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSerilog(this IServiceCollection services)
        {
            Log.Logger = CreateLogger(services);
            return services;
        }
        private static Serilog.Core.Logger CreateLogger(IServiceCollection services)
        {
            var scope = services.BuildServiceProvider();
            var parseResult = scope.GetRequiredService<ParseResult>();
            var isSilentLogger = parseResult.ValueForOption<bool>("--silent");
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(scope.GetRequiredService<IConfiguration>());

            if (isSilentLogger)
            {
                loggerConfiguration.MinimumLevel.Override("Build", Serilog.Events.LogEventLevel.Warning);
                loggerConfiguration.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning);
            }

            return loggerConfiguration.CreateLogger();
        }
    }
}

