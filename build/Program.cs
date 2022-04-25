namespace Build
{
    using System.CommandLine;
    using System.CommandLine.Builder;
    using System.CommandLine.Hosting;
    using System.CommandLine.Parsing;
    using Build.Commands;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;
    using Serilog;

    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var runner = BuildCommandLine()
                .UseHost(_ => CreateHostBuilder(args), (builder) => builder
                .ConfigureHostConfiguration((config) =>
                {
                    config.AddEnvironmentVariables();
                    config.AddCommandLine(args);
                })
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSerilog();
                })
                .UseCommandHandler<BuildCommand, BuildCommand.Handler>()
                ).UseDefaults().Build();


            return await runner.InvokeAsync(args);


            static CommandLineBuilder BuildCommandLine()
            {
                var rootCommand = new BuildCommand();
                rootCommand.AddGlobalOption(new Option<bool>(
                    "--silent",
                    description: "Disables diagnostics output"
                ));

                rootCommand.AddGlobalOption(new Option<string>(
                    "--build-dir",
                    description: "Directory of source code",
                    getDefaultValue: () => Environment.CurrentDirectory
                ));

                rootCommand.AddGlobalOption(new Option<string>(
                    "--cache-dir",
                    description: "Directory for cacheing",
                    getDefaultValue: () =>
                    {
                        string tempDirectory = Path.Combine(Path.GetTempPath(),
                            Path.GetRandomFileName());
                        Directory.CreateDirectory(tempDirectory);
                        return tempDirectory;
                    }
                ));

                rootCommand.AddGlobalOption(new Option<string>(
                    "--env-dir",
                    description: "Directory of environment files",
                    getDefaultValue: () => "/run/secrets"
                ));

                return new CommandLineBuilder(rootCommand);
            }

            static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args);
        }
    }
}
