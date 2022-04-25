namespace Build.Commands
{
    using System.CommandLine;
    using System.CommandLine.Invocation;
    using System.CommandLine.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.KeyPerFile;

    public class BuildCommand : RootCommand
    {
        public BuildCommand()
            : base("Build from source")
        {
            AddOption(new Option<string>(
                new[] {"--target", "-t"},
                description: "Target of the build",
                getDefaultValue: () => "BuildAndTest"
            ));

            AddOption(new Option<string>(
                new[] {"--configuration", "-c"},
                description: "Build configuration",
                getDefaultValue: () => "Release"
            ));
        }

        public new class Handler : ICommandHandler
        {
            private readonly ILogger<Handler> _log;
            private BuildConfiguration _config;

            public Handler(ILogger<Handler> logger, IConfiguration config)
            {
                _log = logger;

                _config = new BuildConfiguration(config);
            }

            public Task<int> InvokeAsync(InvocationContext context)
            {
                var buildDirectory
                    = context.ParseResult.ValueForOption<string>("--build-dir");
                var cacheDirectory
                    = context.ParseResult.ValueForOption<string>("--cache-dir");
                var environmentDirectory
                    = context.ParseResult.ValueForOption<string>("--env-dir");
                var target
                    = context.ParseResult.ValueForOption<string>("--target");
                var configuration
                    = context.ParseResult.ValueForOption<string>("--configuration");

                _log.LogInformation($"build-dir: {buildDirectory}");
                _log.LogInformation($"cache-dir: {cacheDirectory}");
                _log.LogInformation($"env-dir: {environmentDirectory}");

                _config = new BuildConfiguration(
                    new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json", false, true)
                        .AddEnvironmentVariables()
                        .AddKeyPerFile(
                            directoryPath: environmentDirectory == null ? ""
                                : environmentDirectory, optional: true
                        ).Build()
                    );

                context.Console.Out.WriteLine($"{target} @ {configuration}");
                context.Console.Out.WriteLine($"{_config.DotnetVersion} @ {_config.Configuration}");
                return Task.FromResult(0);
            }
        }
    }
}
