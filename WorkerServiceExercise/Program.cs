using CliWrap;

namespace WorkerServiceExercise
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            const string ServiceName = ".NET Joke Service";

            if (args is { Length: 1 })
            {
                try
                {
                    string executablePath =
                        Path.Combine(AppContext.BaseDirectory, "WorkerServiceExercise.exe");

                    if (args[0] is "/Install")
                    {
                        await Cli.Wrap("sc")
                            .WithArguments(new[] { "create", ServiceName, $"binPath={executablePath}", "start=auto" })
                            .ExecuteAsync();
                    }
                    else if (args[0] is "/Uninstall")
                    {
                        await Cli.Wrap("sc")
                            .WithArguments(new[] { "stop", ServiceName })
                            .ExecuteAsync();

                        await Cli.Wrap("sc")
                            .WithArguments(new[] { "delete", ServiceName })
                            .ExecuteAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return;
            }

            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = ".NET Joke Service";
            });

            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}